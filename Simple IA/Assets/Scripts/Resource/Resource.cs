using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Resource : MonoBehaviour
{
    public int id;

    [SerializeField] private ResourceCharge resourceCharge=new ResourceCharge();
    public event Action<Resource> OnEmptyResource;
    public float GetAmount () => resourceCharge.resourceAmount;
    public ResourceType GetResourceType () => resourceCharge.resourceType;

    private void Start ()
    {
        resourceCharge.resourceType = (ResourceType) Random.Range(0, (int) ResourceType.Last);
        resourceCharge.resourceAmount = 3;
    }

    public void TakeResource (ref ResourceCharge antResourceCharge, float maxResourceCharge)
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

    private void DestroyResource ()
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