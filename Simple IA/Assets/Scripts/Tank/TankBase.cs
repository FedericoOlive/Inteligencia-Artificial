using System;
using UnityEngine;

public class TankBase : MonoBehaviour
{
    public Team team;
    public float Speed = 10.0f;
    public float RotSpeed = 20.0f;

    protected Genome genome;
	protected NeuralNetwork brain;
    public Mine nearMine;
    protected Mine goodMine;
    protected Mine badMine;
    protected float[] inputs;
    public MeshRenderer[] meshRenderer;
    
    public void SetBrain(Genome genome, NeuralNetwork brain)
    {
        this.genome = genome;
        this.brain = brain;
        inputs = new float[brain.InputsCount];
        OnReset();
    }

    public void SetNearestMine(Mine mine)
    {
        nearMine = mine;
    }

    public void SetGoodNearestMine(Mine mine)
    {
        goodMine = mine;
    }

    public void SetBadNearestMine(Mine mine)
    {
        badMine = mine;
    }
    
    protected Vector3 GetDirToMine(Mine mine)
    {
        return (mine.transform.position - this.transform.position).normalized;
    }
    
    protected bool IsCloseToMine(GameObject mine)
    {
        return (transform.position - mine.transform.position).sqrMagnitude <= 2.0f;
    }

    protected void SetForces(float leftForce, float rightForce, float dt)
    {
        Vector3 pos = this.transform.position;
        float rotFactor = Mathf.Clamp((rightForce - leftForce), -1.0f, 1.0f);
        this.transform.rotation *= Quaternion.AngleAxis(rotFactor * RotSpeed * dt, Vector3.up);
        pos += this.transform.forward * Mathf.Abs(rightForce + leftForce) * 0.5f * Speed * dt;
        this.transform.position = pos;
    }

	public void Think(float dt) 
	{
        OnThink(dt);

        if(IsCloseToMine(nearMine.gameObject))
        {
            OnTakeMine(nearMine);
            PopulationManager.Instance.RelocateMine(nearMine.gameObject);
        }
	}

    protected virtual void OnThink(float dt)
    {

    }

    protected virtual void OnTakeMine(Mine mine)
    {
    }

    protected virtual void OnReset()
    {

    }
}
