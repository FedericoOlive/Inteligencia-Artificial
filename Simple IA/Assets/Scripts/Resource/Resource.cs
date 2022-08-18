using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Resource : MonoBehaviour
{
    [SerializeField] private ResourceCharge resourceCharge=new ResourceCharge();
    public event Action<Resource> OnEmptyResource;
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
        OnEmptyResource?.Invoke(this);
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