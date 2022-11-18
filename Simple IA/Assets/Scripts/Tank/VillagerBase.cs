using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class VillagerBase : MonoBehaviour
{
    public event Action OnKilled;
    public Vector3 lastPosition = Vector3.one * -100;
    public Team team;
    public int life;
    protected Genome genome;
    protected NeuralNetwork brain;
    public Food targetFood;
    protected float[] inputs;
    public MeshRenderer[] meshRenderer;
    public bool fightForFood;
    public StateAttack stateAttack;
    public int generationsAlive = 3;
    public int foodsEatsInGeneration;

    private void Awake ()
    {
        life = Random.Range(50, 150);
    }

    public void SetBrain (Genome genome, NeuralNetwork brain)
    {
        this.genome = genome;
        this.brain = brain;
        inputs = new float[brain.InputsCount];
        OnReset();
    }

    public void SetNearestFood (Food food)
    {
        targetFood = food;
    }

    protected bool IsCloseToFood (GameObject food)
    {
        return (transform.position == food.transform.position);
    }

    protected void SetDirection (Direction dir)
    {
        if (lastPosition != transform.position)
            lastPosition = transform.position;
        Vector3 nextPos = transform.position;

        switch (dir)
        {
            case Direction.Up:
                nextPos.x += 1;
                break;
            case Direction.Down:
                nextPos.x -= 1;
                break;
            case Direction.Left:
                nextPos.z -= 1;
                break;
            case Direction.Right:
                nextPos.z += 1;
                break;
        }

        if (TerrainGenerator.IsPositionInsideBounds(nextPos))
        {
            if (nextPos.x > TerrainGenerator.maxPos.x)
                nextPos.x = TerrainGenerator.minPos.x;
            if (nextPos.x < TerrainGenerator.minPos.x)
                nextPos.x = TerrainGenerator.maxPos.x;

            transform.position = nextPos;
        }
    }

    public void Think (float dt)
    {
        OnThink(dt);
    }

    public void Kill ()
    {
        OnKilled?.Invoke();
    }

    protected virtual void OnThink (float dt) { }
    public virtual void TakeFood (Food food) { }
    protected virtual void OnReset () { }
}

public enum Direction
{
    None,
    Up,
    Down,
    Left,
    Right
}

public enum StateAttack
{
    None,
    EatOrRun,
    FightAndEat
}