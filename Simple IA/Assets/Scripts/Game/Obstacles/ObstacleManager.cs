using UnityEngine;
using System.Collections.Generic;

public class ObstacleManager : MonoBehaviour
{
    const float DISTANCE_BETWEEN_OBSTACLES = 6f;
    const float MINDISTANCE_BETWEEN_OBSTACLES = 6f;
    const float MAXDISTANCE_BETWEEN_OBSTACLES = 8f;
    const int MIN_COUNT = 3;

    public GameObject[] prefabsObstacles;
    Vector3 pos = new Vector3(DISTANCE_BETWEEN_OBSTACLES, 0, 0);

    List<ObstacleBase> obstacles = new List<ObstacleBase>();

    private static ObstacleManager instance = null;

    public static ObstacleManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ObstacleManager>();

            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public void Reset()
    {
        for (int i = 0; i < obstacles.Count; i++)
            Destroy(obstacles[i].gameObject);

        obstacles.Clear();

        pos.x = 0;

        InstantiateObstacle();
        InstantiateObstacle();
    }

    public ObstacleBase GetNextObstacle(Vector3 pos)
    {
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (pos.x < obstacles[i].transform.position.x + 1f)
                return obstacles[i];
        }

        return null;
    }

    public void CheckAndInstatiate()
    {
        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].CheckToDestroy();
        }

        while (obstacles.Count < MIN_COUNT)
            InstantiateObstacle();
    }

    void InstantiateObstacle ()
    {
        pos.x += Random.Range(MINDISTANCE_BETWEEN_OBSTACLES, MAXDISTANCE_BETWEEN_OBSTACLES);

        int indexObstacle = Random.Range(0, prefabsObstacles.Length);
        GameObject go = Instantiate(prefabsObstacles[indexObstacle], pos, Quaternion.identity);

        go.transform.SetParent(this.transform, false);
        ObstacleBase obstacleBase = go.GetComponent<ObstacleBase>();
        obstacleBase.OnDestroy += OnObstacleDestroy;
        obstacles.Add(obstacleBase);
    }

    void OnObstacleDestroy(ObstacleBase obstacleBase)
    {
        obstacleBase.OnDestroy -= OnObstacleDestroy;
        obstacles.Remove(obstacleBase);
    }
}
