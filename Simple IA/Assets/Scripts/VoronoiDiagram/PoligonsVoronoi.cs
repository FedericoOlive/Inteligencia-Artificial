using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoligonsVoronoi
{
    public bool drawPoli;
    public List<Segment> segments = new List<Segment>();

    public void DrawPoli (float distanceSegment, Color color)
    {
        if (drawPoli)
        {
            foreach (Segment segment in segments)
            {
                Gizmos.DrawRay(segment.Mediatrix, segment.Direction * distanceSegment);
                Gizmos.DrawRay(segment.Mediatrix, -segment.Direction * distanceSegment);
            }
        }
    }
}