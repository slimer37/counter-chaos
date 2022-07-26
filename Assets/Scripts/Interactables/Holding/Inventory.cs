using System;
using Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactables.Holding
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField, Min(0)] int numSlots;
        [SerializeField] GameObject slotTemplate;
        [SerializeField] Transform activeImage;
        [SerializeField] bool invertScrollDirection;

        [Header("Hiding Animation")]
        [SerializeField] bool autoHide = true;
        [SerializeField, Min(0)] float hideDelay = 1.5f;
        [SerializeField, Min(0)] float moveOffDuration = 0.1f;
        [SerializeField, Min(0)] float timeScaleGoingUp = 2;
        
        [field: SerializeField] public ItemHolder Holder { get; private set; }

        Slot[] slots;
        int numItems;
        
        Sequence moveOffScreen;
        float inactiveTime;
        
        public static Inventory Main { get; private set; }
        
        public int ActiveSlotIndex { get; private set; }
        public int AvailableSlots => numSlots - numItems;
        public bool IsFull => AvailableSlots == 0;

        Controls controls;
        
        bool CanSwitch => Time.timeScale != 0
                          && !controls.Gameplay.Interact.IsPressed()
                          && !controls.Gameplay.SecondaryInteract.IsPressed()
                          && !Holder.IsDroppingItem;

        void Awake()
        {
            Main = this;
            
            ConstructMoveOffTween();
            GenerateSlots();

            Keyboard.current.onTextInput += OnTextInput;

            controls = new Controls();
            controls.Enable();

            controls.Gameplay.Scroll.performed += OnScroll;
        }

        void OnScroll(InputAction.CallbackContext ctx)
        {
            if (!CanSwitch) return;
            var delta = (invertScrollDirection ? -1 : 1) * (ctx.ReadValue<float>() > 0 ? 1 : -1);
            var newIndex = (ActiveSlotIndex + delta + numSlots) % numSlots;
            SetActiveSlot(newIndex);
        }
        
        void OnTextInput(char c)
        {
            if (!CanSwitch) return;
            if (int.TryParse(c.ToString(), out var numInput) && numInput > 0 && numInput <= numSlots)
                SetActiveSlot(numInput - 1);
        }

        void OnDestroy()
        {
            controls.Dispose();
            Keyboard.current.onTextInput -= OnTextInput;
        }

        void Update()
        {
            if (!autoHide) return;
            
            inactiveTime += Time.deltaTime;

            var shouldHide = inactiveTime > hideDelay;
            var isHidden = !moveOffScreen.isBackwards;
            
            if (shouldHide == isHidden) return;

            moveOffScreen.timeScale = shouldHide ? 1 : timeScaleGoingUp;
            
            if (shouldHide) moveOffScreen.PlayForward();
            else moveOffScreen.PlayBackwards();
        }

        void SetActiveSlot(int index, bool force = false)
        {
            if (index < 0 || index >= numSlots) throw new IndexOutOfRangeException($"Slot {index} does not exist.");
            if (!force && index == ActiveSlotIndex) return;
            
            ActiveSlotIndex = index;
            activeImage.transform.position = slots[ActiveSlotIndex].transform.position;

            inactiveTime = 0;
            
            if (Holder.IsHoldingItem)
                Holder.StopHolding().gameObject.SetActive(false);

            if (!slots[index].HasItem) return;
            
            var obj = slots[index].Item.gameObject;
            
            var player = transform.root;
            obj.transform.SetPositionAndRotation(player.position, player.rotation);
            
            obj.SetActive(true);
            Holder.Hold(slots[index].Item);
        }

        public Pickuppable GetSlotContent(int index) => slots[index].Item;

        public Pickuppable ClearSlot(int index, bool destroy = false)
        {
            if (index > numSlots) throw new ArgumentOutOfRangeException(nameof(index));
            if (!slots[index].Item) return null;
            
            inactiveTime = 0;
            numItems--;
            
            var temp = slots[index].Item;
            if (index == ActiveSlotIndex && Holder.IsHoldingItem) Holder.StopHolding();
            if (destroy) Destroy(slots[index].Item.gameObject);
            slots[index].Clear();
            return temp;
        }

        public Pickuppable ClearActiveSlot(bool destroy = false) => ClearSlot(ActiveSlotIndex, destroy);

        public void ClearAll(bool destroy = false)
        {
            for (var i = 0; i < numSlots; i++)
                ClearSlot(i, destroy);
        }

        public bool TryGive(Pickuppable item)
        {
            if (IsFull) return false;
            
            var i = ActiveSlotIndex;
            for (var j = 0; j < slots.Length; j++)
            {
                var index = (i + j) % numSlots;
                
                if (slots[index].HasItem) continue;
                
                slots[index].Item = item;
                
                // Record successful slot to switch to.
                i = index;
                
                break;
            }

            numItems++;
            SetActiveSlot(i, true);
            return true;
        }

        void ConstructMoveOffTween()
        {
            var rect = transform as RectTransform;
            moveOffScreen = DOTween.Sequence()
                .Append(rect.DOPivotY(1, moveOffDuration))
                .Join(rect.DOMoveY(0, moveOffDuration))
                .SetAutoKill(false);
            moveOffScreen.Complete();
        }

        void GenerateSlots()
        {
            slots = new Slot[numSlots];
            slots[0] = new Slot(slotTemplate);
            
            for (var i = 1; i < numSlots; i++)
                slots[i] = new Slot(Instantiate(slotTemplate, transform));
        }
    }
}
