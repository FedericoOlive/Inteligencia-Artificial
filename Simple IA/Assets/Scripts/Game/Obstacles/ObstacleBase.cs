using System.Collections.Generic;
using UnityEngine;

public abstract class ObstacleBase : MonoBehaviour
{
    public event System.Action<ObstacleBase> OnDestroy;
    public ObstacleType obstacleType;
    public float maxPos;
    public float minPos;
    public float velocity;
    public bool destroyable;
    public List<BirdBase> birdsDisables = new List<BirdBase>();

    public void CheckToDestroy ()
    {
        if (transform.position.x - Camera.main.transform.position.x < -10.0f)
        {
            if (OnDestroy != null)
                OnDestroy.Invoke(this);

            Destroy(this.gameObject);
        }
    }

    public bool Disable (BirdBase birdBase)
    {
        if (destroyable)
        {
            if (!birdsDisables.Contains(birdBase))
            {
                birdsDisables.Add(birdBase);
                return true;
            }
        }

        return false;
    }
}

public enum ObstacleType
{
    Wall,
    Mine,
    Saw,
    Last
}