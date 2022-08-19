using System.Collections.Generic;
using UnityEngine;

public class AntSelection : MonoBehaviour
{
    public List<Ant> antSelected = new List<Ant>();
    [SerializeField] private AntsInBox selection;

    private Vector3 clickDown;
    private Vector3 clickUp;

    private void Start ()
    {
        selection.OnDetectAnt += AddAntToSelection;
        selection.gameObject.SetActive(false);
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (Ant ant in antSelected)
            {
                ant.meshRenderer.material.color = Color.white;
            }

            antSelected.Clear();
            selection.gameObject.SetActive(true);
            clickDown = Input.mousePosition;
            selection.Init(clickDown);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            clickUp = Input.mousePosition;
            selection.gameObject.SetActive(false);
        }
    }

    void AddAntToSelection (Ant ant, bool isEnterSelection)
    {
        if (isEnterSelection)
            antSelected.Add(ant);
        else
            antSelected.Remove(ant);
    }
}