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

        finiteStateMachine.SetRelation(States.GoToMine, Flags.OnReachMine, States.Harvesting);
        finiteStateMachine.SetRelation(States.Harvesting, Flags.OnFullInventory, States.GoToAnthill);
        finiteStateMachine.SetRelation(States.GoToAnthill, Flags.OnEmpyMine, States.Idle);
        finiteStateMachine.SetRelation(States.GoToAnthill, Flags.OnReachDeposit, States.GoToMine);
        finiteStateMachine.SetRelation(States.Idle, Flags.OnReachDeposit, States.GoToMine);

        finiteStateMachine.AddBehaviour(States.Harvesting, MiningBehaviour);
        finiteStateMachine.AddBehaviour(States.GoToMine, GoingToResourceBehaviour);
        finiteStateMachine.AddBehaviour(States.GoToAnthill, GoingToAnthillBehaviour);
        finiteStateMachine.AddBehaviour(States.Idle, WaitingInstruction);

        finiteStateMachine.AddBehaviour(States.Idle, () => { Debug.Log("Idle"); });
        finiteStateMachine.AddBehaviour(States.Harvesting, () => { Debug.Log("Taking"); });
        finiteStateMachine.AddBehaviour(States.GoToMine, () => { Debug.Log("Go To Mine"); });
        finiteStateMachine.AddBehaviour(States.GoToAnthill, () => { Debug.Log("Go To Anthill"); });
    }

    private void WaitingInstruction ()
    {
        if (!resource)
        {
            Transform newResource = anthill.GetNewResource();
            if (newResource)
                resource = newResource.GetComponent<Resource>();
            return;
        }

        finiteStateMachine.SetFlag(ref currentState, Flags.OnReachDeposit);
    }

    private void GoingToAnthillBehaviour ()
    {
        Vector3 dir = (anthill.transform.position - transform.position).normalized;

        if (Vector3.Distance(anthill.transform.position, transform.position) > 0.1f)
        {
            Vector3 movement = dir * stats.speed * Time.deltaTime;
            transform.position += new Vector3(movement.x, 0, movement.z);
        }
        else
        {
            finiteStateMachine.SetFlag(ref currentState, Flags.OnReachDeposit);
        }
    }

    private void GoingToResourceBehaviour ()
    {
        if (!resource)
        {
            finiteStateMachine.SetFlag(ref currentState, Flags.OnReachMine);
            return;
        }

        Vector3 dir = (resource.transform.position - transform.position).normalized;

        if (Vector3.Distance(resource.transform.position, transform.position) > 0.1f)
        {
            Vector3 movement = dir * stats.speed * Time.deltaTime;
            transform.position += new Vector3(movement.x, 0, movement.z);
        }
        else
        {
            finiteStateMachine.SetFlag(ref currentState, Flags.OnReachMine);
        }
    }

    private void MiningBehaviour ()
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected ()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, stats.visionRadius);
    }
#endif
}