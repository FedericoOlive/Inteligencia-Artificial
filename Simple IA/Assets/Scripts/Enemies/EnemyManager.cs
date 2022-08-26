using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject pfEnemy;
    [SerializeField] private float randomWaitTime;
    [SerializeField] private Vector3 distanceSpawn = new Vector3(15, 0, 7);

    private List<Enemy> enemies = new List<Enemy>();
    private IEnumerator spawnEnemy;
    private int maxEnemies = 3;

    private void OnEnable ()
    {
        if (spawnEnemy != null)
            StopCoroutine(spawnEnemy);
        spawnEnemy = SpawnEnemy();
        StartCoroutine(spawnEnemy);
    }

    private IEnumerator SpawnEnemy ()
    {
        while (true)
        {
            randomWaitTime = Random.Range(2, 10);

            while (randomWaitTime > 0)
            {
                randomWaitTime -= Time.deltaTime;
                yield return null;
            }

            if (enemies.Count < maxEnemies)
            {
                Vector3 randomPos = Terrain.GetAvailablePosition(new Vector2(-distanceSpawn.x, distanceSpawn.x), new Vector2(-distanceSpawn.y, distanceSpawn.y), transform.position);

                GameObject goEnemy = Instantiate(pfEnemy, randomPos, Quaternion.identity, transform);

                Enemy enemy = goEnemy.GetComponent<Enemy>();
                enemies.Add(enemy);
            }
        }
    }

    private IEnumerator NewSpawnEnemy ()
    {
        while (true)
        {
            randomWaitTime = Random.Range(2, 10);

            while (randomWaitTime > 0)
            {
                randomWaitTime -= Time.deltaTime;
                yield return null;
            }


            if (enemies.Count < maxEnemies)
            {
                int randomAngle = Random.Range(0, 360); // Todo: seguir acá para hacer que spawnee en esa dirección.
                Vector3 pos = new Vector3(Random.Range(-distanceSpawn.x, distanceSpawn.x), 0, Random.Range(-distanceSpawn.z, distanceSpawn.z));
                pos += transform.position;
                GameObject goEnemy = Instantiate(pfEnemy, pos, Quaternion.identity, transform);

                Enemy enemy = goEnemy.GetComponent<Enemy>();
                enemies.Add(enemy);
            }
        }
    }

    private void DestoyEnemy (Enemy enemy)
    {
        enemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected ()
    {
        Handles.color = Color.red;
        Handles.DrawWireCube(transform.position, distanceSpawn * 2);
    }
#endif
}