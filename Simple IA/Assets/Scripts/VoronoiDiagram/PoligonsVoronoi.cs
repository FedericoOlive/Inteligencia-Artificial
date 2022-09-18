using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PoligonsVoronoi
{
    public bool drawPoli;
    [SerializeField] private List<Segment> segments = new List<Segment>();
    [SerializeField] private List<Segment> limits = new List<Segment>();
    [SerializeField] private List<Vector3> intersections = new List<Vector3>();
    
    public void SortSegment () => segments.Sort((p1, p2) => p1.Distance.CompareTo(p2.Distance));

    public void AddSegment (Segment refSegment)
    {
        Segment segment = new Segment(refSegment.Origin, refSegment.Final);
        segments.Add(segment);
    }

    public void SetIntersections (List<SegmentLimit> limits)
    {
        intersections.Clear();

        AddSegmentsWithLimits(limits);
        SortSegment();

        for (int i = 0; i < segments.Count; i++)
        {
            for (int j = 0; j < segments.Count; j++)
            {
                if (i == j)
                    continue;
                if (segments[i].id == segments[j].id)
                    continue;

                segments[i].GetTwoPoints(out Vector3 p1, out Vector3 p2);
                segments[j].GetTwoPoints(out Vector3 p3, out Vector3 p4);
                Vector3 centerCircle = Segment.Intersection(p1, p2, p3, p4);

                if (intersections.Contains(centerCircle))
                    continue;

                float maxDistance = Vector3.Distance(centerCircle, segments[i].Origin);

                bool hasOtherPoint = false;
                for (int k = 0; k < segments.Count; k++)
                {
                    if (k == i || k == j)
                        continue;
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

    void AddSegmentsWithLimits(List<SegmentLimit> limits)
    {
        foreach (SegmentLimit limit in limits)
        {
            Vector3 origin = segments[0].Origin;
            Vector3 final = limit.GetOpositePosition(origin);

            Segment segment = new Segment(origin, final);
            this.limits.Add(segment);
            segments.Add(segment);
        }
    }


    private bool HasOtherPointInCircle (Vector3 centerCircle, Segment segment, float maxDistance)
    {
        float distance = Vector3.Distance(centerCircle, segment.Final);
        return distance < maxDistance;
    }

    public void DrawPoli (float distanceSegment)
    {
        if (drawPoli)
        {
            DrawSegments(distanceSegment);
            DrawIntersections();
            //DrawMediatrix();
            DrawOpposite();
            DrawPolygon();
        }
    }

    void DrawOpposite ()
    {
        foreach (Segment limit in limits)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(limit.Final, 0.75f);
        }
    }

    void DrawPolygon ()
    {
        Vector3[] points = new Vector3[intersections.Count];
        Color color = Color.yellow;
        color.a = 0.2f;
        for (int i = 0; i < intersections.Count; i++)
        {
            points[i] = intersections[i];
        }
        
        Handles.color = color;
        Handles.DrawAAConvexPolygon(points);
    }

    void DrawSegments (float distanceSegment)
    {
        if (segments != null)
        {
            foreach (Segment segment in segments)
            {
                segment.DrawSegment(distanceSegment);
            }
        }
    }

    void DrawIntersections ()
    {
        Gizmos.color = Color.red;
        if (intersections != null)
        {
            foreach (Vector3 intersection in intersections)
            {
                Gizmos.DrawSphere(intersection, 0.5f);
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