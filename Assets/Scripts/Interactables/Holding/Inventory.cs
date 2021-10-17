﻿using System;
using Core;
using DG.Tweening;
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
                    
                    var outline = value.GetComponent<Plugins.Outline>();
                    if (outline) outline.enabled = false;

                    previewTex = RuntimePreviewGenerator.GenerateModelPreview(value.transform, dim.x, dim.y);

                    if (outline) outline.enabled = true;

                    thumbnail.texture = previewTex;
                    thumbnail.enabled = true;
                }
            }
            
            Pickuppable content;
            Texture2D previewTex;

            readonly RawImage thumbnail;
            readonly Vector2Int dim;

            public bool IsFilled => content != null;
            public readonly Transform transform;

            public Slot(GameObject obj, Vector2Int dimensions)
            {
                dim = dimensions;
                transform = obj.transform;
                
                thumbnail = transform.GetChild(0).GetComponent<RawImage>()
                       ?? throw new Exception("Slot template's first child is not a raw image.");
                thumbnail.enabled = false;
                
                content = null;
                
                previewTex = null;
            }

            public void Clear()
            {
                content = null;
                thumbnail.enabled = false;
                Destroy(previewTex);
            }
        }
        
        [SerializeField, Min(0)] int numSlots;
        [SerializeField] GameObject slotTemplate;
        [SerializeField] Transform activeImage;
        [SerializeField] bool invertScrollDirection;

        [Header("Hiding Animation")]
        [SerializeField] bool autoHide = true;
        [SerializeField, Min(0)] float hideDelay = 1.5f;
        [SerializeField, Min(0)] float moveOffDuration = 0.1f;
        [SerializeField, Min(0)] float timeScaleGoingUp = 2;

        [Header("Thumbnails")]
        [SerializeField, Min(1)] Vector2Int previewDimensions = new Vector2Int(64, 64);
        [SerializeField] bool orthographic;
        
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
        
        bool CanSwitch => Time.timeScale != 0 && !interactHeld && !secInteractHeld;
        bool interactHeld;
        bool secInteractHeld;

        void Awake()
        {
            RuntimePreviewGenerator.MarkTextureNonReadable = true;
            RuntimePreviewGenerator.BackgroundColor = Color.clear;
            RuntimePreviewGenerator.OrthographicMode = orthographic;
            
            var rect = GetComponent<RectTransform>();
            moveOffScreen = DOTween.Sequence();
            moveOffScreen.Append(rect.DOPivotY(1, moveOffDuration).SetAutoKill(false));
            moveOffScreen.Join(rect.DOMoveY(0, moveOffDuration).SetAutoKill(false));
            moveOffScreen.SetAutoKill(false);
            moveOffScreen.Complete();
            inactiveTime = hideDelay + 1;
            
            Main = this;
            
            slots = new Slot[numSlots];
            slots[0] = new Slot(slotTemplate, previewDimensions);
            
            for (var i = 1; i < numSlots; i++)
                slots[i] = new Slot(Instantiate(slotTemplate, transform), previewDimensions);

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

        void Update()
        {
            if (!autoHide) return;
            
            inactiveTime += Time.deltaTime;

            var goDown = inactiveTime > hideDelay;
            if (goDown != moveOffScreen.isBackwards) return;

            moveOffScreen.timeScale = goDown ? 1 : timeScaleGoingUp;
            
            if (goDown) moveOffScreen.PlayForward();
            else moveOffScreen.PlayBackwards();
        }

        void SetActiveSlot(int index, bool newPickup)
        {
            if (index < 0 || index >= numSlots) throw new IndexOutOfRangeException($"Slot {index} does not exist.");
            
            ActiveSlotIndex = index;
            activeImage.transform.position = slots[ActiveSlotIndex].transform.position;

            inactiveTime = 0;
            
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
            inactiveTime = 0;
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
