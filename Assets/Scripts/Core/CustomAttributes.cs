using System;
using UnityEngine;

namespace Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public class LayerAttribute : PropertyAttribute { }

    [AttributeUsage(AttributeTargets.Field)]
    public class RequireSubstringAttribute : PropertyAttribute
    {
        public readonly string[] substrings;
        public readonly bool textArea;
        public readonly int lines = 3;
        
        public RequireSubstringAttribute(params string[] substrings) => this.substrings = substrings;

        public RequireSubstringAttribute(bool textArea, params string[] substrings)
        {
            this.textArea = textArea;
            this.substrings = substrings;
        }
        
        public RequireSubstringAttribute(bool textArea, int lines, params string[] substrings)
        {
            this.textArea = textArea;
            this.lines = lines;
            this.substrings = substrings;
        }
    }
}
