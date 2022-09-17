using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class VoronoiDiagram : MonoBehaviour
{
    [SerializeField] private List<PoligonsVoronoi> polis = new List<PoligonsVoronoi>();
    [SerializeField] private List<Transform> transformPoints = new List<Transform>();
    [SerializeField] private List<Transform> transformLimits = new List<Transform>();
    [SerializeField] private List<Segment> segments = new List<Segment>();
    public bool createSegments;
    public int distanceSegment;
    public float radiusMediatrix = 0.5f;
    public bool drawSegments;
    [SerializeField] private List<Vector2> intersections = new List<Vector2>();

    private void Update ()
    {
        if (createSegments)
        {
            createSegments = false;
            CreateSegments();
        }
    }

    private void CreateSegments ()
    {
        Segment.amountSegments = 0;
        segments.Clear();
        polis.Clear();
        for (int i = 0; i < transformPoints.Count; i++)
        {
            PoligonsVoronoi poli = new PoligonsVoronoi();
            polis.Add(poli);
        }

        for (int i = 0; i < transformPoints.Count; i++)
        {
            for (int j = i + 1; j < transformPoints.Count; j++)
            {
                Segment segment = new Segment(transformPoints[i].position, transformPoints[j].position);
                segments.Add(segment);
                polis[i].AddSegment(segment);
                polis[j].AddSegment(segment);
            }
        }

        for (int i = 0; i < polis.Count; i++)
        {
            polis[i].SortSegment();
            polis[i].SetIntersections();
        }

        segments.Sort((p1, p2) => p1.distance.CompareTo(p2.distance));


        intersections.Clear();
        for (int i = 0; i < segments.Count; i++)
        {
            for (int j = i + 1; j < segments.Count - 1; j++)
            {
                segments[i].GetTwoPoints(out Vector2 p1, out Vector2 p2);
                segments[j].GetTwoPoints(out Vector2 p3, out Vector2 p4);

                intersections.Add(Segment.Intersection(p1, p2, p3, p4));
            }
        }
    }

    private bool CheckMediatixIsNearOtherPoint (Segment segment, Vector3 point1, Vector3 point2)
    {
        float distance = Vector3.Distance(point1, point2);

        foreach (Transform point in transformPoints)
        {
            if (point.localPosition == point1) continue;
            if (point.localPosition == point2) continue;

            float distanceToSegment = Vector3.Distance(segment.Mediatrix, point.localPosition);
            if (distanceToSegment < distance)
                return true;
        }

        return false;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos ()
    {
        if (polis != null)
        {
            foreach (PoligonsVoronoi poli in polis)
            {
                poli.DrawPoli(distanceSegment, Color.red);
            }
        }

        return;
        DrawLimits();
        DrawSegments();
        DrawPointMediatrix();
        DrawIntersections();
    }

    private void DrawIntersections ()
    {
        //Gizmos.color = Color.red;
        //if (intersections != null)
        //{
        //    foreach (Vector2 intersection in intersections)
        //    {
        //        Vector3 pos = new Vector3(intersection.x, 0, intersection.y);
        //        Gizmos.DrawSphere(pos, radiusMediatrix);
        //    }
        //}
    }

    private void DrawLimits ()
    {
        if (transformLimits != null)
            for (int i = 0; i < transformLimits.Count; i++)
            {
                Vector3 origin = transformLimits[i].position;
                Vector3 final = i < transformLimits.Count - 1 ? transformLimits[i + 1].position : transformLimits[0].position;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(origin, final);
            }
    }

    private void DrawSegments ()
    {
        if (!drawSegments)
            return;
        Gizmos.color = Color.blue;
        if (segments != null)
        {
            foreach (Segment segment in segments)
            {
                Gizmos.DrawRay(segment.Mediatrix, segment.Direction * distanceSegment);
                Gizmos.DrawRay(segment.Mediatrix, -segment.Direction * distanceSegment);
            }
        }
    }

    private void DrawPointMediatrix ()
    {
        //Gizmos.color = Color.cyan;
        //if (segments != null)
        //    foreach (Segment segment in segments)
        //    {
        //        Gizmos.DrawSphere(segment.Mediatrix, radiusMediatrix);
        //    }
    }
#endif
}