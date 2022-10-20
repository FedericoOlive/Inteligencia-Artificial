using UnityEngine;

public class ObstacleSaw : ObstacleBase
{
    private void Start ()
    {
        minPos = -4.5f;
        maxPos = 4.5f;
        velocity = 10.0f;
        obstacleType = ObstacleType.Saw;
    }

    private void Update ()
    {
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
    }
}