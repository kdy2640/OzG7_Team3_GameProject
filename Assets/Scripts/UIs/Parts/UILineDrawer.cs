using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UILineDrawer : Graphic
{
    [SerializeField] private float lineThickness = 4f;
    private List<Vector2> points = new List<Vector2>();

    public void SetPoints(List<Vector2> newPoints)
    {
        points = newPoints;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (points == null || points.Count < 2) return;

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 start = points[i];
            Vector2 end = points[i + 1];

            Vector2 dir = (end - start).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x) * (lineThickness * 0.5f);

            UIVertex[] verts = new UIVertex[4];
            verts[0].position = start - normal;
            verts[1].position = start + normal;
            verts[2].position = end + normal;
            verts[3].position = end - normal;

            for (int j = 0; j < 4; j++)
            {
                verts[j].color = color;
            }

            vh.AddUIVertexQuad(verts);
        }
    }
}