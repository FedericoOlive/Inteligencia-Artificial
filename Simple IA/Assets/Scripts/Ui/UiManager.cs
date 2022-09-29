using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Anthill anthill;
    [SerializeField] private VoronoiDiagram resourceVoronoi;
    [SerializeField] private NodeGenerator nodeGenerator;
    [Space(15)] 
    [SerializeField] private Button btnSpawn1;
    [SerializeField] private Button btnSpawn10;
    [SerializeField] private Button btnSpawn100;
    [SerializeField] private Button btnOrderAllToMines;
    [SerializeField] private Button btnOrderAllToAntHill;
    [SerializeField] private Button btnSOrderAllToIdle;

    [SerializeField] private Toggle toggleShowVoronoi;
    [SerializeField] private Toggle toggleShowPathfinding;
    [SerializeField] private Toggle toggleShowLabel;

    void Start ()
    {
        btnSpawn1.onClick.AddListener(Spawn1Ant);
        btnSpawn10.onClick.AddListener(Spawn10Ant);
        btnSpawn100.onClick.AddListener(Spawn100Ant);
        btnOrderAllToMines.onClick.AddListener(AllAntsToMines);
        btnOrderAllToAntHill.onClick.AddListener(AllAntsToAnthill);
        btnSOrderAllToIdle.onClick.AddListener(AllAntsToIdle);

        toggleShowVoronoi.onValueChanged.AddListener(ToggleShowVoronoi);
        toggleShowPathfinding.onValueChanged.AddListener(ToggleShowPathfinding);
        toggleShowLabel.onValueChanged.AddListener(ToggleShowLabel);
    }

    private void Spawn1Ant () => anthill.SpawnAnts(1);
    private void Spawn10Ant () => anthill.SpawnAnts(10);
    private void Spawn100Ant () => anthill.SpawnAnts(100);
    private void AllAntsToMines () => anthill.SetOrderToAnt(Flags.OnBackToWork);
    private void AllAntsToAnthill () => anthill.SetOrderToAnt(Flags.ForceToAnthill);
    private void AllAntsToIdle () => anthill.SetOrderToAnt(Flags.Idle);

    void ToggleShowVoronoi (bool isOn) => resourceVoronoi.drawPolis = isOn;
    void ToggleShowPathfinding(bool isOn) => nodeGenerator.showNodes = isOn;
    void ToggleShowLabel(bool isOn) => nodeGenerator.showLabel = isOn;
}