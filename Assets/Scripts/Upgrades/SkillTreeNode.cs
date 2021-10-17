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
        [field: SerializeField] public string ID { get; private set; }
        
        [Header("UI")]
        [SerializeField] Button button;
        [SerializeField] Image background;
        [SerializeField] TextMeshProUGUI label;

        bool isUnlocked;
        bool isActive;
        Action onActivate;
        Action onUnlock;
        
        public static IReadOnlyList<SkillTreeNode> AllNodes => allNodes.AsReadOnly();
        static List<SkillTreeNode> allNodes = new List<SkillTreeNode>();

        Image dependencyLine;
        RectTransform dependencyLineRect;

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
            allNodes.Add(this);

            isUnlocked = button.interactable = startsUnlocked;
            
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
            if (isUnlocked) return;
            
            isUnlocked = button.interactable = true;
            onUnlock?.Invoke();

            if (dependencyLine)
            {
                var colorBlock = dependency.button.colors;
                dependencyLine.DOColor(colorBlock.pressedColor, colorBlock.fadeDuration);
            }
        }

        void Activate()
        {
            if (isActive) return;
            isActive = true;
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
