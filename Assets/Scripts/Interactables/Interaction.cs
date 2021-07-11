using UnityEngine;
using Core;

namespace Interactables
{
    public class Interaction : MonoBehaviour
    {
        [SerializeField] float reach;
        [SerializeField, Layer] int interactablesLayer;
        [SerializeField] new Camera camera;
        [SerializeField] IconHandler iconHandler;

        Hoverable hoveredTransform;

        void OnInteract() => hoveredTransform?.GetComponent<IInteractable>()?.OnInteract(transform);

        void Update()
        {
            if (Physics.Raycast(camera.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out var hit, reach))
            {
                if (hit.transform.gameObject.layer != interactablesLayer)
                {
                    HoverOff();
                    return;
                }
                
                var hoverable = hit.transform.GetComponent<Hoverable>();
                
                if (hoverable)
                    (hoveredTransform = hoverable).OnHover(iconHandler);
            }
            else
                HoverOff();

            void HoverOff()
            {
                if (!hoveredTransform) return;
                hoveredTransform.OnHoverExit();
                hoveredTransform = null;
            }
        }
    }
}
