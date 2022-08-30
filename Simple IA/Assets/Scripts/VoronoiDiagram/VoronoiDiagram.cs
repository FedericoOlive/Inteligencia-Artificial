using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class VoronoiDiagram : MonoBehaviour
{
    [SerializeField] private List<Transform> transformPoints = new List<Transform>();
    [SerializeField] private List<Transform> transformLimits = new List<Transform>();
    [SerializeField] private List<Segment> segments = new List<Segment>();
    public bool createSegments;
    public int distanceSegment;
    public float radiusMediatrix = 0.5f;

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
        segments.Clear();
        for (int i = 0; i < transformPoints.Count; i++)
        {
            for (int j = i + 1; j < transformPoints.Count; j++)
            {
                Segment segment;
                if (j >= transformPoints.Count)
                    segment = new Segment(transformPoints[i].localPosition, transformPoints[0].localPosition);
                else
                    segment = new Segment(transformPoints[i].localPosition, transformPoints[j].localPosition);

                segments.Add(segment);
            }
        }

        DeleteUnusedSegments();
    }

    private void DeleteUnusedSegments ()
    {

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
        DrawLimits();
        DrawSegments();
        DrawPointMediatrix();
    }

    private void DrawLimits ()
    {
        if (transformLimits != null)
            for (int i = 0; i < transformLimits.Count; i++)
            {
                Vector3 origin = transformLimits[i].localPosition;
                Vector3 final = i < transformLimits.Count - 1 ? transformLimits[i + 1].localPosition : transformLimits[0].localPosition;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(origin, final);
            }
    }

    private void DrawSegments ()
    {
        Gizmos.color = Color.blue;
        if (segments != null)
            foreach (Segment segment in segments)
            {
                Gizmos.DrawRay(segment.Mediatrix, segment.Direction * distanceSegment);
                Gizmos.DrawRay(segment.Mediatrix, -segment.Direction * distanceSegment);
            }
    }

    private void DrawPointMediatrix ()
    {
        Gizmos.color = Color.cyan;
        if (segments != null)
            foreach (Segment segment in segments)
            {
                Gizmos.DrawSphere(segment.Mediatrix, radiusMediatrix);
            }
    }
#endif
}