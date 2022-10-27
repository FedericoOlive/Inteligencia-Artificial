using UnityEngine;

public class ObstacleWall : ObstacleBase
{
    [SerializeField] private float minDistance = 1;
    [SerializeField] private float maxDistance = 1.5f;

    private void Start ()
    {
        minPos = 8;
        maxPos = 2.0f;
        velocity = 0.0f;
        obstacleType = ObstacleType.Wall;

        Vector3 pos = transform.position;
        pos.y = Random.Range(minPos, maxPos);
        transform.position = pos;

        float distance = Random.Range(minDistance, maxDistance);
        pos = new Vector3(0, distance, 0);
        up.localPosition = pos;
        down.localPosition = -pos;

        safeZone.a1 = up.position.y;
        safeZone.a2 = down.position.y;
        safeZone.b1 = up.position.y;
        safeZone.b2 = down.position.y;
        SetSafeZone();
    }
}