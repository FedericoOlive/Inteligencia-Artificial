using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    enum States
    {
        Mining,
        GoToMine,
        GoToDeposit,
        Idle,
        Last
    }

    enum Flags
    {
        OnFullInventory,
        OnReachMine,
        OnReachDeposit,
        OnEmpyMine,
        Last
    }

    public GameObject mine;
    public GameObject deposit;

    private float speed = 10.0f;
    private float miningTime = 5.0f;
    private float currentMiningTime = 0.0f;
    private int mineUses = 10;

    private FiniteStateMachine finiteStateMachine;
    // Start is called before the first frame update
    void Start()
    {
        finiteStateMachine = new FiniteStateMachine((int)States.Last, (int)Flags.Last);
        finiteStateMachine.ForceCurretState((int)States.GoToMine);

        finiteStateMachine.SetRelation((int)States.GoToMine, (int)Flags.OnReachMine, (int)States.Mining);
        finiteStateMachine.SetRelation((int)States.Mining, (int)Flags.OnFullInventory, (int)States.GoToDeposit);
        finiteStateMachine.SetRelation((int)States.GoToDeposit, (int)Flags.OnReachDeposit, (int)States.GoToMine);
        finiteStateMachine.SetRelation((int)States.GoToDeposit, (int)Flags.OnEmpyMine, (int)States.Idle);

        finiteStateMachine.AddBehaviour((int)States.Idle, () => { Debug.Log("Idle"); });

        finiteStateMachine.AddBehaviour((int)States.Mining, () =>
        {
            if (currentMiningTime < miningTime)
            {
                currentMiningTime += Time.deltaTime;
            }
            else
            {
                currentMiningTime = 0.0f;
                finiteStateMachine.SetFlag((int)Flags.OnFullInventory);
                mineUses--;
            }
        });
        finiteStateMachine.AddBehaviour((int)States.Mining, () =>{ Debug.Log("Mining"); });

        finiteStateMachine.AddBehaviour((int)States.GoToMine, () =>
        {

            Vector2 dir = (mine.transform.position - transform.position).normalized;

            if (Vector2.Distance(mine.transform.position, transform.position) > 1.0f)
            {
                Vector2 movement = dir * 10.0f * Time.deltaTime;
                transform.position += new Vector3(movement.x, movement.y);
            }
            else
            {
                finiteStateMachine.SetFlag((int)Flags.OnReachMine);
            }
        });
        finiteStateMachine.AddBehaviour((int)States.GoToMine, () => { Debug.Log("GoToMine"); });

        finiteStateMachine.AddBehaviour((int)States.GoToDeposit, () =>
        {
            Vector2 dir = (deposit.transform.position - transform.position).normalized;

            if (Vector2.Distance(deposit.transform.position, transform.position) > 1.0f)
            {
                Vector2 movement = dir * 10.0f * Time.deltaTime;
                transform.position += new Vector3(movement.x, movement.y);
            }
            else
            {
                if (mineUses <= 0)
                    finiteStateMachine.SetFlag((int)Flags.OnEmpyMine);
                else
                    finiteStateMachine.SetFlag((int)Flags.OnReachDeposit);
            }
        });
        finiteStateMachine.AddBehaviour((int)States.GoToDeposit, () => { Debug.Log("GoToDeposit"); });

    }

    // Update is called once per frame
    void Update()
    {
        finiteStateMachine.Update();
    }
}
