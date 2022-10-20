using UnityEngine;

public class ObstacleWall : ObstacleBase
{
    [SerializeField] private float minDistance = 1;
    [SerializeField] private float maxDistance = 1.5f;
    [SerializeField] private Transform up;
    [SerializeField] private Transform down;

    private void Start()
    {
        obstacleType = ObstacleType.Wall;
        float distance = Random.Range(minDistance, maxDistance);
        Vector3 pos = new Vector3(0, distance, 0);
        up.localPosition = pos;
        down.localPosition = -pos;
    }
}