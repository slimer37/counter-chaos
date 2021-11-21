using System;
using System.Collections.Generic;
using System.Linq;
using UI.UIShapeHelper;
using UnityEngine;
using UnityEngine.UI;

namespace Upgrades
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class TreeLayout : Graphic
    {
        [Header("Node Layout")]
        [SerializeField] float topRowHeight;
        [SerializeField] float rowHeight = 200;
        
        [Header("Connection Lines")]
        [SerializeField] Color unlockedColor;
        [SerializeField] Color inactiveColor;
        [SerializeField] Color activeColor;
        [SerializeField, Min(0)] float thickness = 5;
        [SerializeField] bool squareCorners;
        
        SkillTreeNode[] nodes;
        Vector3[] positionCache;
        SkillTreeNode.NodeState[] stateCache;

        readonly List<TreeNode> drawnDownBranches = new();

        void Update()
        {
            if (nodes == null)
            {
                SetVerticesDirty();
                return;
            }

            for (var i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].transform.position != positionCache[i]
                || Application.isPlaying && nodes[i].State != stateCache[i])
                {
                    SetVerticesDirty();
                    return;
                }
            }
        }
        
        Color StateToColor(SkillTreeNode.NodeState state) => state switch
        {
            SkillTreeNode.NodeState.Active => activeColor,
            SkillTreeNode.NodeState.Locked => inactiveColor,
            SkillTreeNode.NodeState.Unlocked => unlockedColor,
            _ => throw new ArgumentException("Invalid node state.", nameof(state))
        };

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            TreeNode.AllTreeNodes.Clear();
            vh.Clear();

            nodes ??= GetComponentsInChildren<SkillTreeNode>();
            foreach (var node in nodes)
                if (!node)
                    nodes = GetComponentsInChildren<SkillTreeNode>();
            
            positionCache = new Vector3[nodes.Length];
            stateCache = new SkillTreeNode.NodeState[nodes.Length];

            drawnDownBranches.Clear();

            foreach (var node in nodes) TreeNode.Create(node);
            
            for (var i = 0; i < nodes.Length; i++)
            {
                var nodeT = nodes[i].transform;
                var pos = nodeT.position;
                pos.y = rectTransform.rect.yMax + transform.position.y - topRowHeight - nodes[i].GetDepth() * rowHeight;
                nodeT.position = pos;
                
                var node = TreeNode.AllTreeNodes[i];
                positionCache[i] = node.Position;
                stateCache[i] = node.State;
                
                if (node.parent != null) DrawLineToParent(node, vh);
            }
        }

        void DrawLineToParent(TreeNode node, VertexHelper vh)
        {
            var parentPos = node.parent.Position - transform.position;
            var nodePos = node.Position - transform.position;
            var midHeightDelta = Vector3.up * (parentPos.y - nodePos.y) / 2;
            var nodeBranch = nodePos + midHeightDelta;
            var parentBranch = parentPos - midHeightDelta;

            var cornerOffset = Vector3.up * (squareCorners ? thickness / 2 : 0);
            var sideOffset = Vector3.right * (squareCorners ? thickness / 2 : 0);
            var nodeIsToRight = nodePos.x > parentPos.x;
                
            vh.DrawLine(nodePos, nodeBranch + cornerOffset,
                0, thickness, StateToColor(node.State));
            if (!squareCorners) vh.DrawCaps();
            vh.DrawLine(nodeBranch, parentBranch + sideOffset * (nodeIsToRight ? 1 : -1),
                90, thickness, StateToColor(node.State));

            if (drawnDownBranches.Contains(node.parent)) return;
                
            if (!squareCorners) vh.DrawCaps();

            vh.DrawLine(parentBranch - cornerOffset, parentPos, 0, thickness,
                StateToColor(node.parent.GreatestChildState()));
                
            drawnDownBranches.Add(node.parent);
        }

        class TreeNode
        {
            public readonly SkillTreeNode node;
            public readonly TreeNode parent;
            public readonly List<TreeNode> children = new();
            public Vector3 Position => node.transform.position;
            public SkillTreeNode.NodeState State => node.State;

            public static readonly List<TreeNode> AllTreeNodes = new();

            public SkillTreeNode.NodeState GreatestChildState()
            {
                var max = Enum.GetValues(typeof(SkillTreeNode.NodeState)).Cast<int>().Max();
                var greatest = -1;
                foreach (var child in children)
                {
                    if (greatest == max) break;
                    
                    var state = (int)child.State;
                    
                    if (greatest == -1)
                    {
                        greatest = state;
                        continue;
                    }

                    if (state > greatest)
                        greatest = state;
                }

                return (SkillTreeNode.NodeState)greatest;
            }

            public static void Create(SkillTreeNode node) => AllTreeNodes.Add(new TreeNode(node));

            TreeNode(SkillTreeNode node)
            {
                this.node = node;
                
                if (node.Parent)
                {
                    parent = AllTreeNodes.Find(n => n.node == node.Parent)
                             ?? new TreeNode(node.Parent);
                    parent.children.Add(this);
                }
            }
        }
    }
}
