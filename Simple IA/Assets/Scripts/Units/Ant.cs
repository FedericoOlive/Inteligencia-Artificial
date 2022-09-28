using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Ant : MonoBehaviour
{
    [SerializeField] private Stats stats = new Stats();
    private MeshRenderer meshRenderer;
    private Outline outline;
    [SerializeField] private Anthill anthill;
    [SerializeField] private Resource resource;
    [SerializeField] private States currentState;
    private Vector3 pos;
    private float deltaTime = 0;

    private bool isSelected = false;
    public bool IsSelected
    {
        get => isSelected;

        set
        {
            isSelected = value;
            outline.OutlineWidth = isSelected ? 10 : 0;
        }
    }


    private float currentActionTime = 0.0f;

    private ResourceCharge resourceCharge = new ResourceCharge();
    private List<FiniteStateMachine> fsmList = new List<FiniteStateMachine>();
    private FiniteStateMachine currentFsm;

    [SerializeField] private List<Vector3Int> path = new List<Vector3Int>();
    [SerializeField] private List<Vector3Int> pathBack = new List<Vector3Int>();
    public Flags externalFlag = Flags.Last;
    private float offsetFailDistance = 0.03f;

    private void Awake ()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        outline = GetComponent<Outline>();
        pos = transform.position;
    }

    public void Init (Anthill anthill, Transform resource)
    {
        SetFsm();
        meshRenderer.material.color = Color.white;
        this.anthill = anthill;
    }

    private void Start ()
    {
        SetFsm();
    }

    private void Update ()
    {
        transform.position = pos;
        deltaTime = Time.deltaTime;
    }

    public void CustomUpdate ()
    {
        currentFsm.Update(ref currentState);
    }

    public void SetPath (List<Vector3Int> newPath)
    {
        path = newPath;
        pathBack.Clear();
    }

    private void SetFsm ()
    {
        for (int i = 0; i < 2; i++)
        {
            currentFsm = new FiniteStateMachine(States.Last, Flags.Last);
            fsmList.Add(currentFsm);
        }
        currentFsm = fsmList[0];

        currentState = States.WaitingInstructions;
        SetFsmAnt();
        SetFsmManual();
    }

    void SetFsmAnt ()
    {
        fsmList[0].SetRelation(States.GoingToResource, Flags.OnArriveResource, States.Harvesting);
        fsmList[0].SetRelation(States.Harvesting, Flags.OnFullInventory, States.GoingToAnthill);
        fsmList[0].SetRelation(States.GoingToAnthill, Flags.OnArriveWithResource, States.Depositing);
        fsmList[0].SetRelation(States.Depositing, Flags.OnEmptyInventory, States.WaitingInstructions);
        fsmList[0].SetRelation(States.WaitingInstructions, Flags.OnReceiveResource, States.GoingToResource);

        // Exit FSM Cases:
        fsmList[0].SetRelation(States.Idle, Flags.ForceIndicator, States.ForceIndicator);
        fsmList[0].SetRelation(States.GoingToResource, Flags.ForceIndicator, States.ForceIndicator);
        fsmList[0].SetRelation(States.Harvesting, Flags.ForceIndicator, States.ForceIndicator);
        fsmList[0].SetRelation(States.GoingToAnthill, Flags.ForceIndicator, States.ForceIndicator);
        fsmList[0].SetRelation(States.Depositing, Flags.ForceIndicator, States.ForceIndicator);
        fsmList[0].SetRelation(States.WaitingInstructions, Flags.ForceIndicator, States.ForceIndicator);


        fsmList[0].AddBehaviour(States.Harvesting, HarvestingBehaviour);
        fsmList[0].AddBehaviour(States.GoingToResource, GoingToResourceBehaviour);
        fsmList[0].AddBehaviour(States.GoingToAnthill, GoingToAnthillBehaviour);
        fsmList[0].AddBehaviour(States.Depositing, DepositingBehaviour);
        fsmList[0].AddBehaviour(States.WaitingInstructions, WaitingInstructions);
        fsmList[0].AddBehaviour(States.ForceIndicator, ForceIndicator);
    }

    void SetFsmManual()
    {
        fsmList[1].SetRelation(States.ForceIndicator, Flags.ForceToPosition, States.ForceGoingToPosition);
        fsmList[1].SetRelation(States.ForceIndicator, Flags.ForceToIdle, States.ForceGoingToIdle);
        fsmList[1].SetRelation(States.ForceIndicator, Flags.ForceToAnthill, States.ForceGoingToAnthill);
        fsmList[1].SetRelation(States.ForceIndicator, Flags.ForceToResource, States.ForceToHarvasting);
       
        fsmList[1].AddBehaviour(States.ForceGoingToPosition, ForceGoingToPositionBehaviour);
        fsmList[1].AddBehaviour(States.ForceGoingToAnthill, ForceGoingToAnthillBehaviour);
        fsmList[1].AddBehaviour(States.ForceGoingToIdle, ForceGoingToIdleBehaviour);
    }

    void ForceIndicator ()
    {
        SetFSM(1);
        SetFlag(externalFlag);
    }


    private void ForceGoingToPositionBehaviour()
    {

    }

    private void ForceGoingToAnthillBehaviour()
    {

    }

    private void ForceGoingToIdleBehaviour()
    {

    }

    private void WaitingInstructions ()
    {
        meshRenderer.material.color = Color.red;
        if (!resource)
        {
            Transform newResource = anthill.GetNewResource();
            if (newResource)
            {
                resource = newResource.GetComponent<Resource>();
                path = NodeGenerator.GetPath(pos, resource.pos);
            }

            return;
        }

        meshRenderer.material.color = Color.white;

        currentFsm.SetFlag(ref currentState, Flags.OnReceiveResource);
    }

    private void GoingToAnthillBehaviour ()
    {
        if (pathBack.Count < 1)
        {
            currentFsm.SetFlag(ref currentState, Flags.OnArriveWithResource);
            path.Reverse();
            return;
        }

        if (path.Count < 1)
        {
            path.Add(pathBack[0]);
            pathBack.Remove(pathBack[0]);
        }
        
        Vector3 dir = (pathBack[0] - pos);
        dir.Normalize();

        if (NodeUtils.GetDistanceXZ(pathBack[0], pos) > offsetFailDistance)
        {
            Vector3 movement = dir * (stats.speed * TerrainSettings.GetSpeedInTerrain(pathBack[0])) * deltaTime;
            pos += new Vector3(movement.x, 0, movement.z);
        }
        else
        {
            pos = pathBack[0];
            path.Add(pathBack[0]);
            pathBack.Remove(pathBack[0]);

            if (pathBack.Count > 0)
            {
                dir = (pathBack[0] - path[^1]);
                dir.Normalize();
                Vector3 movement = dir * stats.speed * deltaTime;
                pos += new Vector3(movement.x, 0, movement.z);
            }
        }
    }

    private void GoingToResourceBehaviour ()
    {
        if (path.Count < 1)
        {
            currentFsm.SetFlag(ref currentState, Flags.OnArriveResource);
            pathBack.Reverse();
            return;
        }

        if (pathBack.Count < 1)
        {
            pathBack.Add(path[0]);
            path.Remove(path[0]);
        }
        
        Vector3 dir = (path[0] - pos);
        dir.Normalize();

        if (NodeUtils.GetDistanceXZ(path[0], pos) > offsetFailDistance)
        {
            Vector3 movement = dir * (stats.speed * TerrainSettings.GetSpeedInTerrain(path[0])) * deltaTime;
            pos += new Vector3(movement.x, 0, movement.z);
        }
        else
        {
            pos = path[0];
            pathBack.Add(path[0]);
            path.Remove(path[0]);

            if (path.Count > 0)
            {
                dir = (path[0] - pathBack[^1]);
                dir.Normalize();
                Vector3 movement = dir * stats.speed * deltaTime;
                pos += new Vector3(movement.x, 0, movement.z);
            }
        }
    }

    private void HarvestingBehaviour ()
    {
        if (currentActionTime < stats.pickTime)
        {
            currentActionTime += deltaTime;
        }
        else
        {
            currentActionTime = 0.0f;
            currentFsm.SetFlag(ref currentState, Flags.OnFullInventory);

            if (resource)
                resource.TakeResource(ref resourceCharge, stats.maxChargeResource);
        }
    }

    private void DepositingBehaviour ()
    {
        if (currentActionTime < stats.dropTime)
        {
            currentActionTime += deltaTime;
        }
        else
        {
            currentActionTime = 0.0f;
            currentFsm.SetFlag(ref currentState, Flags.OnEmptyInventory);
        }
    }

    public void SetFlag (Flags flag)
    {
        currentFsm.SetFlag(ref currentState, flag);
    }

    public void SetFSM (int nextFsm)
    {
        currentFsm.exitBehaviour?.Invoke();
        currentFsm = fsmList[nextFsm];
        currentFsm.entryBehaviour?.Invoke();
    }

#if UNITY_EDITOR

    private void OnDrawGizmos ()
    {
        if (path != null)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }

        if (pathBack != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < pathBack.Count - 1; i++)
            {
                Gizmos.DrawLine(pathBack[i], pathBack[i + 1]);
            }
        }
    }

    private void OnDrawGizmosSelected ()
    {
        //Handles.color = Color.red;
        //Handles.DrawWireDisc(pos, Vector3.up, stats.visionRadius);
    }
#endif
}