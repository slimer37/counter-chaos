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
                .GetComponent<SkinnedMeshRenderer>().sharedMesh;

        [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
        static void DrawGizmos(Queue queue, GizmoType type)
        {
            var so = new SerializedObject(queue);
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
