using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class VoronoiDiagram : MonoBehaviour
{
    public bool createSegments;
    [SerializeField] private int distanceSegment = 10;

    [Space(15), SerializeField] private List<PoligonsVoronoi> polis = new List<PoligonsVoronoi>();
    [SerializeField] private List<Transform> transformPoints = new List<Transform>();
    [SerializeField] private List<Transform> transformLimits = new List<Transform>();

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
                polis[i].AddSegment(segment);
                polis[j].AddSegment(segment);
            }
        }

        for (int i = 0; i < polis.Count; i++)
        {
            polis[i].SortSegment();
            polis[i].SetIntersections();
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
        DrawPolis();
        DrawLimits();
    }

    private void DrawPolis ()
    {
        if (polis != null)
        {
            foreach (PoligonsVoronoi poli in polis)
            {
                poli.DrawPoli(distanceSegment);
            }
        }
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
#endif
}