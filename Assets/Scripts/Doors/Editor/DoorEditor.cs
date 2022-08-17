using UnityEditor;
using UnityEngine;

namespace Doors
{
    [CustomEditor(typeof(Door))]
    public class DoorEditor : Editor
    {
        void OnSceneGUI()
        {
            if (target is not Door door) return;
            
            var so = new SerializedObject(door);
            var t = door.transform;

            var min = so.FindProperty("rotationMin").floatValue;
            var max = so.FindProperty("rotationMax").floatValue;
            var inv = so.FindProperty("invert").boolValue;

            var closed = min - t.localEulerAngles.y + (inv ? 180 : 0);
            var from = Quaternion.AngleAxis(closed, Vector3.up) * -t.right;

            Handles.color = new Color(0, 0, 1, 0.5f);
            
            Handles.DrawSolidArc(t.position, t.up, from, max - min, 1);
        }
    }
}