using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UI.Tooltip;
using UnityEngine;
using UnityEngine.UI;

namespace Upgrades
{
    public class SkillTreeNode : MonoBehaviour
    {
        [SerializeField] SkillTreeNode dependency;
        [SerializeField] bool startsUnlocked;
        
        [field: Header("Info")]
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public string Tagline { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        
        [Header("UI")]
        [SerializeField] Button button;
        [SerializeField] TextMeshProUGUI label;
        [SerializeField] TextMeshProUGUI tagline;

        [Header("Animation")]
        [SerializeField] Transform shaker;
        [SerializeField] Vector3 shakeAmount;
        [SerializeField] float shakeDuration;

        public string ID { get; private set; }
        public SkillTreeNode Parent => dependency;
        
        Action onActivate;
        Action onUnlock;

        TooltipTrigger tooltipTrigger;

        public static readonly List<SkillTreeNode> AllNodes = new();

        public int Level
        {
            get
            {
                var current = this;
                var level = 0;
                while (current.dependency)
                {
                    level++;
                    current = current.dependency;
                }
                return level;
            }
        }
        
        public enum NodeState
        {
            Locked = 0,
            Unlocked = 1,
            Active = 2
        }

        public NodeState State { get; private set; }

        public bool IsUnlocked => (int)State > 0;
        public bool IsActive => State == NodeState.Active;

        void Reset()
        {
            label = GetComponentInChildren<TextMeshProUGUI>();
            TryGetComponent(out button);
        }

        void OnDrawGizmos()
        {
            if (!dependency) return;
            Gizmos.DrawLine(transform.position, dependency.transform.position);
        }

        void OnValidate()
        {
            if (dependency == this) dependency = null;
            SetInfo();
        }

        public int GetDepth()
        {
            var node = this;
            var depth = 0;
            while (node.Parent)
            {
                depth++;
                node = node.Parent;
            }
            return depth;
        }

        void SetInfo()
        {
            if (label) name = label.text = DisplayName;
            if (tagline) tagline.text = Tagline;
        }

        void UpdateTooltip()
        {
            tooltipTrigger.titleText = $"{DisplayName} ({State})";
            tooltipTrigger.descriptionText = Description;
        }

        void Awake()
        {
            if (AllNodes.Count > 0 && !AllNodes[0]) AllNodes.Clear();
            
            ID = DisplayName.ToLower().Replace(" ", "");
            AllNodes.Add(this);

            button.interactable = startsUnlocked;
            if (startsUnlocked) State = NodeState.Unlocked;
            
            button.onClick.AddListener(Activate);
            
            tooltipTrigger = gameObject.AddComponent<TooltipTrigger>();
            UpdateTooltip();
            SetInfo();

            if (dependency) dependency.onActivate += Unlock;
        }

        public void InitState(NodeState state)
        {
            switch (state)
            {
                case NodeState.Locked:
                    break;
                case NodeState.Unlocked:
                    if (!startsUnlocked)
                        Unlock();
                    break;
                case NodeState.Active:
                    if (!startsUnlocked)
                        Unlock();
                    Activate();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        void Unlock()
        {
            if (startsUnlocked) throw new Exception("Called Unlock on a node that starts unlocked.");
            if (IsUnlocked) return;
            
            button.interactable = true;
            State = NodeState.Unlocked;
            onUnlock?.Invoke();
            UpdateTooltip();
        }

        void Activate()
        {
            if (IsActive) return;
            State = NodeState.Active;
            var colors = button.colors;
            colors.disabledColor = colors.pressedColor;
            button.colors = colors;
            button.interactable = false;
            label.color = colors.pressedColor;
            onActivate?.Invoke();
            UpdateTooltip();

            var seq = DOTween.Sequence();
            seq.Append(shaker.DOMove(shakeAmount, shakeDuration / 4).SetRelative());
            seq.Append(shaker.DOMove(-shakeAmount * 2, shakeDuration / 2).SetRelative());
            seq.Append(shaker.DOMove(transform.position, shakeDuration / 4));
            
        }
    }
}
