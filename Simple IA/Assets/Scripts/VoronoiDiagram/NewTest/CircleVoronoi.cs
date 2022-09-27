using UnityEngine;

public class CircleVoronoi
{
    public Vector3 center;
    public float radius;

    public CircleVoronoi (Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector2Int p1Gen = new Vector2Int((int) p1.x, (int) p1.z);
        Vector2Int p2Gen = new Vector2Int((int) p2.x, (int) p2.z);
        Vector2Int p3Gen = new Vector2Int((int) p3.x, (int) p3.z);

        Generic(p1Gen, p2Gen, p3Gen);
    }

    void Generic (Vector2Int p1, Vector2Int p2, Vector2Int p3)
    {



    }

    public static void BelongsToCircle (Vector3 p1, Vector3 p2, Vector3 p3, Vector3 point)
    {

    }

}