using System.Collections.Generic;
using UnityEngine;

public abstract class ObstacleBase : MonoBehaviour
{
    public event System.Action<ObstacleBase> OnDestroy;
    public ObstacleType obstacleType;
    [SerializeField] protected Transform up;
    [SerializeField] protected Transform down;

    public float maxPos;
    public float minPos;
    public float velocity;
    public bool destroyable;
    public List<BirdBase> birdsDisables = new List<BirdBase>();
    [SerializeField] protected SafeZone safeZone;

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

    public bool IsDestroyable (BirdBase birdBase)
    {
        return !birdsDisables.Contains(birdBase);
    }

    public SafeZone GetSafeZone () => safeZone;

    protected void SetSafeZone ()
    {
        safeZone.SetSafeZone();
    }
}

[System.Serializable]
public struct SafeZone
{
    public float a1;
    public float a2;
    public float b1;
    public float b2;

    public void SetSafeZone ()
    {
        midSafeZoneA = (a1 + a2) / 2;
        midSafeZoneB = (b1 + b2) / 2;
        distanceSafeZoneA = a1 - a2;
        distanceSafeZoneB = b1 - b2;
    }

    public float midSafeZoneA;
    public float midSafeZoneB;
    public float distanceSafeZoneA;
    public float distanceSafeZoneB;
}

public enum ObstacleType
{
    Wall,
    Mine,
    Saw,
    Last
}