using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoligonsVoronoi
{
    public bool drawPoli;
    [SerializeField] private List<Segment> segments = new List<Segment>();
    [SerializeField] private List<Vector2> intersections = new List<Vector2>();

    public List<Segment> Segments => segments;
    public void SortSegment () => segments.Sort((p1, p2) => p1.distance.CompareTo(p2.distance));

    public void AddSegment (Segment refSegment)
    {
        Segment segment = new Segment(refSegment.Origin, refSegment.Final);
        segments.Add(segment);
    }

    public void SetIntersections ()
    {
        SortSegment();
        intersections.Clear();

        for (int i = 0; i < segments.Count; i++)
        {
            for (int j = i + 1; j < segments.Count - 1; j++)
            {
                if (segments[i].id == segments[j].id)
                    continue;

                segments[i].GetTwoPoints(out Vector2 p1, out Vector2 p2);
                segments[j].GetTwoPoints(out Vector2 p3, out Vector2 p4);
                Vector2 centerCircle = Segment.Intersection(p1, p2, p3, p4);

                Vector2 vec2Origin = new Vector2(segments[i].Origin.x, segments[i].Origin.z);
                float maxDistance = Vector2.Distance(centerCircle, vec2Origin);

                bool hasOtherPoint = false;
                for (int k = 0; k < segments.Count; k++)
                {
                    if (HasOtherPointInCircle(centerCircle, segments[k], maxDistance))
                    {
                        hasOtherPoint = true;
                        break;
                    }
                }

                if (!hasOtherPoint)
                    intersections.Add(centerCircle);
            }
        }
    }

    private bool HasOtherPointInCircle (Vector2 centerCircle, Segment segment, float maxDistance)
    {
        Vector3 centerCir = new Vector3(centerCircle.x, 0, centerCircle.y);
        float distance = Vector3.Distance(centerCir, segment.Final);
        return distance < maxDistance;
    }

    public void DrawPoli (float distanceSegment)
    {
        if (drawPoli)
        {
            DrawSegments(distanceSegment);
            DrawIntersections();
            DrawMediatrix();
        }
    }

    void DrawSegments (float distanceSegment)
    {
        if (segments != null)
        {
            Gizmos.color = Color.cyan;
            foreach (Segment segment in segments)
            {
                Gizmos.DrawRay(segment.Mediatrix, segment.Direction * distanceSegment);
                Gizmos.DrawRay(segment.Mediatrix, -segment.Direction * distanceSegment);
            }
        }
    }

    void DrawIntersections ()
    {
        Gizmos.color = Color.red;
        if (intersections != null)
        {
            foreach (Vector2 intersection in intersections)
            {
                Vector3 pos = new Vector3(intersection.x, 0, intersection.y);
                Gizmos.DrawSphere(pos, 0.5f);
            }
        }
    }

    void DrawMediatrix ()
    {
        Gizmos.color = Color.cyan;
        if (segments != null)
        {
            foreach (Segment segment in segments)
            {
                Gizmos.DrawSphere(segment.Mediatrix, 0.5f);
            }
        }
    }
}