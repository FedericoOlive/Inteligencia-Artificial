using System;
using UnityEngine;

public class VillagerBase : MonoBehaviour
{
    public Team team;

    protected Genome genome;
    protected NeuralNetwork brain;
    public Food nearFood;
    protected float[] inputs;
    public MeshRenderer[] meshRenderer;

    public void SetBrain (Genome genome, NeuralNetwork brain)
    {
        this.genome = genome;
        this.brain = brain;
        inputs = new float[brain.InputsCount];
        OnReset();
    }

    public void SetNearestFood (Food food)
    {
        nearFood = food;
    }

    protected Vector3 GetDirToFood (Food food)
    {
        return (food.transform.position - this.transform.position).normalized;
    }

    protected bool IsCloseToFood (GameObject food)
    {
        return (transform.position - food.transform.position).sqrMagnitude <= 2.0f;
    }
    
    protected void SetDirection (Direction dir)
    {
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
            transform.position = nextPos;
    }

    public void Think (float dt)
    {
        OnThink(dt);

        if (IsCloseToFood(nearFood.gameObject))
        {
            OnTakeFood(nearFood);
            //PopulationManager.Instance.RelocateFood(nearFood.gameObject);
        }
    }

    protected virtual void OnThink (float dt)
    {

    }

    protected virtual void OnTakeFood (Food food)
    {
    }

    protected virtual void OnReset ()
    {

    }
}

public enum Direction
{
    None,
    Up,
    Down,
    Left,
    Right
}