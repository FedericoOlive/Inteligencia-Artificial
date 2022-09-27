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
    private FiniteStateMachine fsmAnt;
    private FiniteStateMachine fsmManual;

    [SerializeField] private List<Vector3Int> path = new List<Vector3Int>();
    [SerializeField] private List<Vector3Int> pathBack = new List<Vector3Int>();

    private float offsetFailDistance = 0.03f;

    private void Awake ()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        outline = GetComponent<Outline>();
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
        fsmAnt.Update(ref currentState);
    }

    private void SetFsm ()
    {
        fsmAnt = new FiniteStateMachine(States.Last, Flags.Last);
        fsmManual = new FiniteStateMachine(States.Last, Flags.Last);

        currentState = States.WaitingInstructions;
        SetFsmAnt();
        SetFsmManual();
    }

    void SetFsmAnt ()
    {
        fsmAnt.SetRelation(States.GoingToResource, Flags.OnArriveResource, States.Harvesting);
        fsmAnt.SetRelation(States.Harvesting, Flags.OnFullInventory, States.GoingToAnthill);
        fsmAnt.SetRelation(States.GoingToAnthill, Flags.OnArriveWithResource, States.Depositing);
        fsmAnt.SetRelation(States.Depositing, Flags.OnEmptyInventory, States.WaitingInstructions);
        fsmAnt.SetRelation(States.WaitingInstructions, Flags.OnReceiveResource, States.GoingToResource);

        fsmAnt.AddBehaviour(States.Harvesting, HarvestingBehaviour);
        fsmAnt.AddBehaviour(States.GoingToResource, GoingToResourceBehaviour);
        fsmAnt.AddBehaviour(States.GoingToAnthill, GoingToAnthillBehaviour);
        fsmAnt.AddBehaviour(States.Depositing, DepositingBehaviour);
        fsmAnt.AddBehaviour(States.WaitingInstructions, WaitingInstructions);

        // Exit FSM Cases:
        fsmAnt.SetRelation(States.Idle, Flags.ForceIndicator, States.ForceIndicator);
        fsmAnt.SetRelation(States.GoingToResource, Flags.ForceIndicator, States.ForceIndicator);
        fsmAnt.SetRelation(States.Harvesting, Flags.ForceIndicator, States.ForceIndicator);
        fsmAnt.SetRelation(States.GoingToAnthill, Flags.ForceIndicator, States.ForceIndicator);
        fsmAnt.SetRelation(States.Depositing, Flags.ForceIndicator, States.ForceIndicator);
        fsmAnt.SetRelation(States.WaitingInstructions, Flags.ForceIndicator, States.ForceIndicator);
    }

    void SetFsmManual()
    {
        fsmManual.SetRelation(States.ForceIndicator, Flags.ForceToPosition, States.ForceGoingToPosition);
        fsmManual.SetRelation(States.ForceIndicator, Flags.ForceToIdle, States.ForceGoingToIdle);
        fsmManual.SetRelation(States.ForceIndicator, Flags.ForceToAnthill, States.ForceGoingToAnthill);
        fsmManual.SetRelation(States.ForceIndicator, Flags.ForceToHarvesting, States.ForceToHarvasting);

        fsmManual.AddBehaviour(States.ForceGoingToPosition, ForceGoingToPositionBehaviour);
        fsmManual.AddBehaviour(States.ForceGoingToAnthill, ForceGoingToAnthillBehaviour);
        fsmManual.AddBehaviour(States.ForceGoingToIdle, ForceGoingToIdleBehaviour);

        // Exit FSM Cases:

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
                path = NodeGenerator.GetPath(transform.position, resource.transform.position);
            }

            return;
        }

        meshRenderer.material.color = Color.white;

        fsmAnt.SetFlag(ref currentState, Flags.OnReceiveResource);
    }

    private void GoingToAnthillBehaviour ()
    {
        if (pathBack.Count < 1)
        {
            fsmAnt.SetFlag(ref currentState, Flags.OnArriveWithResource);
            path.Reverse();
            return;
        }

        if (path.Count < 1)
        {
            path.Add(pathBack[0]);
            pathBack.Remove(pathBack[0]);
        }
        
        Vector3 dir = (pathBack[0] - transform.position);
        dir.Normalize();

        if (NodeUtils.GetDistanceXZ(pathBack[0], transform.position) > offsetFailDistance)
        {
            Vector3 movement = dir * (stats.speed * TerrainSettings.GetSpeedInTerrain(pathBack[0])) * Time.deltaTime;
            transform.position += new Vector3(movement.x, 0, movement.z);
        }
        else
        {
            transform.position = pathBack[0];
            path.Add(pathBack[0]);
            pathBack.Remove(pathBack[0]);

            if (pathBack.Count > 0)
            {
                dir = (pathBack[0] - path[^1]);
                dir.Normalize();
                Vector3 movement = dir * stats.speed * Time.deltaTime;
                transform.position += new Vector3(movement.x, 0, movement.z);
            }
        }
    }

    private void GoingToResourceBehaviour ()
    {
        if (path.Count < 1)
        {
            fsmAnt.SetFlag(ref currentState, Flags.OnArriveResource);
            pathBack.Reverse();
            return;
        }

        if (pathBack.Count < 1)
        {
            pathBack.Add(path[0]);
            path.Remove(path[0]);
        }
        
        Vector3 dir = (path[0] - transform.position);
        dir.Normalize();

        if (NodeUtils.GetDistanceXZ(path[0], transform.position) > offsetFailDistance)
        {
            Vector3 movement = dir * (stats.speed * TerrainSettings.GetSpeedInTerrain(path[0])) * Time.deltaTime;
            transform.position += new Vector3(movement.x, 0, movement.z);
        }
        else
        {
            transform.position = path[0];
            pathBack.Add(path[0]);
            path.Remove(path[0]);

            if (path.Count > 0)
            {
                dir = (path[0] - pathBack[^1]);
                dir.Normalize();
                Vector3 movement = dir * stats.speed * Time.deltaTime;
                transform.position += new Vector3(movement.x, 0, movement.z);
            }
        }
    }

    private void HarvestingBehaviour ()
    {
        if (currentActionTime < stats.pickTime)
        {
            currentActionTime += Time.deltaTime;
        }
        else
        {
            currentActionTime = 0.0f;
            fsmAnt.SetFlag(ref currentState, Flags.OnFullInventory);

            if (resource)
                resource.TakeResource(ref resourceCharge, stats.maxChargeResource);
        }
    }

    private void DepositingBehaviour ()
    {
        if (currentActionTime < stats.dropTime)
        {
            currentActionTime += Time.deltaTime;
        }
        else
        {
            currentActionTime = 0.0f;
            fsmAnt.SetFlag(ref currentState, Flags.OnEmptyInventory);
        }
    }

    public void SetFlag (Flags flag)
    {
        fsmAnt.SetFlag(ref currentState, flag);
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
        //Handles.DrawWireDisc(transform.position, Vector3.up, stats.visionRadius);
    }
#endif
}