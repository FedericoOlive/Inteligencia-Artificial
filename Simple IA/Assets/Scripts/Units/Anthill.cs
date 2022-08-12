using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Anthill : MonoBehaviour
{
    public GameObject pfAnt;
    public GameObject pfObjetive;
    private Anthill origin;
    private GameObject objetive;
    private List<Ant> ants = new List<Ant>();
    [SerializeField] private Vector2 distance = new Vector2(15, 7.5f);

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
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift))
        {
            StartCoroutine(SpawnMultipleAnts(10));
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SpawnMultipleAnts(1));
        }
    }

    IEnumerator SpawnMultipleAnts (int amount)
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
        ant.Init(origin, objetive.transform);
    }

    public Transform GetNewResource ()
    {
        if (!objetive)
        {
            Vector3 pos = new Vector3(Random.Range(-distance.x, distance.x), 0, Random.Range(-distance.y, distance.y));
            objetive = Instantiate(pfObjetive, pos, Quaternion.identity);
        }

        return objetive.transform;
    }
}