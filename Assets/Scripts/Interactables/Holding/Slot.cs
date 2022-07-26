using System;
using UnityEngine;
using UnityEngine.UI;

namespace Interactables.Holding
{
    internal class Slot
    {
        public Item Item
        {
            get => item;
            set
            {
                if (!value) throw new NullReferenceException(
                    $"Cannot assign null to slot. Did you mean {nameof(Clear)}()?"
                );
                if (item) throw new Exception("Attempted to assign occupied slot.");
                
                item = value;
                
                var highlight = value.GetComponent<Base.Highlight>();
                if (highlight) highlight.enabled = false;

                previewTex = Preview.Thumbnail.Grab(item.Info.label, item.transform);

                if (highlight) highlight.enabled = true;

                thumbnail.texture = previewTex;
                thumbnail.enabled = true;
            }
        }
        
        Item item;
        Texture2D previewTex;

        readonly RawImage thumbnail;

        public bool HasItem => item != null;
        public readonly Transform transform;

        public Slot(GameObject obj)
        {
            transform = obj.transform;
            
            thumbnail = transform.GetChild(0).GetComponent<RawImage>()
                        ?? throw new Exception("Slot template's first child is not a raw image.");
            thumbnail.enabled = false;
        }

        public void Clear()
        {
            item = null;
            thumbnail.enabled = false;
        }
    }
}