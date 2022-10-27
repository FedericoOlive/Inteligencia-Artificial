using UnityEngine;

public class ObstacleMine : ObstacleBase
{
    private void Start ()
    {
        minPos = 0.5f;
        maxPos = 9.5f;
        obstacleType = ObstacleType.Mine;
        destroyable = true;

        Vector3 pos = transform.position;
        pos.y = Random.Range(minPos, maxPos);
        transform.position = pos;

        safeZone.a1 = 10;
        safeZone.a2 = up.position.y;
        safeZone.b1 = down.position.y;
        safeZone.b2 = 0;
        SetSafeZone();
    }
}