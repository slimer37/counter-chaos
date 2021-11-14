using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Statistics
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class LineGraph : Graphic
    {
        [SerializeField] bool colorByDifference;
        [SerializeField] Color positiveColor = Color.green;
        [SerializeField] Color negativeColor = Color.red;
        [SerializeField] GridRenderer grid;
        [SerializeField, Min(0.01f)] float thickness = 1;
        
        [Range(0, 1)] public float progress = 1;

        public List<Vector2> points = new();

        Vector2Int gridSize;

        void Update()
        {
            if (!grid) return;
            if (gridSize != grid.GridSize)
            {
                gridSize = grid.GridSize;
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (points.Count < 2 || progress < 0.001f) return;
            
            var limit = progress * grid.GridSize.x;
            for (var i = 1; i < points.Count; i++)
            {
                if (points[i].x >= limit)
                {
                    var pt = points[i];
                    var prev = points[i - 1];
                    var s = (pt.y - prev.y) / (pt.x - prev.x);
                    pt.x = limit;
                    pt.y = prev.y + (limit - prev.x) * s;
                    DrawLine(prev, pt, i - 1, vh);
                    break;
                }
                
                DrawLine(points[i - 1], points[i], i - 1, vh);

                if (i == points.Count - 1) break;
                
                // Draw caps.
                var index = i * 4 - 2;
                vh.AddTriangle(index + 3, index + 2, index);
                vh.AddTriangle(index, index + 1, index + 3);
            }
        }

        float GetAngle(Vector2 me, Vector2 target) => Mathf.Atan2(target.y - me.y, target.x - me.x) * 180 / Mathf.PI;

        void DrawLine(Vector2 a, Vector2 b, int index, VertexHelper vh)
        {
            var vertex = UIVertex.simpleVert;

            vertex.color = a.y == b.y || !colorByDifference ? color :
                b.y - a.y > 0 ? positiveColor : negativeColor;

            DrawVerticesAtPoint(a, GetAngle(a, b) + 90);
            DrawVerticesAtPoint(b, GetAngle(a, b) + 90);

            index *= 4;
            vh.AddTriangle(index, index + 1, index + 3);
            vh.AddTriangle(index + 3, index + 2, index);

            void DrawVerticesAtPoint(Vector2 point, float angle)
            {
                vertex.position = new Vector3(grid.CellWidth * point.x, grid.CellHeight * point.y);
                vertex.position -= new Vector3(grid.Width, grid.Height) / 2;
                vertex.position -= Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
                vh.AddVert(vertex);

                vertex.position += Quaternion.Euler(0, 0, angle) * new Vector3(thickness, 0);
                vh.AddVert(vertex);
            }
        }
    }
}
