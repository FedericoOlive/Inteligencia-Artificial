using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PoligonsVoronoi
{
    public bool drawPoli;
    public int weight = 0;
    [SerializeField] private Transform itemSector;
    [SerializeField] private List<Segment> segments = new List<Segment>();
    [SerializeField] private List<Segment> limits = new List<Segment>();
    [SerializeField] private List<Vector3> intersections = new List<Vector3>();
    [SerializeField] private List<int> indexIntersections = new List<int>();
    private List<Vector3> allIntersections;
    private Color colorGizmos = new Color(0, 0, 0, 0);
    public void SortSegment () => segments.Sort((p1, p2) => p1.Distance.CompareTo(p2.Distance));
    
    public PoligonsVoronoi (Transform item, List<Vector3> allIntersections)
    {
        itemSector = item;
        this.allIntersections = allIntersections;
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
        weight = 0;

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


        indexIntersections.Clear();
        for (int i = 0; i < intersections.Count; i++)
        {
            Vector3 intersection = intersections[i];
            if (!allIntersections.Contains(intersection))
            {
                allIntersections.Add(intersection);
                indexIntersections.Add(allIntersections.Count - 1);
            }
            else
            {
                for (int j = 0; j < allIntersections.Count; j++)
                {
                    if (allIntersections[j] == intersection)
                    {
                        indexIntersections.Add(j);
                        break;
                    }
                }
            }
        }

        UpdateIntersectionList();
    }

    void UpdateIntersectionList ()
    {
        intersections.Clear();

        for (int i = 0; i < indexIntersections.Count; i++)
        {
            intersections.Add(allIntersections[indexIntersections[i]]);
        }
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

    public static bool IsPointInSegment(Vector3 point, Vector3 start, Vector3 end)
    {
        return (point.x <= Mathf.Max(start.x, end.x) &&
                point.x >= Mathf.Min(start.x, end.x) &&
                point.z <= Mathf.Max(start.z, end.z) &&
                point.z >= Mathf.Min(start.z, end.z));
    }
}