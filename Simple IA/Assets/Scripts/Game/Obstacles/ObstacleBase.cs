using UnityEngine;

public abstract class ObstacleBase : MonoBehaviour
{
    public event System.Action<ObstacleBase> OnDestroy;
    [SerializeField] protected ObstacleType obstacleType;
    
    public void CheckToDestroy()
    {
        if (this.transform.position.x - Camera.main.transform.position.x < -7.5f)
        {
            if (OnDestroy != null)
                OnDestroy.Invoke(this);

            Destroy(this.gameObject);
        }
    }
}

public enum ObstacleType
{
    Wall,
    Mine,
    Saw
}