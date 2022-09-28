using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Anthill : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private GameObject pfAnt;
    private ConcurrentBag<Ant> ants = new ConcurrentBag<Ant>();
    private ParallelOptions parallelOptions = new ParallelOptions();

    private Anthill origin;
    private float visionRadius = 25.0f;
    [SerializeField] private float timeToSpawningAnts = 0.1f;

    private void Awake ()
    {
        origin = GetComponent<Anthill>();
    }

    private void Start ()
    {
        GetNewResource();
    }

    private void Update ()
    {
        Parallel.ForEach(ants, parallelOptions, ant =>
        {
            ant.CustomUpdate();
        });
    }

    public void SpawnAnts (int amount) => StartCoroutine(SpawnMultipleAnts(amount));
    
    private IEnumerator SpawnMultipleAnts (int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AddAnt();
            yield return new WaitForSeconds(timeToSpawningAnts);
        }
    }

    private void AddAnt ()
    {
        Vector3 pos = origin.transform.position;
        pos.y = 0;
        GameObject newAnt = Instantiate(pfAnt, pos, Quaternion.identity, origin.transform);
        Ant ant = newAnt.GetComponent<Ant>();
        ants.Add(ant);
        ant.Init(origin, GetNewResource());
    }

    public Transform GetNewResource ()
    {
        Resource resource = resourceManager.GetNearResourceVoronoi(origin.transform.position);

        if (resource != null)
            return resource.transform;
        return null;
    }

    public void SetOrderToAnt (Flags flag)
    {
        Parallel.ForEach(ants, parallelOptions, ant =>
        {
            ant.SetFlag(flag);
        });
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected ()
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.up, visionRadius);
    }
#endif
}