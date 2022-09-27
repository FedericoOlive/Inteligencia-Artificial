using System;
using UnityEngine;

public class AntsInBox : MonoBehaviour
{
    public event Action<Ant, bool> OnDetectAnt;

    private Camera cam;
    private Vector3 initialPos;
    private Vector3 currentPos;
    private Vector3 center;
    private Vector3 distance;

    private void Awake ()
    {
        cam = Camera.main;
    }

    public void Init (Vector3 mouseInitPosition)
    {
        gameObject.SetActive(true);
        initialPos = cam.ScreenToWorldPoint(mouseInitPosition);
        initialPos.y = 0;
        currentPos = initialPos;
        transform.localScale = Vector3.zero;
    }

    private void Update ()
    {
        if (Input.GetMouseButton(0))
        {
            currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            currentPos.y = 0;

            center = (currentPos + initialPos) / 2;
            distance.x = Mathf.Abs(Mathf.Abs(currentPos.x) - Mathf.Abs(initialPos.x));
            distance.y = 0.1f;
            distance.z = Mathf.Abs(Mathf.Abs(currentPos.z) - Mathf.Abs(initialPos.z));
            transform.position = center;
            transform.localScale = distance;
        }
    }

    private void OnTriggerEnter (Collider other)
    {
        Ant ant = other.gameObject.GetComponent<Ant>();
        if (ant)
        {
            OnDetectAnt?.Invoke(ant, true);
        }
    }

    private void OnTriggerExit (Collider other)
    {
        Ant ant = other.gameObject.GetComponent<Ant>();
        if (ant)
        {
            OnDetectAnt?.Invoke(ant, false);
        }
    }
}