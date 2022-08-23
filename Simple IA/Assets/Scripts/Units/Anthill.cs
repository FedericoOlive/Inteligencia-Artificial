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
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftControl))
        {
            StartCoroutine(SpawnMultipleAnts(100));
        }
        else if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift))
        {
            StartCoroutine(SpawnMultipleAnts(10));
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SpawnMultipleAnts(1));
        }
    }

    private IEnumerator SpawnMultipleAnts (int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AddAnt();
            yield return new WaitForSeconds(0.1f);
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected ()
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.up, visionRadius);
    }
#endif
}