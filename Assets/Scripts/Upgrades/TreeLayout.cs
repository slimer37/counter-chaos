using System;
using System.Collections.Generic;
using System.Linq;
using UI.UIShapeHelper;
using UnityEngine;
using UnityEngine.UI;

namespace Upgrades
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class TreeLayout : Graphic, ILayoutGroup
    {
        [Header("Node Layout")]
        [SerializeField] float rowHeight = 200;
        [SerializeField] float horizontalNodeSpace;
        
        [Header("Connection Lines")]
        [SerializeField] Color unlockedColor;
        [SerializeField] Color inactiveColor;
        [SerializeField] Color activeColor;
        [SerializeField, Min(0)] float thickness = 5;
        [SerializeField] bool squareCorners;
        
        SkillTreeNode[] nodes;
        Vector3[] positionCache;
        Vector3 oldPosition;
        SkillTreeNode.NodeState[] stateCache;

        [ContextMenu("Manual Update")]
        void ManualUpdate()
        {
            nodes = GetComponentsInChildren<SkillTreeNode>();
            SetAllDirty();
        }

        void Update()
        {
            if (nodes == null || nodes.Length != transform.childCount)
            {
                nodes = GetComponentsInChildren<SkillTreeNode>();
                positionCache = new Vector3[nodes.Length];
                stateCache = new SkillTreeNode.NodeState[nodes.Length];
                SetAllDirty();
                return;
            }

            if (oldPosition != transform.position)
            {
                oldPosition = transform.position;
                SetAllDirty();
                return;
            }

            for (var i = 0; i < nodes.Length; i++)
            {
                // Lock positions
                if (nodes[i].transform.position != positionCache[i])
                    nodes[i].transform.position = positionCache[i];
                
                if (Application.isPlaying && nodes[i].State != stateCache[i])
                {
                    SetAllDirty();
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

        public void SetLayoutHorizontal()
        {
            for (var i = 0; i < TreeNode.AllTreeNodes.Count; i++)
            {
                var node = TreeNode.AllTreeNodes[i];
                if (node.parent == null)
                {
                    var pos = node.node.transform.position;
                    pos.x = transform.position.x;
                    node.node.transform.position = pos;
                    node.PositionHierarchy(horizontalNodeSpace);
                }
                positionCache[i].x = node.node.transform.position.x;
            }
        }

        public void SetLayoutVertical()
        {
            if (nodes == null) return;
            
            var maxDepth = 0;
            foreach (var node in nodes)
            {
                var depth = node.GetDepth();
                if (depth > maxDepth) maxDepth = depth;
            }

            for (var i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                var pos = node.transform.position;
                pos.y = maxDepth * rowHeight / 2 + transform.position.y - node.GetDepth() * rowHeight;
                node.transform.position = pos;
                positionCache[i].y = pos.y;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            TreeNode.AllTreeNodes.Clear();
            vh.Clear();

            foreach (var node in nodes)
                if (!node)
                    nodes = GetComponentsInChildren<SkillTreeNode>();

            foreach (var node in nodes) TreeNode.Create(node);
            
            for (var i = 0; i < nodes.Length; i++)
            {
                var node = TreeNode.AllTreeNodes[i];
                stateCache[i] = node.State;

                if (node.children.Count > 0) DrawLinesToChildren(node, vh);
            }
        }

        void DrawLinesToChildren(TreeNode node, VertexHelper vh)
        {
            if (node.children.Count == 1)
            {
                vh.DrawLine(node.children[0].Position - transform.position, node.Position - transform.position,
                    0, thickness, StateToColor(node.children[0].State));
                return;
            }
            
            var parentPos = node.node.transform.position - transform.position;
            var midHeightDelta = Vector3.up * rowHeight / 2;
            var cornerOffset = Vector3.up * (squareCorners ? thickness / 2 : 0);
            var sideOffset = Vector3.right * (squareCorners ? thickness / 2 : 0);
            var parentBranch = parentPos - midHeightDelta;

            var greatestState = node.GreatestChildState();
            
            if (greatestState != SkillTreeNode.NodeState.Active)
                vh.DrawLine(parentPos, parentBranch, 0, thickness, StateToColor(greatestState));

            var firstChildPos = node.children[0].Position - transform.position;
            var lastChildPos = node.children[^1].Position - transform.position;
            var col = StateToColor((SkillTreeNode.NodeState)Mathf.Max((int)node.State - 1, 0));
            vh.DrawLine(firstChildPos, firstChildPos + midHeightDelta + cornerOffset, 0, thickness, col);
            if (!squareCorners) vh.DrawCaps();

            vh.DrawLine(firstChildPos + midHeightDelta - sideOffset,
                lastChildPos + midHeightDelta + sideOffset,
                90, thickness, col);
            
            if (!squareCorners) vh.DrawCaps();
            vh.DrawLine(lastChildPos + midHeightDelta + cornerOffset, lastChildPos, 0, thickness, col);

            for (var i = 0; i < node.children.Count; i++)
            {
                var child = node.children[i];
                var childPos = child.Position - transform.position;
                var isMiddleNode = Mathf.Abs(i - (node.children.Count - 1) / 2f) < 0.1f;

                if (isMiddleNode)
                {
                    vh.DrawLine(childPos, parentPos, 0, thickness, StateToColor(child.State));
                    continue;
                }
                
                var childBranch = childPos + midHeightDelta;

                var factor = child.Position.x == node.Position.x ? 0
                    : child.Position.x > node.Position.x ? 1 : -1;

                var end = childBranch + cornerOffset;
                vh.DrawLine(childPos, end, 0, thickness, StateToColor(child.State));
                
                if (child.State != SkillTreeNode.NodeState.Active) continue;
                
                if (!squareCorners) vh.DrawCaps();

                vh.DrawLine(childBranch + sideOffset * factor, parentBranch - sideOffset * factor, 90, thickness, activeColor);
                if (!squareCorners) vh.DrawCaps();
                vh.DrawLine(parentBranch - cornerOffset, parentPos, 0, thickness, StateToColor(greatestState));
            }
        }

        class TreeNode
        {
            public readonly SkillTreeNode node;
            public readonly TreeNode parent;
            public readonly List<TreeNode> children = new();
            public Vector3 Position => node.transform.position;
            public SkillTreeNode.NodeState State => node.State;

            public static readonly List<TreeNode> AllTreeNodes = new();

            readonly RectTransform rectTransform;

            public void PositionHierarchy(float horizontalNodeSpace)
            {
                if (parent != null) 
                    throw new InvalidOperationException($"Can only use {nameof(PositionHierarchy)} on root nodes.");
                
                var maxDepth = 0;
                foreach (var treeNode in AllTreeNodes)
                {
                    var depth = treeNode.node.GetDepth();
                    if (depth > maxDepth) maxDepth = depth;
                }

                for (var i = 0; i < maxDepth; i++) RecursivePositionHierarchy(horizontalNodeSpace);
            }

            float RecursivePositionHierarchy(float horizontalNodeSpace)
            {
                var totalWidth = children.Count * horizontalNodeSpace;
                
                for (var i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    var pos = child.rectTransform.position;
                    var nodeSpace = horizontalNodeSpace;
                    var factor = i - (children.Count - 1) / 2f;
                    pos.x = rectTransform.position.x + nodeSpace * factor;
                    if (child.children.Count > 0)
                    {
                        var offset = (child.RecursivePositionHierarchy(horizontalNodeSpace) - horizontalNodeSpace) / 2 * (factor < 0 ? -1 : 1);
                        if (children.Count > 1) pos.x += offset;
                    }
                    child.rectTransform.position = pos;
                }
                
                return totalWidth;
            }

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
                rectTransform = node.GetComponent<RectTransform>();
                
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
