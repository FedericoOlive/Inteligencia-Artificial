using UnityEngine;

public class Ant : MonoBehaviour
{
    //[SerializeField] private GameObject objetive;
    [SerializeField] private GameObject anthill;
    [SerializeField] private Resource resource;

    private float speed = 10.0f;
    private float miningTime = 5.0f;
    private float currentMiningTime = 0.0f;

    private ResourceCharge resourceCharge = new ResourceCharge();
    private int maxResourceCharge = 1;

    //private int mineUses = 10;

    private FiniteStateMachine finiteStateMachine;

    void Start()
    {
        finiteStateMachine = new FiniteStateMachine((int) States.Last, (int) Flags.Last);

        finiteStateMachine.ForceCurretState((int) States.GoToMine);

        finiteStateMachine.SetRelation((int) States.GoToMine, (int) Flags.OnReachMine, (int) States.Mining);
        finiteStateMachine.SetRelation((int) States.Mining, (int) Flags.OnFullInventory, (int) States.GoToAnthill);
        finiteStateMachine.SetRelation((int) States.GoToAnthill, (int) Flags.OnReachDeposit, (int) States.GoToMine);
        finiteStateMachine.SetRelation((int) States.GoToAnthill, (int) Flags.OnEmpyMine, (int) States.Idle);


        finiteStateMachine.AddBehaviour((int) States.Mining, MiningBehaviour);
        finiteStateMachine.AddBehaviour((int) States.GoToMine, GoingBehaviour);
        finiteStateMachine.AddBehaviour((int) States.GoToAnthill, Going2Behaviour);


        finiteStateMachine.AddBehaviour((int) States.Idle, () => { Debug.Log("Idle"); });
        finiteStateMachine.AddBehaviour((int) States.Mining, () => { Debug.Log("Taking"); });
        finiteStateMachine.AddBehaviour((int) States.GoToMine, () => { Debug.Log("Go To Mine"); });
        finiteStateMachine.AddBehaviour((int) States.GoToAnthill, () => { Debug.Log("Go To Anthill"); });
    }

    private void Going2Behaviour ()
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
                finiteStateMachine.SetFlag((int) Flags.OnEmpyMine);
            else
                finiteStateMachine.SetFlag((int) Flags.OnReachDeposit);
        }
    }

    private void GoingBehaviour ()
    {
        Vector3 dir = (resource.transform.position - transform.position).normalized;

        if (Vector3.Distance(resource.transform.position, transform.position) > 0.1f)
        {
            Vector3 movement = dir * speed * Time.deltaTime;
            transform.position += new Vector3(movement.x, 0, movement.z);
        }
        else
        {
            finiteStateMachine.SetFlag((int) Flags.OnReachMine);
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
            finiteStateMachine.SetFlag((int) Flags.OnFullInventory);

            resource.TakeResource(ref resourceCharge, maxResourceCharge);
        }
    }

    // Update is called once per frame
    void Update()
    {
        finiteStateMachine.Update();
    }
}
