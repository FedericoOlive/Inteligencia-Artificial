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

    private Anthill origin;
    private List<Ant> ants = new List<Ant>();
    private float visionRadius = 20.0f;
    [SerializeField] private float timeToSpawningAnts = 0.1f;

    private void Awake ()
    {
        origin = GetComponent<Anthill>();
    }

    private void Start ()
    {
        GetNewResource();
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
        GameObject newAnt = Instantiate(pfAnt, origin.transform.position, Quaternion.identity, origin.transform);
        Ant ant = newAnt.GetComponent<Ant>();
        ants.Add(ant);
        ant.Init(origin, GetNewResource());
    }

    public Transform GetNewResource ()
    {
        Resource resource = resourceManager.GetRandomResource();
        if (resource != null)
            return resource.transform;
        return null;
    }

    public void SetOrderToAnt (Flags flag)
    {
        for (int i = 0; i < ants.Count; i++)
        {
            ants[i].SetFlag(flag);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected ()
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.up, visionRadius);
    }
#endif
}