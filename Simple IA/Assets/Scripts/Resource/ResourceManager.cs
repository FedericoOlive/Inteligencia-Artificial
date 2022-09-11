using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private GameObject pfResource;
    [SerializeField] private Vector3Int distanceSpawn = new Vector3Int(15, 0, 7);
    [SerializeField] private List<Resource> resources = new List<Resource>();
    [SerializeField] private float spawnTimeResource = 2f;
    private float currentSpawnTimeResource;
    [SerializeField] private int maxAmountResources = 10;

    void Update ()
    {
        float deltaTime = Time.deltaTime;
        currentSpawnTimeResource += deltaTime;

        if (currentSpawnTimeResource > spawnTimeResource)
        {
            currentSpawnTimeResource = 0;
            if (resources.Count < maxAmountResources)
            {
                CreateResource();
            }
        }
    }

    void CreateResource ()
    {
        Vector3 randomPos = TerrainTextureDetector.GetRandomAvailablePosition(distanceSpawn);
        Resource resource = Instantiate(pfResource, randomPos, Quaternion.identity, transform).GetComponent<Resource>();
        resource.OnEmptyResource += DestroyResource;
        resources.Add(resource);
    }

    private void DestroyResource (Resource resource)
    {
        resource.OnEmptyResource -= DestroyResource;
        Destroy(resource.gameObject);
        resources.Remove(resource);
    }

    public List<Resource> GetResources () => resources;

    public Resource GetRandomResource ()
    {
        if (resources.Count > 0)
            return resources[Random.Range(0, resources.Count)];
        return null;
    }
}