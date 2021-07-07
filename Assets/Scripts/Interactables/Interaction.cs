using UnityEngine;

namespace Interactables
{
    public class Interaction : MonoBehaviour
    {
        [SerializeField] float reach;
        [SerializeField] LayerMask mask;
        [SerializeField] new Camera camera;

        Hoverable hoveredTransform;

        void OnInteract() => hoveredTransform?.GetComponent<IInteractable>()?.OnInteract(transform);

        void Update()
        {
            if (Physics.Raycast(camera.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out var hit, reach, mask))
            {
                var hoverable = hit.transform.GetComponent<Hoverable>();
                
                if (hoverable)
                    (hoveredTransform = hoverable).OnHover();
            }
            else if (hoveredTransform)
            {
                hoveredTransform.OnHoverExit();
                hoveredTransform = null;
            }
        }
    }
}
