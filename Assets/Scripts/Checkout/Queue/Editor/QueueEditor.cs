using UnityEngine;
using UnityEditor;

namespace Checkout.Editor
{
    [CustomEditor(typeof(Queue))]
    public class QueueEditor : UnityEditor.Editor
    {
        static Mesh humanMesh;
        static Vector3[] positions;
        
        [DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.InSelectionHierarchy)]
        static void DrawGizmos(Queue queue, GizmoType type)
        {
            humanMesh ??= AssetDatabase.LoadAssetAtPath<Transform>("Assets/Models/human.fbx").GetChild(2)
                .GetComponent<SkinnedMeshRenderer>().sharedMesh;
            
            var so = new SerializedObject(queue);
            DrawItemArea(so, queue);
            DrawCustomerQueue(so, queue);
        }

        static void DrawItemArea(SerializedObject so, Queue queue)
        {
            var cornerTransform = so.FindProperty("itemAreaCorner").objectReferenceValue;
            if (!cornerTransform) return;
            var corner = ((Transform)cornerTransform).position;
            var size = so.FindProperty("itemAreaDimensions").vector2IntValue;
            var flat = new Vector3(1, 0, 1) * ItemArea.UnitSize;
            
            var displayFilled = Application.isPlaying;
            
            for (var x = 0; x < size.x; x++)
            for (var y = 0; y < size.y; y++)
            {
                var center = corner + new Vector3(x, 0, y) * ItemArea.UnitSize;
                var filled = displayFilled && queue.Area[x, y];
                Gizmos.color = filled ? Color.yellow : Color.green;
                if (filled)
                    Gizmos.DrawCube(center, flat);
                else
                    Gizmos.DrawWireCube(center, flat);
            }
        }

        static void DrawCustomerQueue(SerializedObject so, Queue queue)
        {
            if (!Application.isPlaying)
            {
                var limit = so.FindProperty("limit").intValue;
                var spacing = so.FindProperty("spotSpacing").floatValue;

                var temp = Random.state;
                Random.InitState(0);
                positions = QueuePositioning.GenerateQueue(
                    queue.transform.position, queue.transform.forward, spacing, limit);
                Random.state = temp;
            }
            else
                positions = queue.LineSpots;

            Gizmos.color = new Color(1, 0.4f, 0, 0.8f);
            for (var index = 0; index < positions.Length; index++)
            {
                var pos = positions[index];
                var rotation = index > 0 ? Quaternion.LookRotation(positions[index - 1] - pos)
                    : Quaternion.Euler(Vector3.up * queue.transform.eulerAngles.y);
                Gizmos.DrawMesh(humanMesh, pos, rotation);
            }
        }
    }
}
