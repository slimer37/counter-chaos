using UnityEngine;
using UnityEditor;

namespace Checkout.Editor
{
    [CustomEditor(typeof(Queue))]
    public class QueueEditor : UnityEditor.Editor
    {
        static Mesh humanMesh;
        static Vector3[] positions;
        
        [InitializeOnLoadMethod]
        static void Init() =>
            humanMesh = AssetDatabase.LoadAssetAtPath<Transform>("Assets/Models/human.fbx").GetChild(1)
                .GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;

        [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
        static void DrawGizmos(Queue queue, GizmoType type)
        {
            var offset = Vector3.up * 0.6f;
            var so = new SerializedObject(queue);
            if (!Application.isPlaying)
            {
                var limit = so.FindProperty("limit").intValue;
                var spacing = so.FindProperty("spotSpacing").floatValue;

                var temp = Random.state;
                Random.InitState(0);
                positions = QueuePositioning.GenerateQueue(
                    queue.transform.position + offset, queue.transform.forward, spacing, limit);
                Random.state = temp;
            }
            else
                positions = queue.LineSpots;

            for (var index = 0; index < positions.Length; index++)
            {
                var pos = positions[index];
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(pos, 0.1f);
                Gizmos.color = new Color(1, 0.4f, 0, 0.8f);

                var rotation = index > 0 ? Quaternion.LookRotation(positions[index - 1] - pos)
                    : Quaternion.Euler(Vector3.up * queue.transform.eulerAngles.y);
                Gizmos.DrawMesh(humanMesh, pos - offset, rotation);
            }
        }
    }
}
