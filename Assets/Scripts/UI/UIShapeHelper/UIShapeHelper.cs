using UnityEngine;
using UnityEngine.UI;

namespace UI.UIShapeHelper
{
    public static class UIShapeHelper
    {
        public static void DrawRect(this VertexHelper vh, Vector3 corner, Color rectColor, float width, float height)
        {
            var vertex = UIVertex.simpleVert;
            vertex.color = rectColor;
            var i = vh.currentVertCount;
            
            vh.CreateRect(corner, width, height, vertex);
            vh.AddTriangle(i, i + 1, i + 2);
            vh.AddTriangle(i + 2, i + 3, i);
        }
        
        public static void CreateRect(this VertexHelper vh, Vector3 corner, float width, float height, Color color)
        {
            var vertex = UIVertex.simpleVert;
            vh.CreateRect(corner, width, height, vertex);
        }
    
        public static void CreateRect(this VertexHelper vh, Vector3 corner, float width, float height, UIVertex vertex)
        {
            vertex.position = corner;
            vh.AddVert(vertex);
            vertex.position = corner + new Vector3(0, height);
            vh.AddVert(vertex);
            vertex.position = corner + new Vector3(width, height);
            vh.AddVert(vertex);
            vertex.position = corner + new Vector3(width, 0);
            vh.AddVert(vertex);
        }
    
        public static float GetAngle(Vector2 me, Vector2 target) => Mathf.Atan2(target.y - me.y, target.x - me.x) * 180 / Mathf.PI + 45;

        public static void DrawCaps(this VertexHelper vh)
        {
            var index = vh.currentVertCount - 2;
            vh.AddTriangle(index + 3, index + 2, index);
            vh.AddTriangle(index, index + 1, index + 3);
        }

        public static void DrawLine(this VertexHelper vh, Vector2 a, Vector2 b, float thickness, Color color) =>
            vh.DrawLine(a, b, GetAngle(a, b), thickness, color);

        public static void DrawLine(this VertexHelper vh, Vector2 a, Vector2 b, float angle, float thickness, Color color)
        {
            var vertex = UIVertex.simpleVert;
            vertex.color = color;

            var index = vh.currentVertCount;
        
            DrawVerticesAtPoint(a, angle);
            DrawVerticesAtPoint(b, angle);

            vh.AddTriangle(index, index + 1, index + 3);
            vh.AddTriangle(index + 3, index + 2, index);

            void DrawVerticesAtPoint(Vector2 point, float angle)
            {
                vertex.position = point;
                vertex.position -= Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
                vh.AddVert(vertex);

                vertex.position += Quaternion.Euler(0, 0, angle) * new Vector3(thickness, 0);
                vh.AddVert(vertex);
            }
        }
    }
}
