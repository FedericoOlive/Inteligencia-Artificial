﻿using UnityEngine;

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

            OnThink(dt, birdBehaviour, obstacleBase);

            birdBehaviour.UpdateBird(dt);

            if (this.transform.position.y > 5f || this.transform.position.y < -5f || ObstacleManager.Instance.IsColliding(this.transform.position))
            {
                KillBird();
            }
        }
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
