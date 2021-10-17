﻿using System;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Interactables.Holding
{
    public class Inventory : MonoBehaviour
    {
        struct Slot
        {
            public Pickuppable Content
            {
                get => content;
                set
                {
                    if (value == null) throw new Exception("Use Clear() to empty a slot.");
                    if (content != null) throw new Exception("Attempted to assign occupied slot.");
                    content = value;
                    label.text = content.name;
                    icon.enabled = true;
                }
            }

            public bool IsFilled => content != null;
            public Transform Transform { get; private set; }
            
            TextMeshProUGUI label;
            Pickuppable content;
            Image icon;

            public Slot(GameObject obj)
            {
                Transform = obj.transform;
                icon = Transform.GetChild(0).GetComponent<Image>()
                       ?? throw new Exception("Slot template's first child is not an image.");
                icon.enabled = false;
                label = obj.GetComponentInChildren<TextMeshProUGUI>()
                        ?? throw new Exception("Slot template does not contain text.");
                label.text = "";
                content = null;
            }

            public void Clear()
            {
                content = null;
                label.text = "";
                icon.enabled = false;
            }
        }
        
        [SerializeField, Min(0)] int numSlots;
        [SerializeField] GameObject slotTemplate;
        [SerializeField] Transform activeImage;
        [SerializeField] bool invertScrollDirection;
        
        [field: SerializeField] public ItemHolder Holder { get; private set; }

        Slot[] slots;
        int numItems;
        
        public static Inventory Main { get; private set; }
        
        public int ActiveSlotIndex { get; private set; }
        public int AvailableSlots => numSlots - numItems;
        public bool IsFull => AvailableSlots == 0;

        Controls controls;
        
        bool CanSwitch => !interactHeld && !secInteractHeld;
        bool interactHeld;
        bool secInteractHeld;

        void Awake()
        {
            Main = this;
            
            slots = new Slot[numSlots];
            slots[0] = new Slot(slotTemplate);
            
            for (var i = 1; i < numSlots; i++)
                slots[i] = new Slot(Instantiate(slotTemplate, transform));

            Keyboard.current.onTextInput += c => {
                if (CanSwitch && int.TryParse(c.ToString(), out var i) && i > 0 && i <= numSlots)
                    SetActiveSlot(i - 1, false);
            };

            controls = new Controls();
            controls.Enable();
            controls.Gameplay.Interact.performed += _ => interactHeld = true;
            controls.Gameplay.Interact.canceled += _ => interactHeld = false;
            controls.Gameplay.SecondaryInteract.performed += _ => secInteractHeld = true;
            controls.Gameplay.SecondaryInteract.canceled += _ => secInteractHeld = false;

            controls.Gameplay.Scroll.performed += ctx => {
                if (!CanSwitch) return;
                var delta = (invertScrollDirection ? -1 : 1) * (ctx.ReadValue<float>() > 0 ? 1 : -1);
                var newIndex = (ActiveSlotIndex + delta + numSlots) % numSlots;
                SetActiveSlot(newIndex, false);
            };
        }

        void OnDestroy() => controls.Dispose();

        void SetActiveSlot(int index, bool newPickup)
        {
            if (index < 0 || index >= numSlots) throw new IndexOutOfRangeException($"Slot {index} does not exist.");
            
            ActiveSlotIndex = index;
            activeImage.transform.position = slots[ActiveSlotIndex].Transform.position;
            
            if (Holder.IsHoldingItem)
                Holder.StopHolding().gameObject.SetActive(false);
            
            if (slots[index].IsFilled)
            {
                var obj = slots[index].Content.gameObject;

                if (!newPickup)
                {
                    var player = transform.root;
                    obj.transform.position = player.position;
                    obj.transform.rotation = player.rotation;
                }
                
                obj.SetActive(true);
                Holder.Hold(slots[index].Content, this);
            }
        }

        public Pickuppable ClearSlot(int index)
        {
            numItems--;
            
            var temp = slots[index].Content;
            if (index == ActiveSlotIndex && Holder.IsHoldingItem) Holder.StopHolding();
            slots[index].Clear();
            return temp;
        }

        public Pickuppable ClearActiveSlot() => ClearSlot(ActiveSlotIndex);

        public bool TryGive(Pickuppable item)
        {
            if (IsFull) return false;

            var success = false;
            
            var i = ActiveSlotIndex;
            for (var j = 0; j < slots.Length; j++)
            {
                var index = (i + j) % numSlots;
                if (slots[index].IsFilled) continue;
                slots[index].Content = item;
                success = true;
                break;
            }
            
            if (!success) return false;
            
            SetActiveSlot(i, true);
            return true;
        }
    }
}
