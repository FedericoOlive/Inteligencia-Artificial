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

        currentState = States.Idle;

        finiteStateMachine.SetRelation(States.GoingToResource, Flags.OnArriveResource, States.Harvesting);
        finiteStateMachine.SetRelation(States.Harvesting, Flags.OnFullInventory, States.GoingToAnthill);
        finiteStateMachine.SetRelation(States.GoingToAnthill, Flags.OnArriveWithResource, States.Depositing);
        finiteStateMachine.SetRelation(States.Depositing, Flags.OnEmptyInventory, States.Idle);
        finiteStateMachine.SetRelation(States.Idle, Flags.OnReceiveResource, States.GoingToResource);

        finiteStateMachine.AddBehaviour(States.Harvesting, HarvestingBehaviour);
        finiteStateMachine.AddBehaviour(States.GoingToResource, GoingToResourceBehaviour);
        finiteStateMachine.AddBehaviour(States.GoingToAnthill, GoingToAnthillBehaviour);
        finiteStateMachine.AddBehaviour(States.Depositing, DepositingBehaviour);
        finiteStateMachine.AddBehaviour(States.Idle, Idle);

        finiteStateMachine.AddBehaviour(States.Idle, () => { Debug.Log("Idle"); });
        finiteStateMachine.AddBehaviour(States.Harvesting, () => { Debug.Log("Harvesting..."); });
        finiteStateMachine.AddBehaviour(States.Depositing, () => { Debug.Log("Depositing..."); });
        finiteStateMachine.AddBehaviour(States.GoingToResource, () => { Debug.Log("Going To Resource"); });
        finiteStateMachine.AddBehaviour(States.GoingToAnthill, () => { Debug.Log("Going To Anthill"); });
    }

    private void Idle ()
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

        if (NodeUtils.GetDistanceXZ(pathBack[0], transform.position) > 0.1f)
        {
            Vector3 movement = dir * stats.speed * Time.deltaTime;
            transform.position += new Vector3(movement.x, 0, movement.z);
        }
        else
        {
            path.Add(pathBack[0]);
            pathBack.Remove(pathBack[0]);
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

        if (NodeUtils.GetDistanceXZ(path[0], transform.position) > 0.1f)
        {
            Vector3 movement = dir * stats.speed * Time.deltaTime;
            transform.position += new Vector3(movement.x, 0, movement.z);
        }
        else
        {
            pathBack.Add(path[0]);
            path.Remove(path[0]);
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
    private void OnDrawGizmosSelected ()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, stats.visionRadius);
    }
#endif
}