using UnityEngine;

public class ObstacleMine : ObstacleBase
{
    private void Start ()
    {
        minPos = -4.5f;
        maxPos = 4.5f;
        obstacleType = ObstacleType.Mine;
        destroyable = true;

        Vector3 pos = transform.position;
        pos.y = Random.Range(minPos, maxPos);
        transform.position = pos;
    }
}