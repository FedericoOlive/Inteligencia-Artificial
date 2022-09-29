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

    private VoronoiDiagram voronoiDiagram;

    private void Awake ()
    {
        voronoiDiagram = GetComponent<VoronoiDiagram>();
    }

    private void Update ()
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

    private void CreateResource ()
    {
        Vector3 randomPos = Vector3.zero;
        bool posOcupped = false;
        do
        {
            posOcupped = false;
            randomPos = TerrainTextureDetector.GetRandomAvailablePosition(distanceSpawn);
            foreach (Resource res in resources)
            {
                if (res.transform.position == randomPos)
                    posOcupped = true;
            }

        } while (posOcupped);

        randomPos.y = transform.position.y;
        Resource resource = Instantiate(pfResource, randomPos, Quaternion.identity, transform).GetComponent<Resource>();
        resource.OnEmptyResource += DestroyResource;
        resources.Add(resource);
        voronoiDiagram.AddNewItem(resource.transform);
    }

    private void DestroyResource (Resource resource)
    {
        voronoiDiagram.RemoveItem(resource.transform);
        resource.OnEmptyResource -= DestroyResource;
        Destroy(resource.gameObject);
        resources.Remove(resource);
    }

    public List<Resource> GetResources () => resources;

    public Resource GetNearResource (Vector3 pos, float visionRadius)
    {
        Resource nearResource = null;
        float distance = visionRadius;

        for (int i = 0; i < resources.Count; i++)
        {
            float currentDistance = NodeUtils.GetDistanceXZ(pos, resources[i].transform.position);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                nearResource = resources[i];
            }
        }

        return nearResource;
    }

    public Resource GetNearResourceVoronoi (Vector3 pos)
    {
        pos.y = 0;
        Resource nearResource = null;
        int indexPoly = 0;
        for (int i = 0; i < voronoiDiagram.GetPoly.Count; i++)  // En que Poly está el Anthill
        {
            if (voronoiDiagram.GetPoly[i].IsInside(pos))
            {
                indexPoly = i;
                break;
            }
        }

        for (int i = 0; i < resources.Count; i++)   // En el poly del Anthill, cual es el recurso dentro
        {
            if (voronoiDiagram.GetPoly[indexPoly].IsInside(resources[i].pos))
            {
                return resources[i];
            }
        }

        return nearResource;
    }

    public Resource GetRandomResource ()
    {
        if (resources.Count > 0)
            return resources[Random.Range(0, resources.Count)];
        return null;
    }
}