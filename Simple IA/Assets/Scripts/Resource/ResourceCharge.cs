[System.Serializable]
public class ResourceCharge
{
    public ResourceCharge(){}
    public ResourceCharge (ResourceType resourceType, float resourceAmount)
    {
        this.resourceType = resourceType;
        this.resourceAmount = resourceAmount;
    }

    public ResourceType resourceType;
    public float resourceAmount;
}