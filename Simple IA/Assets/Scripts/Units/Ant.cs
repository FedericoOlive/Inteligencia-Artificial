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

    private List<Vector3Int> path = new List<Vector3Int>();
    private List<Vector3Int> pathBack = new List<Vector3Int>();


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
                resource = newResource.GetComponent<Resource>();
            return;
        }
        meshRenderer.material.color = Color.white;

        finiteStateMachine.SetFlag(ref currentState, Flags.OnReceiveResource);
    }

    private void GoingToAnthillBehaviour ()
    {
        Vector3 dir = (anthill.transform.position - transform.position).normalized;

        if (GetDistanceXZ(anthill.transform.position, transform.position) > 0.1f)
        {
            Vector3 movement = dir * stats.speed * Time.deltaTime;
            transform.position += new Vector3(movement.x, 0, movement.z);
        }
        else
        {
            finiteStateMachine.SetFlag(ref currentState, Flags.OnArriveWithResource);
        }
    }

    private void GoingToResourceBehaviour ()
    {
        if (!resource)
        {
            finiteStateMachine.SetFlag(ref currentState, Flags.OnArriveResource);
            return;
        }

        Vector3 dir = (resource.transform.position - transform.position).normalized;

        if (GetDistanceXZ(resource.transform.position, transform.position) > 0.1f)
        {
            Vector3 movement = dir * stats.speed * Time.deltaTime;
            transform.position += new Vector3(movement.x, 0, movement.z);
        }
        else
        {
            finiteStateMachine.SetFlag(ref currentState, Flags.OnArriveResource);
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

    private float GetDistanceXZ (Vector3 vec3One, Vector3 vec3Two)
    {
        Vector2 pos1 = new Vector2(vec3One.x, vec3One.z);
        Vector2 pos2 = new Vector2(vec3Two.x, vec3Two.z);
        return Vector2.Distance(pos1, pos2);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected ()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, stats.visionRadius);
    }
#endif
}