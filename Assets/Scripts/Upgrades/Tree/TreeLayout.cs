using System;
using System.Collections.Generic;
using UI.UIShapeHelper;
using UnityEngine;
using UnityEngine.UI;

namespace Upgrades.Tree
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
        readonly List<TreeNode> treeNodes = new();
        
        SkillTreeNode[] parentCache;
        Vector3[] positionCache;
        Vector3 oldPosition;
        SkillTreeNode.NodeState[] stateCache;

        [ContextMenu("Manual Update")]
        void ManualUpdate()
        {
            ReinitNodes();
            SetAllDirty();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            ManualUpdate();
        }

        void Update()
        {
            var setDirty = false;
            
            if (nodes == null || nodes.Length != transform.childCount || nodes.Length != treeNodes.Count)
            {
                ReinitNodes();
                setDirty = true;
            }

            if (oldPosition != transform.position)
            {
                oldPosition = transform.position;
                setDirty = true;
            }

            for (var i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].Parent != parentCache[i])
                {
                    ReinitNodes();
                    setDirty = true;
                    break;
                }
                
                if (nodes[i].transform.position != positionCache[i]
                    || Application.isPlaying && nodes[i].State != stateCache[i])
                {
                    setDirty = true;
                    break;
                }
            }
            
            if (setDirty) SetAllDirty();
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
            if (nodes.Length == 0 || treeNodes.Count == 0) return;

            // Verify that all nodes still exist
            foreach (var node in treeNodes)
                if (!node.node)
                {
                    ReinitNodes();
                    break;
                }
            
            treeNodes[0].PositionHierarchy(transform.position.x);
            
            for (var i = 0; i < treeNodes.Count; i++) positionCache[i].x = treeNodes[i].rectTransform.position.x;
        }

        public void SetLayoutVertical()
        {
            if (nodes.Length == 0 || treeNodes.Count == 0) return;
            
            var maxDepth = 0;
            foreach (var node in nodes)
            {
                var depth = node.GetDepth();
                if (depth > maxDepth) maxDepth = depth;
            }

            for (var i = 1; i < nodes.Length; i++)
            {
                var node = nodes[i];
                var pos = node.transform.position;
                pos.y = nodes[0].transform.position.y - node.GetDepth() * rowHeight;
                node.transform.position = pos;
                positionCache[i].y = pos.y;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (nodes.Length == 0 || treeNodes.Count == 0) return;
            
            vh.Clear();

            for (var i = 0; i < nodes.Length; i++)
            {
                var node = treeNodes[i];
                stateCache[i] = node.State;
                parentCache[i] = nodes[i].Parent;

                if (node.children.Count > 0) DrawLinesToChildren(node, vh);
            }
        }

        void ReinitNodes()
        {
            nodes = GetComponentsInChildren<SkillTreeNode>();
            parentCache = new SkillTreeNode[nodes.Length];
            positionCache = new Vector3[nodes.Length];
            stateCache = new SkillTreeNode.NodeState[nodes.Length];
            treeNodes.Clear();
            foreach (var node in nodes) treeNodes.Add(new TreeNode(node, treeNodes, horizontalNodeSpace));
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
                var isActive = child.State == SkillTreeNode.NodeState.Active;
                var childPos = child.Position - transform.position;
                var isMiddleNode = Mathf.Abs(i - (node.children.Count - 1) / 2f) < 0.1f;

                if (isMiddleNode && isActive)
                {
                    vh.DrawLine(childPos, parentPos, 0, thickness, StateToColor(child.State));
                    continue;
                }
                
                var childBranch = childPos + midHeightDelta;

                var factor = child.Position.x == node.Position.x ? 0
                    : child.Position.x > node.Position.x ? 1 : -1;

                var end = childBranch + cornerOffset;
                if (!squareCorners && !isActive && i > 0 && i < node.children.Count - 1) end -= Vector3.up * thickness / 2;
                vh.DrawLine(childPos, end, 0, thickness, StateToColor(child.State));
                
                if (child.State != SkillTreeNode.NodeState.Active) continue;
                
                if (!squareCorners) vh.DrawCaps();

                vh.DrawLine(childBranch + sideOffset * factor, parentBranch - sideOffset * factor, 90, thickness, activeColor);
                if (!squareCorners) vh.DrawCaps();
                vh.DrawLine(parentBranch - cornerOffset, parentPos, 0, thickness, StateToColor(greatestState));
            }
        }
    }
}
