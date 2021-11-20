using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.UIShapeHelper;

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
        Vector2Int step;

        void Update()
        {
            if (!grid) return;
            if (gridSize != grid.GridSize || step != grid.Step)
            {
                gridSize = grid.GridSize;
                step = grid.Step;
                SetVerticesDirty();
            }
        }

        Vector2 GridToWorld(Vector2 point) =>
            new Vector2(grid.CellWidth * point.x / step.x, grid.CellHeight * point.y / step.y)
            - new Vector2(grid.Width, grid.Height) / 2;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (points.Count < 2 || progress < 0.001f) return;
            
            var limit = progress * grid.GridSize.x * step.x;
            for (var i = 1; i < points.Count; i++)
            {
                var pt = points[i];
                var prev = points[i - 1];
                
                var lineColor = prev.y == pt.y || !colorByDifference ? color :
                    pt.y - prev.y > 0 ? positiveColor : negativeColor;

                if (points[i].x >= limit)
                {
                    var s = (pt.y - prev.y) / (pt.x - prev.x);
                    pt.x = limit;
                    pt.y = prev.y + (limit - prev.x) * s;
                    DrawLine();
                    break;
                }
                
                DrawLine();

                if (i == points.Count - 1) break;
                
                // Draw caps.
                var index = i * 4 - 2;
                vh.AddTriangle(index + 3, index + 2, index);
                vh.AddTriangle(index, index + 1, index + 3);
                
                void DrawLine() => vh.DrawLine(GridToWorld(prev), GridToWorld(pt), thickness, lineColor);
            }
        }
    }
}
