using UnityEngine;
using Core;
using UnityEngine.InputSystem;

namespace Interactables
{
    public class Interaction : MonoBehaviour
    {
        [SerializeField] float reach;
        [SerializeField, Layer] int interactablesLayer;
        [SerializeField] new Camera camera;
        [SerializeField] IconHandler iconHandler;

        Hoverable hoveredObject;

        void OnInteract(InputValue value)
        {
            if (value.isPressed)
                hoveredObject?.GetComponent<IInteractable>()?.OnInteract(transform);
            else
                hoveredObject?.GetComponent<IStopInteractHandler>()?.OnStopInteract(transform);
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
                    if (hoveredObject.transform == hit.transform)
                        hoveredObject.OnHover(iconHandler, transform);
                    else
                        HoverOff();
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
                hoveredObject.OnHoverExit();
                hoveredObject = null;
            }
        }
    }
}
