using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PoligonsVoronoi
{
    public bool drawPoli;
    [SerializeField] List<Segment> segments = new List<Segment>();
    [SerializeField] private List<Vector2> intersections = new List<Vector2>();
    private Color[] colors = new[] {Color.red, Color.blue, Color.green, Color.yellow,};
    public void AddSegment (Segment refSegment)
    {
        Segment segment = new Segment(refSegment.Origin, refSegment.Final);
        segments.Add(segment);
    }

    public void SortSegment () => segments.Sort((p1, p2) => p1.distance.CompareTo(p2.distance));

    public void DrawPoli (float distanceSegment, Color color)
    {
        if (drawPoli)
        {
            Gizmos.color = Color.cyan;
            if (segments != null)
            {
                foreach (Segment segment in segments)
                {
                    Gizmos.DrawRay(segment.Mediatrix, segment.Direction * distanceSegment);
                    Gizmos.DrawRay(segment.Mediatrix, -segment.Direction * distanceSegment);
                }
            }

            Gizmos.color = Color.red;
            if (intersections != null)
            {
                foreach (Vector2 intersection in intersections)
                {
                    Vector3 pos = new Vector3(intersection.x, 0, intersection.y);
                    Gizmos.DrawSphere(pos, 0.5f);
                }
            }

            Gizmos.color = Color.cyan;
            if (segments != null)
            {
                foreach (Segment segment in segments)
                {
                    Gizmos.DrawSphere(segment.Mediatrix, 0.5f);
                }
            }
        }

        for (int i = 0; i < segments.Count; i++)
        {
            Gizmos.color = colors[i];
            Gizmos.DrawLine(segments[i].pointFrom, segments[i].pointTo);
        }

        Gizmos.color = Color.black;
    }

    public void SetIntersections ()
    {
        SortSegment();
        intersections.Clear();
        Segment currentPath = null;
        List<Segment> banneds = new List<Segment>();
        segments[0].pointFrom = segments[0].Mediatrix;
        banneds.Add(segments[0]);
        FindNext(segments[0], ref banneds);
    }

    void FindNext (Segment currentSegment, ref List<Segment> banneds)
    {
        int indexSegment = -1;
        float distance = float.MaxValue;

        for (int i = 0; i < segments.Count; i++)
        {
            if (currentSegment.id == segments[i].id)
                continue;
            bool goNext = false;

            for (int j = 0; j < banneds.Count; j++)
            {
                if (banneds[j].id == segments[i].id)
                {
                    goNext = true;
                    break;
                }
            }

            if (goNext)
                continue;

            Vector2 p1 = new Vector2(currentSegment.mediatrix.x, currentSegment.mediatrix.z);
            Vector2 p2 = new Vector2(currentSegment.pointFrom.x, currentSegment.pointFrom.z);
            segments[i].GetTwoPoints(out Vector2 p3, out Vector2 p4);

            Vector2 posIntersec = Segment.Intersection(p1, p2, p3, p4);
            Vector3 pos = new Vector3(posIntersec.x, 0, posIntersec.y);
            if (pos == currentSegment.pointFrom)
                continue;
            float newDist = Vector3.Distance(segments[0].Mediatrix, pos);

            if (newDist < distance)
            {
                distance = newDist;
                currentSegment.pointTo = pos;
                indexSegment = i;
            }
        }

        if (indexSegment > -1)
        {
            segments[indexSegment].pointFrom = currentSegment.pointTo;
            banneds.Add(currentSegment);
            FindNext(segments[indexSegment], ref banneds);
        }
    }

    public void SetIntersectionss ()
    {
        SortSegment();
        intersections.Clear();
        //for (int i = 0; i < segments.Count; i++)
        //{
        //    for (int j = i + 1; j < segments.Count - 1; j++)
        //    {
        //        segments[i].GetTwoPoints(out Vector2 p1, out Vector2 p2);
        //        segments[j].GetTwoPoints(out Vector2 p3, out Vector2 p4);
        //
        //        intersections.Add(Segment.Intersection(p1, p2, p3, p4));
        //    }
        //
        //    //float distance = float.MaxValue;
        //    //for (int j = i + 1; j < intersections.Count; j++)
        //    //{
        //    //    if (Vector3.Distance(segments[i].Mediatrix, intersections[j]) < distance)
        //    //    {
        //    //        Vector3 newIntersection = new Vector3(intersections[j].x, 0, intersections[j].y);
        //    //        segments[i].nearIntersection = newIntersection;
        //    //        segments[i].segmentIntersection = segments[j];
        //    //    }
        //    //}
        //}

        List<Segment> pendingSegments = new List<Segment>();
        for (var i = 0; i < Segments.Count; i++)
        {
            Segment segment = Segments[i];
            pendingSegments.Add(segment);
        }

        for (int i = 0; i < segments.Count; i++)
        {
            Segment nearSegment = null;
            float distance = float.MaxValue;

            for (int j = 0; j < pendingSegments.Count; j++)
            {
                segments[i].GetTwoPoints(out Vector2 p1, out Vector2 p2);
                segments[j].GetTwoPoints(out Vector2 p3, out Vector2 p4);
                Vector2 posIntersec = Segment.Intersection(p1, p2, p3, p4);
                Vector3 pos = new Vector3(posIntersec.x, 0, posIntersec.y);

                float newDist = Vector3.Distance(segments[i].Mediatrix, pos);
                if (segments[i].id == pendingSegments[j].id)
                    continue;

                if (newDist < distance)
                {
                    distance = newDist;
                    nearSegment = pendingSegments[j];
                    segments[i].nearIntersection = pos;
                    segments[i].segmentIntersection = pendingSegments[j];
                    segments[i].idIntersection = pendingSegments[j].id;
                }
            }

            if (nearSegment != null)
            {
                pendingSegments.Remove(nearSegment);
                pendingSegments.Insert(0, nearSegment);
                pendingSegments.Remove(segments[i]);
                Vector2 intersecPos = new Vector3(segments[i].nearIntersection.x, segments[i].nearIntersection.z);
                intersections.Add(intersecPos);
            }
        }
    }

    public List<Segment> Segments => segments;
}