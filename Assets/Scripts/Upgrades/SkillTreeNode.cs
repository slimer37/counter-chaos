using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
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
        
        [Header("UI")]
        [SerializeField] Button button;
        [SerializeField] Image background;
        [SerializeField] TextMeshProUGUI label;

        public string ID { get; private set; }
        
        Action onActivate;
        Action onUnlock;
        
        Image dependencyLine;
        RectTransform dependencyLineRect;

        public static readonly List<SkillTreeNode> AllNodes = new List<SkillTreeNode>();
        
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
            TryGetComponent(out background);
            TryGetComponent(out button);
        }

        void OnDrawGizmos()
        {
            if (!dependency) return;
            Gizmos.DrawLine(transform.position, dependency.transform.position);
        }

        void OnValidate() => SetInfo();

        void SetInfo() => name = label.text = DisplayName;

        void Awake()
        {
            ID = DisplayName.ToLower().Replace(" ", "");
            AllNodes.Add(this);

            button.interactable = startsUnlocked;
            if (startsUnlocked) State = NodeState.Unlocked;
            
            button.onClick.AddListener(Activate);
            
            SetInfo();
            GenerateLine();

            if (dependency)
            {
                dependency.onActivate += Unlock;
                
                if (dependency.startsUnlocked) DependencyUnlock();
                else dependency.onUnlock += DependencyUnlock;
            }
        }

        void DependencyUnlock()
        {
            var colorBlock = dependency.button.colors;
            dependencyLine.DOColor(colorBlock.normalColor, colorBlock.fadeDuration);
        }

        void Unlock()
        {
            if (startsUnlocked) throw new Exception("Called Unlock on a node that starts unlocked.");
            if (IsUnlocked) return;
            
            button.interactable = true;
            State = NodeState.Unlocked;
            onUnlock?.Invoke();

            if (dependencyLine)
            {
                var colorBlock = dependency.button.colors;
                dependencyLine.DOColor(colorBlock.pressedColor, colorBlock.fadeDuration);
            }
        }

        void Activate()
        {
            if (IsActive) return;
            State = NodeState.Active;
            background.color = button.colors.pressedColor;
            button.enabled = false;
            onActivate?.Invoke();
        }

        void Update() => CalculateLinePos();

        void GenerateLine()
        {
            if (!dependency) return;

            dependencyLine = new GameObject("line").AddComponent<CanvasRenderer>().gameObject.AddComponent<Image>();
            dependencyLineRect = dependencyLine.GetComponent<RectTransform>();
            dependencyLineRect.SetParent(transform.parent, false);
            dependencyLineRect.SetAsFirstSibling();
            dependencyLine.color = button.colors.disabledColor;
            
            CalculateLinePos();
        }

        void CalculateLinePos()
        {
            if (!dependency) return;
            
            var difference = dependency.transform.position - transform.position;
            dependencyLineRect.position = transform.position + difference / 2;
            dependencyLineRect.sizeDelta = new Vector2(10, difference.magnitude);
            dependencyLineRect.eulerAngles = Vector3.forward * (Mathf.Atan2(difference.y, difference.x) * 180 / Mathf.PI + 90);
        }
    }
}