using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private ResourceCharge resourceCharge=new ResourceCharge();

    public int GetAmount () => resourceCharge.resourceAmount;
    public ResourceType GetResourceType () => resourceCharge.resourceType;

    private void Start ()
    {
        resourceCharge.resourceType = (ResourceType) Random.Range(0, (int) ResourceType.Last);
        resourceCharge.resourceAmount = Random.Range(1, 11);
    }

    public void TakeResource (ref ResourceCharge antResourceCharge, int maxResourceCharge)
    {
        antResourceCharge.resourceType = resourceCharge.resourceType;
        resourceCharge.resourceAmount -= maxResourceCharge;
        if (resourceCharge.resourceAmount <= 0)
        {
            antResourceCharge.resourceAmount += resourceCharge.resourceAmount;
            DestroyResource();
            return;
        }

        antResourceCharge.resourceAmount = maxResourceCharge;
    }

    void DestroyResource ()
    {
        Destroy(gameObject);
    }
}

public enum ResourceType
{
    Seed,
    Meat,
    Honey,
    Grass,
    Last
}