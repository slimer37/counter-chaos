using Core;
using Interactables.Tools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Furniture
{
    public class FurnitureMover : Tool
    {
        [SerializeField] Vector3 pos;
        [SerializeField] Vector3 rot;
        [SerializeField] LayerMask ground;
        
        Transform cargo;
        Furniture furniture;

        protected override void OnEquip(Transform sender)
        {
            transform.parent = sender;
            transform.localPosition = pos;
            transform.localRotation = Quaternion.Euler(rot);
        }

        protected override void OnDequip(Transform sender)
        {
            transform.parent = null;
            transform.localRotation = Quaternion.identity;
        }

        public override bool IsCompatibleWith(Toolable toolable)
        {
            furniture = toolable as Furniture;
            return furniture;
        }

        public override void UseWith(Toolable toolable)
        {
            furniture.OnCarry();
            furniture.transform.root.gameObject.SetActive(false);
        }

        void FixedUpdate()
        {
            if (!Equipped) return;
            Physics.Raycast(transform.position, Vector3.down, out var hit, 5, ground);
            transform.localPosition = pos;
            transform.position += Vector3.up * hit.point.y;
        }

        protected override void OnUse(InputAction.CallbackContext ctx)
        {
            if (ctx.canceled || !furniture) return;
            furniture.transform.root.gameObject.SetActive(true);

            var ray = Player.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out var hit, 5))
            {
                furniture.transform.root.position = hit.point;
                furniture.OnPlace();
                furniture = null;
            }
        }
    }
}