using UnityEngine;

namespace Products.Browser
{
    public static class FindExtensions
    {
        public static Transform FindRecursive(this Transform parent, string childName) =>
            parent.Find(parent.FindPathRecursive(childName));

        /// <summary>
        /// Finds the child of the specified name and gives the path that would give that child if used in a Find query on the parent.
        /// </summary>
        /// <param name="parent">The parent transform with the hierarchy to search.</param>
        /// <param name="childName">The name to look for.</param>
        /// <returns>The path to the child from the parent. Is empty if no matching child is found.</returns>
        public static string FindPathRecursive(this Transform parent, string childName)
        {
            var basePath = "";
            return parent.FindBasePathRecursive(childName, ref basePath)
                ? basePath + childName
                : "";
        }
        
        static bool FindBasePathRecursive(this Transform parent, string childName, ref string path)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName) return true;
                
                if (child.FindBasePathRecursive(childName, ref path))
                {
                    // Find results propagate upwards from the child (i.e., in backwards order)
                    // so add parent names to the back of the path
                    path = child.name + '/' + path;
                    return true;
                }
            }
            
            path = null;
            return false;
        }
    }
}
