using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Ant : MonoBehaviour
{
    [SerializeField] private Stats stats = new Stats();
    public MeshRenderer meshRenderer;
    [SerializeField] private Anthill anthill;
    [SerializeField] private Resource resource;
    [SerializeField] private States currentState;

    private float currentActionTime = 0.0f;

    private ResourceCharge resourceCharge = new ResourceCharge();
    private FiniteStateMachine finiteStateMachine;

    [SerializeField] private List<Vector3Int> path = new List<Vector3Int>();
    [SerializeField] private List<Vector3Int> pathBack = new List<Vector3Int>();

    private float offsetFailDistance = 0.03f;

    private void Awake ()
    {
        meshRenderer = GetComponent<MeshRenderer>();
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
        finiteStateMachine.Update(ref currentState);
    }

    private void SetFsm ()
    {
        finiteStateMachine = new FiniteStateMachine(States.Last, Flags.Last);

        currentState = States.WaitingInstructions;

        finiteStateMachine.SetRelation(States.GoingToResource, Flags.OnArriveResource, States.Harvesting);
        finiteStateMachine.SetRelation(States.Harvesting, Flags.OnFullInventory, States.GoingToAnthill);
        finiteStateMachine.SetRelation(States.GoingToAnthill, Flags.OnArriveWithResource, States.Depositing);
        finiteStateMachine.SetRelation(States.Depositing, Flags.OnEmptyInventory, States.WaitingInstructions);
        finiteStateMachine.SetRelation(States.WaitingInstructions, Flags.OnReceiveResource, States.GoingToResource);

        finiteStateMachine.AddBehaviour(States.Harvesting, HarvestingBehaviour);
        finiteStateMachine.AddBehaviour(States.GoingToResource, GoingToResourceBehaviour);
        finiteStateMachine.AddBehaviour(States.GoingToAnthill, GoingToAnthillBehaviour);
        finiteStateMachine.AddBehaviour(States.Depositing, DepositingBehaviour);
        finiteStateMachine.AddBehaviour(States.WaitingInstructions, WaitingInstructions);
        
        finiteStateMachine.AddBehaviour(States.WaitingInstructions, () => { Debug.Log("Waiting Instructions"); });
        finiteStateMachine.AddBehaviour(States.Harvesting, () => { Debug.Log("Harvesting..."); });
        finiteStateMachine.AddBehaviour(States.Depositing, () => { Debug.Log("Depositing..."); });
        finiteStateMachine.AddBehaviour(States.GoingToResource, () => { Debug.Log("Going To Resource"); });
        finiteStateMachine.AddBehaviour(States.GoingToAnthill, () => { Debug.Log("Going To Anthill"); });


        // Hacer Varios FMS y que se pueda intercambiar uno entre otro
        // Un scriptableObject podría ser la configuracion de donde se tome.
        // Separar todos los behaviours en scripts (ver como se podría hacer para pasar los parámetros)
        // Todo: Ver clase de distintos FSM: 18/08/2022 Minuto 27.
        finiteStateMachine.SetRelation(States.Idle, Flags.ForceToPosition, States.ForceGoingToPosition);
        finiteStateMachine.SetRelation(States.WaitingInstructions, Flags.ForceToPosition, States.ForceGoingToPosition);
        finiteStateMachine.SetRelation(States.Harvesting, Flags.ForceToPosition, States.ForceGoingToPosition);
        finiteStateMachine.SetRelation(States.GoingToResource, Flags.ForceToPosition, States.ForceGoingToPosition);
        finiteStateMachine.SetRelation(States.GoingToAnthill, Flags.ForceToPosition, States.ForceGoingToPosition);
        finiteStateMachine.SetRelation(States.Depositing, Flags.ForceToPosition, States.ForceGoingToPosition);

        finiteStateMachine.AddBehaviour(States.ForceGoingToPosition, ForceGoingToPositionBehaviour);
        finiteStateMachine.AddBehaviour(States.ForceGoingToAnthill, ForceGoingToAnthillBehaviour);
        finiteStateMachine.AddBehaviour(States.ForceGoingToIdle, ForceGoingToIdleBehaviour);
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

        finiteStateMachine.SetFlag(ref currentState, Flags.OnReceiveResource);
    }

    private void GoingToAnthillBehaviour ()
    {
        if (pathBack.Count < 1)
        {
            finiteStateMachine.SetFlag(ref currentState, Flags.OnArriveWithResource);
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
            finiteStateMachine.SetFlag(ref currentState, Flags.OnArriveResource);
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
            finiteStateMachine.SetFlag(ref currentState, Flags.OnFullInventory);

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
            finiteStateMachine.SetFlag(ref currentState, Flags.OnEmptyInventory);
        }
    }

    public void SetFlag (Flags flag)
    {
        finiteStateMachine.SetFlag(ref currentState, flag);
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