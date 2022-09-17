using UnityEngine;

[System.Serializable]
public class Segment
{
    public static int amountSegments = 0;
    public int id = 0;
    private Vector3 origin;
    private Vector3 final;

    public Vector3 direction;
    public Vector3 mediatrix;
    public float distance;

    public Vector3 nearIntersection;
    public Vector3 pointFrom;
    public Vector3 pointTo;

    public int idIntersection = 0;
    public Segment segmentIntersection;

    public Segment (Vector3 newOrigin, Vector3 newFinal)
    {
        id = amountSegments;
        amountSegments++;
        origin = newOrigin;
        final = newFinal;
        distance = Vector3.Distance(origin, final);

        mediatrix = new Vector3((origin.x + final.x) / 2, (origin.y + final.y) / 2, (origin.z + final.z) / 2);

        direction = new Vector3(final.x - origin.x, final.y - origin.y, final.z - origin.z);
        Vector2 perpendicular = Vector2.Perpendicular(new Vector2(direction.x, direction.z));
        direction.x = perpendicular.x;
        direction.y = 0;
        direction.z = perpendicular.y;
        nearIntersection = Vector3.zero;
    }

    public Vector3 Direction => direction;
    public Vector3 Mediatrix => mediatrix;
    public Vector3 Origin => origin;
    public Vector3 Final => final;

    public void GetTwoPoints (out Vector2 p1, out Vector2 p2)
    {
        p1 = new Vector2(mediatrix.x, mediatrix.z);
        Vector3 aux = mediatrix + direction * 10;
        p2 = new Vector2(aux.x, aux.z);
    }

    /// <summary>
    /// Calcula el punto de intersección de 2 rectas.
    /// </summary>
    /// <param name="ap1">Recta 1 Punto 1</param>
    /// <param name="ap2">Recta 1 Punto 2</param>
    /// <param name="bp1">Recta 2 Punto 1</param>
    /// <param name="bp2">Recta 2 Punto 2</param>
    /// <returns></returns>
    public static Vector2 Intersection(Vector2 ap1, Vector2 ap2, Vector2 bp1, Vector2 bp2)
    {
        // https://es.wikipedia.org/wiki/Intersección_de_dos_rectas o https://en.wikipedia.org/wiki/Line–line_intersection
        Vector2 intersection = Vector2.zero;
        if (((ap1.x - ap2.x) * (bp1.y - bp2.y) - (ap1.y - ap2.y) * (bp1.x - bp2.x)) == 0)
            return intersection;

        intersection.x = ((ap1.x * ap2.y - ap1.y * ap2.x) * (bp1.x - bp2.x) - (ap1.x - ap2.x) * (bp1.x * bp2.y - bp1.y * bp2.x)) / ((ap1.x - ap2.x) * (bp1.y - bp2.y) - (ap1.y - ap2.y) * (bp1.x - bp2.x));
        intersection.y = ((ap1.x * ap2.y - ap1.y * ap2.x) * (bp1.y - bp2.y) - (ap1.y - ap2.y) * (bp1.x * bp2.y - bp1.y * bp2.x)) / ((ap1.x - ap2.x) * (bp1.y - bp2.y) - (ap1.y - ap2.y) * (bp1.x - bp2.x));

        return intersection;
    }
}