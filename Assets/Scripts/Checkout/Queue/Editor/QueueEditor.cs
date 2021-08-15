using UnityEngine;
using UnityEditor;

namespace Checkout.Editor
{
    [CustomEditor(typeof(Queue))]
    public class QueueEditor : UnityEditor.Editor
    {
        static Mesh humanMesh;
        
        [InitializeOnLoadMethod]
        static void Init() =>
            humanMesh = AssetDatabase.LoadAssetAtPath<Transform>("Assets/Models/human.fbx").GetChild(1)
                .GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;

        [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
        static void DrawGizmos(Queue queue, GizmoType type)
        {
            Gizmos.color = new Color(1, 0.4f, 0, 0.8f);
            var limit = new SerializedObject(queue).FindProperty("limit").intValue;
            for (var i = limit; i > 0; i--)
                Gizmos.DrawMesh(humanMesh, queue.GetSpotInLine(i - 1),
                    Quaternion.Euler(Vector3.up * queue.transform.eulerAngles.y));
        }
    }
}
