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
    public Vector3 destination;
    private Color colorAnt = Color.white;
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
        meshRenderer.material.color = colorAnt;
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
        meshRenderer.material.color = colorAnt;
        Debug.Log("State: " + currentState);
    }

    public void FsmUpdate ()
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
        currentFsm = new FiniteStateMachine(States.Last, Flags.Last);

        currentState = States.WaitingInstructions;
        SetFsmAnt();
    }

    void SetFsmAnt ()
    {
        currentFsm.AddBehaviour(States.Harvesting, HarvestingBehaviour);
        currentFsm.AddBehaviour(States.GoingToResource, GoingToResourceBehaviour, GetPathResource, CleanPath);
        currentFsm.AddBehaviour(States.GoingToAnthill, GoingToAnthillBehaviour, GetPathAnthill, CleanPath);
        currentFsm.AddBehaviour(States.Depositing, DepositingBehaviour, CleanPath);
        currentFsm.AddBehaviour(States.WaitingInstructions, WaitingInstructions, SetColorRed, SetColorNormal);

        currentFsm.AddBehaviour(States.ForceGoingToAnthill, GoingToAnthillBehaviour, GetPathAnthill, CleanPath);
        currentFsm.AddBehaviour(States.ForceGoingToPosition, ForceGoingToPositionBehaviour, GetPathTarget, CleanPath);
        currentFsm.AddBehaviour(States.Idle, IdleBehaviour, IdleEntry, IdleExit);


        currentFsm.SetRelation(States.GoingToResource, Flags.OnArriveResource, States.Harvesting);
        currentFsm.SetRelation(States.Harvesting, Flags.OnFullInventory, States.GoingToAnthill);
        currentFsm.SetRelation(States.GoingToAnthill, Flags.OnArriveAnthill, States.Depositing);
        currentFsm.SetRelation(States.Depositing, Flags.OnEmptyInventory, States.WaitingInstructions);
        currentFsm.SetRelation(States.WaitingInstructions, Flags.OnReceiveResource, States.GoingToResource);

        currentFsm.SetRelation(States.Idle, Flags.Idle, States.Idle);
        currentFsm.SetRelation(States.GoingToResource, Flags.Idle, States.Idle);
        currentFsm.SetRelation(States.Harvesting, Flags.Idle, States.Idle);
        currentFsm.SetRelation(States.GoingToAnthill, Flags.Idle, States.Idle);
        currentFsm.SetRelation(States.Depositing, Flags.Idle, States.Idle);
        currentFsm.SetRelation(States.WaitingInstructions, Flags.Idle, States.Idle);
        currentFsm.SetRelation(States.ForceGoingToAnthill, Flags.Idle, States.Idle);
        currentFsm.SetRelation(States.ForceGoingToPosition, Flags.Idle, States.Idle);
        currentFsm.SetRelation(States.ForceGoingToAnthill, Flags.OnArriveAnthill, States.Idle);

        currentFsm.SetRelation(States.ForceGoingToAnthill, Flags.ForceToPoint, States.ForceGoingToPosition);
        currentFsm.SetRelation(States.Idle, Flags.ForceToPoint, States.ForceGoingToPosition);
        currentFsm.SetRelation(States.GoingToResource, Flags.ForceToPoint, States.ForceGoingToPosition);
        currentFsm.SetRelation(States.Harvesting, Flags.ForceToPoint, States.ForceGoingToPosition);
        currentFsm.SetRelation(States.GoingToAnthill, Flags.ForceToPoint, States.ForceGoingToPosition);
        currentFsm.SetRelation(States.Depositing, Flags.ForceToPoint, States.ForceGoingToPosition);
        currentFsm.SetRelation(States.WaitingInstructions, Flags.ForceToPoint, States.ForceGoingToPosition);
        //currentFsm.SetRelation(States.ForceGoingToPosition, Flags.OnArriveAnthill, States.ForceGoingToPosition);

        currentFsm.SetRelation(States.Idle, Flags.ForceToAnthill, States.ForceGoingToAnthill);
        currentFsm.SetRelation(States.GoingToResource, Flags.ForceToAnthill, States.ForceGoingToAnthill);
        currentFsm.SetRelation(States.Harvesting, Flags.ForceToAnthill, States.ForceGoingToAnthill);
        currentFsm.SetRelation(States.GoingToAnthill, Flags.ForceToAnthill, States.ForceGoingToAnthill);
        currentFsm.SetRelation(States.Depositing, Flags.ForceToAnthill, States.ForceGoingToAnthill);
        currentFsm.SetRelation(States.WaitingInstructions, Flags.ForceToAnthill, States.ForceGoingToAnthill);
        currentFsm.SetRelation(States.ForceGoingToPosition, Flags.ForceToAnthill, States.ForceGoingToAnthill);

        currentFsm.SetRelation(States.ForceGoingToPosition, Flags.OnBackToWork, States.WaitingInstructions);
        currentFsm.SetRelation(States.ForceGoingToAnthill, Flags.OnBackToWork, States.WaitingInstructions);
        currentFsm.SetRelation(States.Idle, Flags.OnBackToWork, States.WaitingInstructions);
    }


    private void ForceGoingToPositionBehaviour()
    {
        if (path.Count < 1)
        {
            currentFsm.SetFlag(ref currentState, Flags.Idle);
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
    
    private void IdleBehaviour()
    {
        
    }

    private void IdleEntry ()
    {
        CleanPath();
        SetColorRed();
    }

    private void IdleExit ()
    {
        CleanPath();
        SetColorNormal();
    }

    private void CleanPath ()
    {
        pathBack.Clear();
        path.Clear();
    }

    private void SetColorRed ()
    {
        colorAnt = Color.red;
    }

    private void SetColorNormal ()
    {
        colorAnt = Color.white;
    }

    private void GetPathAnthill ()
    {
        path = NodeGenerator.GetPath(pos, anthill.pos);
        if (path.Count < 1)
            path = NodeGenerator.GetPath(pos, anthill.pos);
        int i = 0;
    }

    private void GetPathResource()
    {
        path = NodeGenerator.GetPath(pos, resource.pos);
    }

    private void GetPathTarget ()
    {
        Debug.Log("Pos: " + pos + "     Destination: " + destination);
        path = NodeGenerator.GetPath(pos, destination);
    }

    private void WaitingInstructions ()
    {
        Resource newResource = anthill.GetNewResource();
        if (!newResource)
        {
            return;
        }

        resource = newResource;
        colorAnt = Color.white;

        currentFsm.SetFlag(ref currentState, Flags.OnReceiveResource);
    }

    private void GoingToAnthillBehaviour ()
    {
        if (path.Count < 1)
        {
            currentFsm.SetFlag(ref currentState, Flags.OnArriveAnthill);
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

    private void GoingToResourceBehaviour ()
    {
        if (path.Count < 1)
        {
            currentFsm.SetFlag(ref currentState, Flags.OnArriveResource);
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