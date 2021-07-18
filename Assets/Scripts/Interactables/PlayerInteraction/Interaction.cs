using Core;
using Interactables.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactables
{
    internal class Interaction : MonoBehaviour
    {
        [SerializeField] float reach;
        [SerializeField, Layer] int interactablesLayer;
        [SerializeField] new Camera camera;
        [SerializeField] IconHandler iconHandler;

        Hoverable hoveredObject;

        void OnInteract(InputValue value)
        {
            if (value.isPressed)
                hoveredObject?.GetComponent<IInteractHandler>()?.OnInteract(transform);
            else
                hoveredObject?.GetComponent<IStopInteractHandler>()?.OnStopInteract(transform);
        }

        void OnSecondaryInteract(InputValue value)
        {
            if (value.isPressed)
                hoveredObject?.GetComponent<ISecondaryInteractHandler>()?.OnSecondaryInteract(transform);
            else
                hoveredObject?.GetComponent<IStopSecondaryInteractHandler>()?.OnStopSecondaryInteract(transform);
        }

        void Update()
        {
            if (Physics.Raycast(camera.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out var hit, reach))
            {
                if (hit.transform.gameObject.layer != interactablesLayer)
                {
                    HoverOff();
                    return;
                }

                if (hoveredObject)
                {
                    if (hoveredObject.transform != hit.transform)
                        HoverOff();
                    else
                    {
                        hoveredObject.OnHover(iconHandler, transform);
                        return;
                    }
                }
                
                var hoverable = hit.transform.GetComponent<Hoverable>();
                
                if (hoverable)
                    (hoveredObject = hoverable).OnHover(iconHandler, transform);
            }
            else
                HoverOff();

            void HoverOff()
            {
                if (!hoveredObject) return;
                hoveredObject.GetComponent<IStopInteractHandler>()?.OnStopInteract(transform);
                hoveredObject.GetComponent<IStopSecondaryInteractHandler>()?.OnStopSecondaryInteract(transform);
                hoveredObject.OnHoverExit();
                hoveredObject = null;
            }
        }
    }
}
