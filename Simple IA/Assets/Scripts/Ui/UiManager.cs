using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Anthill anthill;
    [Space(15)] 
    [SerializeField] private Button btnSpawn1;
    [SerializeField] private Button btnSpawn10;
    [SerializeField] private Button btnSpawn100;
    [SerializeField] private Button btnOrderAllToMines;
    [SerializeField] private Button btnOrderAllToAntHill;
    [SerializeField] private Button btnSOrderAllToIdle;

    void Start ()
    {
        btnSpawn1.onClick.AddListener(Spawn1Ant);
        btnSpawn10.onClick.AddListener(Spawn10Ant);
        btnSpawn100.onClick.AddListener(Spawn100Ant);
        btnOrderAllToMines.onClick.AddListener(AllAntsToMines);
        btnOrderAllToAntHill.onClick.AddListener(AllAntsToAnthill);
        btnSOrderAllToIdle.onClick.AddListener(AllAntsToIdle);
    }

    private void Spawn1Ant () => anthill.SpawnAnts(1);
    private void Spawn10Ant () => anthill.SpawnAnts(10);
    private void Spawn100Ant () => anthill.SpawnAnts(100);
    private void AllAntsToMines () => anthill.SpawnAnts(100);
    private void AllAntsToAnthill () => anthill.SpawnAnts(100);
    private void AllAntsToIdle () => anthill.SpawnAnts(100);
}