using UnityEngine;

[System.Serializable]
public class Segment
{
    private Vector3 origin;
    private Vector3 final;

    private Vector3 direction;
    private Vector3 mediatrix;
    public float distance;

    public Segment (Vector3 newOrigin, Vector3 newFinal)
    {
        origin = newOrigin;
        final = newFinal;
        distance = Vector3.Distance(origin, final);

        mediatrix = new Vector3((origin.x + final.x) / 2, (origin.y + final.y) / 2, (origin.z + final.z) / 2);

        direction = new Vector3(final.x - origin.x, final.y - origin.y, final.z - origin.z);
        Vector2 perpendicular = Vector2.Perpendicular(new Vector2(direction.x, direction.z));
        direction.x = perpendicular.x;
        direction.y = 0;
        direction.z = perpendicular.y;
    }

    public Vector3 Direction => direction;
    public Vector3 Mediatrix => mediatrix;
    public Vector3 Origin => origin;
    public Vector3 Final => final;
}