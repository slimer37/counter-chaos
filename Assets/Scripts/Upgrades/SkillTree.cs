using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Serialization;
using UnityEngine;

namespace Upgrades
{
    public class SkillTree : MonoBehaviour
    {
        const char Separator = '|';

        void Start() => ApplySaveString(SaveSystem.LoadedSave.upgrades);
        void OnDestroy()
        {
            SaveSystem.LoadedSave.upgrades = GenerateSaveString();
            SaveSystem.LoadedSave.Save();
        }

        [ContextMenu("Generate"), Pure]
        string GenerateSaveString()
        {
            SkillTreeNode.AllNodes.Sort((node1, node2) =>
                string.Compare(node1.DisplayName, node2.DisplayName, StringComparison.Ordinal));

            var upgradesString = "";
            foreach (var node in SkillTreeNode.AllNodes)
                upgradesString += $"{node.ID},{(int)node.State}{Separator}";
            
            return upgradesString;
        }

        void ApplySaveString(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            var recordedStates = new Dictionary<string, SkillTreeNode.NodeState>();
            var pairs = input.Split(Separator);
            foreach (var pair in pairs)
            {
                if (pair == "") continue;
                var comma = pair.IndexOf(',');
                var key = pair.Substring(0, comma);
                var num = pair.Substring(comma + 1, 1);
                recordedStates[key] = (SkillTreeNode.NodeState)int.Parse(num);
            }
            
            SkillTreeNode.AllNodes.Sort((node1, node2) => node1.Level.CompareTo(node2.Level));

            foreach (var node in SkillTreeNode.AllNodes)
                node.InitState(recordedStates[node.ID]);
        }
    }
}
