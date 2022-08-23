using UnityEngine;

public class AntForceOrder : MonoBehaviour
{
    private AntSelection antSelection;
    private Camera cam;

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

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            foreach (Ant ant in antSelection.antSelected)
            {
                Debug.Log("Send to: " + hit.transform.gameObject.name);
            }
        }
    }
}

enum ForceState
{
    Go,
    Harvest,
    Attack,
    Last
}