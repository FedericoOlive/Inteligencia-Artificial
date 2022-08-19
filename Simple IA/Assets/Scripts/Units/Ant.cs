using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ant : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    [SerializeField] private Anthill anthill;
    [SerializeField] private Resource resource;
    [SerializeField] private States currentState;

    private float speed = 10.0f;
    private float miningTime = 1.0f;
    private float currentMiningTime = 0.0f;

    private ResourceCharge resourceCharge = new ResourceCharge();
    private int maxResourceCharge = 1;
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

    void SetFsm ()
    {
        finiteStateMachine = new FiniteStateMachine(States.Last, Flags.Last);

        currentState = States.Idle;

        finiteStateMachine.SetRelation(States.GoToMine, Flags.OnReachMine, States.Mining);
        finiteStateMachine.SetRelation(States.Mining, Flags.OnFullInventory, States.GoToAnthill);
        finiteStateMachine.SetRelation(States.GoToAnthill, Flags.OnEmpyMine, States.Idle);
        finiteStateMachine.SetRelation(States.GoToAnthill, Flags.OnReachDeposit, States.GoToMine);
        finiteStateMachine.SetRelation(States.Idle, Flags.OnReachDeposit, States.GoToMine);

        finiteStateMachine.AddBehaviour(States.Mining, MiningBehaviour);
        finiteStateMachine.AddBehaviour(States.GoToMine, GoingToResourceBehaviour);
        finiteStateMachine.AddBehaviour(States.GoToAnthill, GoingToAnthillBehaviour);
        finiteStateMachine.AddBehaviour(States.Idle, WaitingInstruction);

        finiteStateMachine.AddBehaviour(States.Idle, () => { Debug.Log("Idle"); });
        finiteStateMachine.AddBehaviour(States.Mining, () => { Debug.Log("Taking"); });
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
            Vector3 movement = dir * speed * Time.deltaTime;
            transform.position += new Vector3(movement.x, 0, movement.z);
        }
        else
        {
            if (resource.GetAmount() <= 0)
                finiteStateMachine.SetFlag(ref currentState, Flags.OnEmpyMine);
            else
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
            Vector3 movement = dir * speed * Time.deltaTime;
            transform.position += new Vector3(movement.x, 0, movement.z);
        }
        else
        {
            finiteStateMachine.SetFlag(ref currentState, Flags.OnReachMine);
        }
    }

    private void MiningBehaviour ()
    {
        if (currentMiningTime < miningTime)
        {
            currentMiningTime += Time.deltaTime;
        }
        else
        {
            currentMiningTime = 0.0f;
            finiteStateMachine.SetFlag(ref currentState, Flags.OnFullInventory);

            if (resource)
                resource.TakeResource(ref resourceCharge, maxResourceCharge);
        }
    }

    void Update ()
    {
        if (finiteStateMachine == null)
            SetFsm();
        finiteStateMachine.Update(ref currentState);
    }
}