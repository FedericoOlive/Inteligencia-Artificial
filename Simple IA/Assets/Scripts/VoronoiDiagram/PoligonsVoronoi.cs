using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PoligonsVoronoi
{
    public bool drawPoli;
    [SerializeField] private Transform itemSector;
    [SerializeField] private List<Segment> segments = new List<Segment>();
    [SerializeField] private List<Segment> limits = new List<Segment>();
    [SerializeField] private List<Vector3> intersections = new List<Vector3>();
    private Color colorGizmos = new Color(0, 0, 0, 0);
    public void SortSegment () => segments.Sort((p1, p2) => p1.Distance.CompareTo(p2.Distance));

    public PoligonsVoronoi (Transform item)
    {
        itemSector = item;
    }

    public void AddSegment (Segment refSegment)
    {
        Segment segment = new Segment(refSegment.Origin, refSegment.Final);
        segments.Add(segment);
    }

    public void SetIntersections ()
    {
        colorGizmos.r = Random.Range(0, 1.0f);
        colorGizmos.g = Random.Range(0, 1.0f);
        colorGizmos.b = Random.Range(0, 1.0f);
        colorGizmos.a = 0.3f;

        intersections.Clear();

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
                {
                    intersections.Add(centerCircle);
                    segments[i].intersection.Add(centerCircle);
                    segments[j].intersection.Add(centerCircle);
                }
            }
        }

        RemoveUnusedSegments();
        SortPointsPolygon();
    }

    public void AddSegmentsWithLimits (List<SegmentLimit> limits)
    {
        foreach (SegmentLimit limit in limits)
        {
            Vector3 origin = itemSector.transform.position;
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

    void RemoveUnusedSegments ()
    {
        List<Segment> segmentsUnused = new List<Segment>();
        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i].intersection.Count != 2)
                segmentsUnused.Add(segments[i]);
        }

        for (int i = 0; i < segmentsUnused.Count; i++)
        {
            segments.Remove(segmentsUnused[i]);
        }
    }

    void SortPointsPolygon ()
    {
        intersections.Clear();
        Vector3 lastIntersection = segments[0].intersection[0];
        intersections.Add(lastIntersection);

        Vector3 firstIntersection;
        Vector3 secondIntersection;

        for (int i = 0; i < segments.Count; i++)
        {
            for (int j = 0; j < segments.Count; j++)
            {
                if (i == j)
                    continue;

                firstIntersection = segments[j].intersection[0];
                secondIntersection = segments[j].intersection[1];

                if (!intersections.Contains(secondIntersection))
                    if (firstIntersection == lastIntersection)
                    {
                        intersections.Add(secondIntersection);
                        lastIntersection = secondIntersection;
                        break;
                    }

                if (!intersections.Contains(firstIntersection))
                    if (secondIntersection == lastIntersection)
                    {
                        intersections.Add(firstIntersection);
                        lastIntersection = firstIntersection;
                        break;
                    }
            }
        }

        firstIntersection = segments[^1].intersection[0];
        if (!intersections.Contains(firstIntersection))
            intersections.Add(firstIntersection);
        secondIntersection = segments[^1].intersection[1];
        if (!intersections.Contains(secondIntersection))
            intersections.Add(secondIntersection);
    }

    public void DrawPoli (bool drawPolis)
    {
        if (drawPolis)
            DrawPolygon();
        else if (drawPoli)
            DrawPolygon();
    }

    void DrawPolygon ()
    {
        Vector3[] points = new Vector3[intersections.Count + 1];

        for (int i = 0; i < intersections.Count; i++)
        {
            points[i] = intersections[i];
        }

        points[intersections.Count] = points[0];
        Handles.color = colorGizmos;
        Handles.DrawAAConvexPolygon(points);

        Handles.color = Color.black;
        Handles.DrawPolyLine(points);
    }


    // https://www.geeksforgeeks.org/how-to-check-if-a-given-point-lies-inside-a-polygon/?ref=gcse
    public bool IsInside (Vector3 point)
    {
        int lenght = intersections.Count;

        if (lenght < 3)
        {
            return false;
        }
        
        Vector3 extreme = new Vector3(100, 0, point.z);
        
        int count = 0;
        for (int i = 0; i < lenght; i++)
        {
            int next = (i + 1) % lenght;
            
            Vector3 intersection = Segment.Intersection(intersections[i], intersections[next], point, extreme);
            if (intersection != Vector3.zero)
                if (IsPointInSegment(intersection, intersections[i], intersections[next]))
                    if (IsPointInSegment(intersection, point, extreme))
                        count++;
        } 
        
        return (count % 2 == 1); 
    }

    public bool IsPointInSegment(Vector3 point, Vector3 start, Vector3 end)
    {
        return (point.x <= Mathf.Max(start.x, end.x) &&
                point.x >= Mathf.Min(start.x, end.x) &&
                point.z <= Mathf.Max(start.z, end.z) &&
                point.z >= Mathf.Min(start.z, end.z));
    }

    public int Orientation(Vector3 p, Vector3 q, Vector3 r)
    {
        int val = (int)((q.z - p.z) * (r.x - q.x) - (q.x - p.x) * (r.z - q.z));

        if (val == 0)
        {
            return 0; // collinear
        }
        return (val > 0) ? 1 : 2; // clock or counterclock wise
    }

    public bool DoIntersect(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
    {
        // Find the four orientations needed for
        // general and special cases
        int o1 = Orientation(p1, q1, p2);
        int o2 = Orientation(p1, q1, q2);
        int o3 = Orientation(p2, q2, p1);
        int o4 = Orientation(p2, q2, q1);

        // General case
        if (o1 != o2 && o3 != o4)
        {
            return true;
        }

        // Special Cases
        // p1, q1 and p2 are collinear and
        // p2 lies on segment p1q1
        if (o1 == 0 && IsPointInSegment(p1, p2, q1))
        {
            return true;
        }

        // p1, q1 and p2 are collinear and
        // q2 lies on segment p1q1
        if (o2 == 0 && IsPointInSegment(p1, q2, q1))
        {
            return true;
        }

        // p2, q2 and p1 are collinear and
        // p1 lies on segment p2q2
        if (o3 == 0 && IsPointInSegment(p2, p1, q2))
        {
            return true;
        }

        // p2, q2 and q1 are collinear and
        // q1 lies on segment p2q2
        if (o4 == 0 && IsPointInSegment(p2, q1, q2))
        {
            return true;
        }

        // Doesn't fall in any of the above cases
        return false;
    }
}