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
        [SerializeField] LayerMask inclusionMask;
        [SerializeField] new Camera camera;
        [SerializeField] IconHandler iconHandler;
        [SerializeField] InteractionChannel channel;

        Hoverable hoveredObject;

        void Awake()
        {
            if (channel.IsActive)
                 channel.Deactivate();

            channel.OnActivate += HoldInteractionStarted;
        }

        void HoldInteractionStarted(bool exitHover)
        {
            if (exitHover)
                HoverOff();
        }

        void OnInteract(InputValue value)
        {
            if (channel.IsActive || !hoveredObject) return;
            hoveredObject.Interact(transform, value.isPressed, false);
        }

        void OnSecondaryInteract(InputValue value)
        {
            if (channel.IsActive || !hoveredObject) return;
            hoveredObject.Interact(transform, value.isPressed, true);
        }

        void Update()
        {
            if (channel.IsActive) return;
            
            var eyeRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            
            if (Physics.Raycast(eyeRay, out var hit, reach, inclusionMask)
                && hit.transform.gameObject.layer == interactablesLayer)
            {
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
                (hoveredObject = hoverable)?.OnHover(iconHandler, transform);
            }
            else
                HoverOff();
        }
        
        void HoverOff()
        {
            if (!hoveredObject) return;
            hoveredObject.Interact(transform, false, false);
            hoveredObject.Interact(transform, false, true);
            hoveredObject.OnHoverExit();
            hoveredObject = null;
        }
    }
}
