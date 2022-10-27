using UnityEngine;

public class ObstacleSaw : ObstacleBase
{
    private void Start ()
    {
        minPos = 0.5f;
        maxPos = 9.5f;
        velocity = 5.0f;
        obstacleType = ObstacleType.Saw;

        Vector3 pos = transform.position;
        pos.y = Random.Range(minPos, maxPos);
        transform.position = pos;

        if (Random.Range(0, 1) == 0) 
            velocity *= -1;
    }

    private void FixedUpdate ()
    {
        safeZone.a1 = 10;
        safeZone.a2 = up.position.y;
        safeZone.b1 = down.position.y;
        safeZone.b2 = 0;

        Vector3 pos = transform.position;
        pos.y += velocity * Time.deltaTime;

        if (pos.y > maxPos)
        {
            pos.y = maxPos;
            velocity *= -1;
        }
        else if (pos.y < minPos)
        {
            pos.y = minPos;
            velocity *= -1;
        }

        transform.position = pos;
        SetSafeZone();
    }
}