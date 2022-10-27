using System;
using UnityEngine;

public class BirdBase : MonoBehaviour
{
    public enum State
    {
        Alive,
        Dead
    }

    public State state
    {
        get; private set;
    }

    protected Genome genome;
    protected NeuralNetwork brain;
    protected BirdBehaviour birdBehaviour;

    private ObstacleBase lastObstacleBase;

    private void Awake()
    {
        birdBehaviour = GetComponent<BirdBehaviour>();
    }

    public void SetBrain(Genome genome, NeuralNetwork brain)
    {
        this.genome = genome;
        this.brain = brain;
        state = State.Alive;
        birdBehaviour.Reset();
        OnReset();
    }

    public void Flap()
    {
        if (state == State.Alive)
            birdBehaviour.Flap();
    }

    public void Think(float dt)
    {
        if (state == State.Alive)
        {
            ObstacleBase obstacleBase = ObstacleManager.Instance.GetNextObstacle(this.transform.position);

            if (obstacleBase == null)
                return;

            if (lastObstacleBase != obstacleBase)
            {
                if (lastObstacleBase != null)
                    AddFitnessByNearCenterOfObstacle();
                lastObstacleBase = obstacleBase;
            }

            OnThink(dt, birdBehaviour, obstacleBase);

            birdBehaviour.UpdateBird(dt);

            if (transform.position.y > 10f || transform.position.y < 0f || IsColliding(transform.position))
            {
                KillBird();
            }
        }
    }

    private void AddFitnessByNearCenterOfObstacle ()
    {
        float heightBird = transform.position.y;
        float distanceMiddleSafeZoneA = Mathf.Abs(lastObstacleBase.GetSafeZone().midSafeZoneA - heightBird);
        float distanceMiddleSafeZoneB = Mathf.Abs(lastObstacleBase.GetSafeZone().midSafeZoneB - heightBird);
        float nearDistance = 0;

        if (distanceMiddleSafeZoneA > distanceMiddleSafeZoneB)
        {
            nearDistance = 1 - Mathf.Log10(distanceMiddleSafeZoneB);
        }
        else
        {
            nearDistance = 1 - Mathf.Log10(distanceMiddleSafeZoneA);
        }

        if (nearDistance < 0)
            nearDistance = 1;

        genome.fitness += 5000 * (1 - nearDistance);
    }

    public bool IsColliding(Vector3 pos)
    {
        Collider2D coll = Physics2D.OverlapBox(pos, new Vector2(0.3f, 0.3f), 0);

        if (coll != null)
        {
            ObstacleBase obstacleBase = coll.GetComponentInParent<ObstacleBase>();
            if (obstacleBase)
                return !obstacleBase.birdsDisables.Contains(this);
            return true;
        }

        return false;
    }

    public void KillBird ()
    {
        OnDead();
        state = State.Dead;
    }

    protected virtual void OnDead()
    {

    }

    protected virtual void OnThink(float dt, BirdBehaviour birdBehaviour, ObstacleBase obstacleBase)
    {

    }

    protected virtual void OnReset()
    {

    }

}
