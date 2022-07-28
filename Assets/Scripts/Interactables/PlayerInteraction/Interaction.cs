using Core;
using Interactables.Base;
using UnityEngine;

namespace Interactables
{
    internal class Interaction : MonoBehaviour
    {
        [SerializeField] InputProvider input;
        [SerializeField] float reach;
        [SerializeField, Layer] int interactablesLayer;
        [SerializeField] LayerMask inclusionMask;
        [SerializeField] new Camera camera;
        [SerializeField] IconHandler iconHandler;

        Hoverable hoveredObject;

        void Awake()
        {
            input.StartInteract += OnStartInteract;
            input.StopInteract += OnStopInteract;
            input.StartSecondaryInteract += OnStartSecondaryInteract;
            input.StopSecondaryInteract += OnStopSecondaryInteract;
        }

        void OnDestroy()
        {
            input.StartInteract -= OnStartInteract;
            input.StopInteract -= OnStopInteract;
            input.StartSecondaryInteract -= OnStartSecondaryInteract;
            input.StopSecondaryInteract -= OnStopSecondaryInteract;
        }

        void OnStartInteract() => OnInteract(true);
        void OnStopInteract() => OnInteract(false);
        void OnStartSecondaryInteract() => OnSecondaryInteract(true);
        void OnStopSecondaryInteract() => OnSecondaryInteract(false);

        void OnInteract(bool pressed)
        {
            if (!hoveredObject) return;
            hoveredObject.Interact(transform, pressed, false);
        }

        void OnSecondaryInteract(bool pressed)
        {
            if (!hoveredObject) return;
            hoveredObject.Interact(transform, pressed, true);
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
                hoveredObject.Interact(transform, false, false);
                hoveredObject.Interact(transform, false, true);
                hoveredObject.OnHoverExit();
                hoveredObject = null;
            }
        }
    }
}
