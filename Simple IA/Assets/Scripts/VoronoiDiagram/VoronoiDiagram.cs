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
    public bool drawPolis;

    [Space(15), SerializeField] private List<PoligonsVoronoi> polis = new List<PoligonsVoronoi>();
    [SerializeField] private List<Transform> transformPoints = new List<Transform>();
    [SerializeField] private List<SegmentLimit> segmentLimit = new List<SegmentLimit>();


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
            for (int j = 0; j < transformPoints.Count; j++)
            {
                if (i == j)
                    continue;
                Segment segment = new Segment(transformPoints[i].position, transformPoints[j].position);
                polis[i].AddSegment(segment);
            }
        }

        for (int i = 0; i < polis.Count; i++)
        {
            polis[i].SetIntersections(segmentLimit);
        }
    }

#if UNITY_EDITOR
    
    private void OnDrawGizmos ()
    {
        DrawPolis(drawPolis);
    }

    private void DrawPolis (bool drawPolis)
    {
        if (polis != null)
        {
            foreach (PoligonsVoronoi poli in polis)
            {
                poli.DrawPoli(drawPolis);
            }
        }
    }
#endif
}