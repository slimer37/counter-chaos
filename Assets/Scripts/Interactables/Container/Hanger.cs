using Interactables.Base;
using Interactables.Holding;
using UnityEngine;

namespace Interactables.Container
{
    [RequireComponent(typeof(ContainerPositioner))]
    public class Hanger : MonoBehaviour, IInteractHandler
    {
        Hoverable hoverable;
        ContainerPositioner positioner;
        Pickuppable hungItem;

        void Reset() => TryGetComponent(out hoverable);

        void Awake()
        {
            positioner = GetComponent<ContainerPositioner>();
            hoverable = GetComponent<Hoverable>();
            
            hoverable.OnAttemptHover +=
                sender =>
                    !hungItem && Inventory.Main.Holder.IsHoldingItem && Inventory.Main.Holder.HeldItem.Info.canBeHung
                    || hungItem && !Inventory.Main.Holder.IsHoldingItem;
        }

        public void OnInteract(Transform sender)
        {
            if (!sender.CompareTag("Player")) return;

            var inventory = Inventory.Main;

            if (!inventory.Holder.IsHoldingItem)
            {
                if (hungItem && positioner.TryGiveToPlayer(0))
                    hungItem = null;
            }
            else if (!hungItem)
            {
                hungItem = inventory.ClearActiveSlot();
                positioner.PlaceInPosition(hungItem.transform, 0);
            }
        }
    }
}
