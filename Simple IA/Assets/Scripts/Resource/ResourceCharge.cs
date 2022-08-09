using UnityEngine;

[System.Serializable]
public class ResourceCharge
{
    public ResourceCharge(){}
    public ResourceCharge (ResourceType resourceType, int resourceAmount)
    {
        this.resourceType = resourceType;
        this.resourceAmount = resourceAmount;
    }

    public ResourceType resourceType;
    public int resourceAmount;
}