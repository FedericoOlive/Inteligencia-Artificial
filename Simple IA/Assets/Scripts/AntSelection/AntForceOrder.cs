using UnityEngine;

public class AntForceOrder : MonoBehaviour
{
    private AntSelection antSelection;
    private Camera cam;
    [SerializeField] private LayerMask layerResource;
    [SerializeField] private LayerMask layerEnemy;
    [SerializeField] private LayerMask layerAnthill;
    [SerializeField] private LayerMask layerTerrain;


    private void Awake ()
    {
        antSelection = GetComponent<AntSelection>();
        cam = Camera.main;
    }

    private void Update ()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SendSelectedAntToOrder();
        }
    }

    private void SendSelectedAntToOrder ()
    {
        if (antSelection.antSelected.Count < 1)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] hitsResource = Physics.RaycastAll(ray, Mathf.Infinity, layerResource);
        RaycastHit[] hitsEnemy = Physics.RaycastAll(ray, Mathf.Infinity, layerEnemy);
        RaycastHit[] hitsAnthill = Physics.RaycastAll(ray, Mathf.Infinity, layerAnthill);
        RaycastHit[] hitsTerrain = Physics.RaycastAll(ray, Mathf.Infinity, layerTerrain);

        if (HasRaycast(hitsResource))
        {
            foreach (Ant ant in antSelection.antSelected)
            {
                Vector3 origin = ant.transform.position;
                Vector3 final = hitsResource[0].transform.position;
                
                ant.destination = final;
                ant.SetFlag(Flags.ForceToPoint);
                Debug.Log("Send to Resource: " + hitsResource[0].transform.gameObject.name);
            }
        }
        else if (HasRaycast(hitsEnemy))
        {
            //foreach (Ant ant in antSelection.antSelected)
            {
                Debug.Log("Not Implemented!   Send to Enemy: " + hitsEnemy[0].transform.gameObject.name);
            }
        }
        else if (HasRaycast(hitsAnthill))
        {
            foreach (Ant ant in antSelection.antSelected)
            {
                Vector3 origin = ant.transform.position;
                Vector3 final = hitsAnthill[0].transform.position;

                ant.destination = final;
                ant.SetFlag(Flags.ForceToAnthill);

                Debug.Log("Send to Anthill: " + hitsAnthill[0].transform.gameObject.name);
            }
        }
        else if (HasRaycast(hitsTerrain))
        {
            foreach (Ant ant in antSelection.antSelected)
            {
                Vector3 origin = ant.transform.position;
                Vector3 final = hitsTerrain[0].point;
                final = new Vector3(Mathf.RoundToInt(final.x), 0, Mathf.RoundToInt(final.z));

                ant.destination = final;
                ant.SetFlag(Flags.ForceToPoint);

                Debug.Log("Send to Point: " + hitsTerrain[0].transform.gameObject.name);
            }
        }
    }

    bool HasRaycast (RaycastHit[] hits)
    {
        return hits != null && hits.Length > 0;
    }
}

enum ForceState
{
    Go,
    Harvest,
    Attack,
    Last
}