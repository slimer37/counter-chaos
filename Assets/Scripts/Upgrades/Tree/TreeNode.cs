using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Upgrades.Tree
{
    internal class TreeNode
    { 
        public readonly SkillTreeNode node;
        public readonly TreeNode parent;
        public readonly List<TreeNode> children = new();
        public Vector3 Position => node.transform.position;
        public SkillTreeNode.NodeState State => node.State;

        public readonly RectTransform rectTransform;
        
        readonly int localX;
        readonly int depth;
        readonly float horizontalNodeSpace;

        float mod;

        float X
        {
            get => rectTransform.position.x;
            set
            {
                var pos = rectTransform.position;
                pos.x = value;
                rectTransform.position = pos;
            }
        }

        public TreeNode(SkillTreeNode node, List<TreeNode> allNodes, float horizontalSpace)
        {
            horizontalNodeSpace = horizontalSpace;
            depth = node.GetDepth();
            this.node = node;
            rectTransform = node.GetComponent<RectTransform>();
            
            if (node.Parent && node.Parent != node)
            {
                parent = allNodes.Find(n => n.node == node.Parent)
                         ?? new TreeNode(node.Parent, allNodes, horizontalSpace);
                localX = parent.children.Count;
                parent.children.Add(this);
            }
        }
        
        void RecursiveGetChildren(ref List<TreeNode> all)
        {
            foreach (var child in children)
                child.RecursiveGetChildren(ref all);
            all.Add(this);
        }
        
        public void PositionHierarchy(float rootCenter)
        {
            if (parent != null) 
                throw new InvalidOperationException($"Can only use {nameof(PositionHierarchy)} on root nodes.");

            var allNodes = new List<TreeNode>();
            RecursiveGetChildren(ref allNodes);
            
            CalculateInitialX();
            
            CalculateFinalX(mod);

            var offset = rootCenter - X;
            foreach (var n in allNodes) n.X += offset;
        }

        TreeNode GetPreviousSibling() => parent.children[localX - 1];
        TreeNode GetNextSibling() => parent.children[localX + 1];

        void CalculateInitialX()
        {
            foreach (var child in children)
                child.CalculateInitialX();
 
            if (children.Count == 0)
            {
                if (localX > 0)
                    X = GetPreviousSibling().X + horizontalNodeSpace;
                else
                    X = 0;
            }
            else if (children.Count == 1)
            {
                // if this is the first node in a set, set it's X value equal to it's child's X value
                if (localX == 0)
                    X = children[0].X;
                else
                {
                    X = GetPreviousSibling().X + horizontalNodeSpace;
                    mod = X - children[0].X;
                } 
            }
            else
            {
                var mid = (children[0].X + children[^1].X) / 2;
 
                if (localX == 0)
                    X = mid;
                else
                {
                    X = GetPreviousSibling().X + horizontalNodeSpace;
                    mod = X - mid;
                }
            }
            
            if (children.Count > 0 && localX > 0) CheckForConflicts();
        }
        
        void CheckForConflicts()
        {
            var shiftValue = 0F;
 
            var nodeContour = new Dictionary<int, float>();
            GetLeftContour(0, ref nodeContour);
 
            var sibling = parent.children[0];
            while (sibling != null && sibling != this)
            {
                var siblingContour = new Dictionary<int, float>();
                sibling.GetRightContour(0, ref siblingContour);
 
                for (var level = depth + 1; level <= Math.Min(siblingContour.Keys.Max(), nodeContour.Keys.Max()); level++)
                {
                    var distance = nodeContour[level] - siblingContour[level];
                    if (distance + shiftValue < horizontalNodeSpace)
                    {
                        shiftValue = horizontalNodeSpace - distance;
                    }
                }
 
                if (shiftValue > 0)
                {
                    X += shiftValue;
                    mod += shiftValue;

                    CenterNodesBetween(this, sibling);

                    shiftValue = 0;
                }
 
                sibling = sibling.GetNextSibling();
            }
        }
        
        void GetLeftContour(float modSum, ref Dictionary<int, float> values)
        {
            if (!values.ContainsKey(node.GetDepth()))
                values.Add(depth, X + modSum);
            else
                values[depth] = Math.Min(values[depth], X + modSum);
 
            modSum += mod;
            foreach (var child in children)
                child.GetLeftContour(modSum, ref values);
        }
        
        void GetRightContour(float modSum, ref Dictionary<int, float> values)
        {
            if (!values.ContainsKey(depth))
                values.Add(depth, X + modSum);
            else
                values[depth] = Math.Max(values[depth], X + modSum);
 
            modSum += mod;
            foreach (var child in children)
                child.GetRightContour(modSum, ref values);
        }
        
        void CalculateFinalX(float modSum)
        {
            X += modSum;
            modSum += mod;
 
            foreach (var child in children)
                child.CalculateFinalX(modSum);
        }
        
        void CenterNodesBetween(TreeNode leftNode, TreeNode rightNode)
        {
            var leftIndex = leftNode.parent.children.IndexOf(rightNode);
            var rightIndex = leftNode.parent.children.IndexOf(leftNode);
            
            var numNodesBetween = rightIndex - leftIndex - 1;

            if (numNodesBetween > 0)
            {
                var distanceBetweenNodes = (leftNode.X - rightNode.X) / (numNodesBetween + 1);

                var count = 1;
                for (var i = leftIndex + 1; i < rightIndex; i++)
                {
                    var middleNode = leftNode.parent.children[i];

                    var desiredX = rightNode.X + (distanceBetweenNodes * count);
                    var offset = desiredX - middleNode.X;
                    middleNode.X += offset;
                    middleNode.mod += offset;

                    count++;
                }

                leftNode.CheckForConflicts();
            }
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
    }
}
