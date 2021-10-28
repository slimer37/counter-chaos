using UnityEditor;
using UnityEngine;

namespace Checkout
{
    [CustomEditor(typeof(ItemArea))]
    public class ItemAreaEditor : Editor
    {
        [DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.InSelectionHierarchy)]
        static void DrawGizmos(ItemArea area, GizmoType type)
        {
            var flat = new Vector3(1, 0, 1) * ItemArea.UnitSize;
            var displayFilled = Application.isPlaying;
            
            for (var x = 0; x < area.Width; x++)
            for (var y = 0; y < area.Length; y++)
            {
                var center = area.transform.position + area.transform.rotation * new Vector3(x, 0, y) * ItemArea.UnitSize;
                var filled = displayFilled && area[x, y];
                Gizmos.color = filled ? Color.yellow : Color.green;
                if (filled)
                    Gizmos.DrawCube(center, flat);
                else
                    Gizmos.DrawWireCube(center, flat);
            }
        }
    }
}
