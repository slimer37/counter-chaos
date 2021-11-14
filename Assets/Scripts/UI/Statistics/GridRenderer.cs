using UnityEngine;
using UnityEngine.UI;

namespace UI.Statistics
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class GridRenderer : Graphic
    {
        [field: SerializeField, Min(0.01f)] public float Thickness { get; private set; } = 5;
        [field: SerializeField, Min(0.01f)] public Vector2Int GridSize { get; private set; } = new(1, 1);

        [Header("Border")]
        [SerializeField, Min(0)] float borderThickness = 5;
        [SerializeField, Min(0)] float tickThickness = 5;
        [SerializeField, Min(0)] float tickHeight = 40;
        [SerializeField] Color borderColor = Color.white;

        VertexHelper vh;

        public float Width { get; private set; }
        public float Height { get; private set; }
        public float CellWidth { get; private set; }
        public float CellHeight { get; private set; }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            this.vh = vh;

            Width = rectTransform.rect.width;
            Height = rectTransform.rect.height;

            CellWidth = Width / GridSize.x;
            CellHeight = Height / GridSize.y;

            var index = 0;

            for (var x = 0; x < GridSize.x; x++)
            {
                for (var y = 0; y < GridSize.y; y++)
                {
                    DrawCell(x, y, index);
                    index++;
                }
            }
            
            var gridHeight = GridSize.y * CellHeight;
            var gridWidth = GridSize.x * CellWidth;
            DrawRect(Vector3.zero, borderColor, gridWidth, borderThickness);
            DrawRect(Vector3.zero, borderColor, borderThickness, gridHeight);

            for (var x = 1; x < GridSize.x; x++)
            {
                var pos = new Vector3(x * CellWidth - tickThickness / 2, borderThickness);
                DrawRect(pos, borderColor, tickThickness, tickHeight);
            }
            for (var y = 1; y < GridSize.y; y++)
            {
                var pos = new Vector3(borderThickness, y * CellHeight - tickThickness / 2);
                DrawRect(pos, borderColor, tickHeight, tickThickness);
            }
        }

        void DrawRect(Vector3 origin, Color rectColor, float width, float height)
        {
            var vertex = UIVertex.simpleVert;
            vertex.color = rectColor;
            var i = vh.currentVertCount;
            
            CreateRect(origin, width, height, vertex);
            
            vh.AddTriangle(i, i + 1, i + 2);
            vh.AddTriangle(i + 2, i + 3, i);
        }

        void DrawCell(int x, int y, int index)
        {
            var xPos = CellWidth * x;
            var yPos = CellHeight * y;

            var vertex = UIVertex.simpleVert;
            vertex.color = color;

            CreateRect(new Vector3(xPos, yPos), CellWidth, CellHeight, vertex);

            CreateRect(new Vector3(xPos + Thickness, yPos + Thickness), CellWidth - Thickness * 2, CellHeight - Thickness * 2, vertex);

            var offset = index * 8;
            AddEdge(offset, offset + 1, offset + 5, offset + 4);
            AddEdge(offset + 1, offset + 2, offset + 6, offset + 5);
            AddEdge(offset + 2, offset + 3, offset + 7, offset + 6);
            AddEdge(offset + 3, offset, offset + 4, offset + 7);

            void AddEdge(params int[] verts)
            {
                vh.AddTriangle(verts[0], verts[1], verts[2]);
                vh.AddTriangle(verts[2], verts[3], verts[0]);
            }
        }

        void CreateRect(Vector3 origin, float width, float height, UIVertex vertex)
        {
            origin -= new Vector3(rectTransform.rect.width, rectTransform.rect.height) / 2;
            
            vertex.position = origin;
            vh.AddVert(vertex);
            vertex.position = origin + new Vector3(0, height);
            vh.AddVert(vertex);
            vertex.position = origin + new Vector3(width, height);
            vh.AddVert(vertex);
            vertex.position = origin + new Vector3(width, 0);
            vh.AddVert(vertex);
        }
    }
}
