using System;
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

        Hoverable hoveredObject;

        void Call(Func<IInteractable, Action<Transform>> action)
        {
            var t = transform;
            if (!hoveredObject || !hoveredObject.TryGetInteractable(t, out var interactable)) return;
            var interactionFunc = action.Invoke(interactable);
            interactionFunc.Invoke(t);
        }

        void OnInteract(InputValue value)
        {
            if (value.isPressed) Call(i => i.OnInteract);
            else Call(i => i.OnStopInteract);
        }

        void OnSecondaryInteract(InputValue value)
        {
            if (value.isPressed) Call(i => i.OnSecondaryInteract);
            else Call(i => i.OnStopSecondaryInteract);
        }

        void Update()
        {
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

            void HoverOff()
            {
                if (!hoveredObject) return;
                Call(i => i.OnStopInteract);
                Call(i => i.OnStopSecondaryInteract);
                hoveredObject.OnHoverExit();
                hoveredObject = null;
            }
        }
    }
}
