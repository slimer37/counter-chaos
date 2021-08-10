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

        void CallOnEach<THandler>(Action<THandler> action)
        {
            if (!hoveredObject) return;
            var handlers = hoveredObject.GetOnce<THandler>();
            foreach (var handler in handlers)
            {
                if (!(handler as MonoBehaviour).enabled) continue;
                action(handler);
            }
        }

        void OnInteract(InputValue value)
        {
            if (value.isPressed)
                CallOnEach<IInteractHandler>(handler => handler.OnInteract(transform));
            else
                CallOnEach<IStopInteractHandler>(handler => handler.OnStopInteract(transform));
        }

        void OnSecondaryInteract(InputValue value)
        {
            if (value.isPressed)
                CallOnEach<ISecondaryInteractHandler>(handler => handler.OnSecondaryInteract(transform));
            else
                CallOnEach<IStopSecondaryInteractHandler>(handler => handler.OnStopSecondaryInteract(transform));
        }

        void Update()
        {
            if (Physics.Raycast(camera.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out var hit, reach, inclusionMask))
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
                
                if (hoverable && hoverable.enabled)
                    (hoveredObject = hoverable).OnHover(iconHandler, transform);
            }
            else
                HoverOff();

            void HoverOff()
            {
                if (!hoveredObject) return;
                CallOnEach<IStopInteractHandler>(handler => handler.OnStopInteract(transform));
                CallOnEach<IStopSecondaryInteractHandler>(handler => handler.OnStopSecondaryInteract(transform));
                hoveredObject.OnHoverExit();
                hoveredObject = null;
            }
        }
    }
}
