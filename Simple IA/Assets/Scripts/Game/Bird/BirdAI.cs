using UnityEngine;

public class BirdAI : BirdBase
{
    protected override void OnThink (float dt, BirdBehaviour birdBehaviour, ObstacleBase obstacleBase)
    {
        float[] inputs = new float[6];
        inputs[0] = (int) obstacleBase.obstacleType / (float) ObstacleType.Last;
        inputs[1] = (obstacleBase.transform.position - birdBehaviour.transform.position).x / 10.0f;
        inputs[2] = (obstacleBase.transform.position - birdBehaviour.transform.position).y / 10.0f;
        inputs[3] = obstacleBase.velocity;
        inputs[4] = obstacleBase.maxPos;
        inputs[5] = obstacleBase.minPos;

        float[] outputs;
        outputs = brain.Synapsis(inputs);

        if (outputs[0] > 0.5f)
        {
            birdBehaviour.Flap();
        }

        if (Vector3.Distance(obstacleBase.transform.position, birdBehaviour.transform.position) <= 1.0f)
        {
            genome.fitness *= 2;
        }

        genome.fitness += (100.0f - Vector3.Distance(obstacleBase.transform.position, birdBehaviour.transform.position));
    }

    protected override void OnDead()
    {

    }

    protected override void OnReset()
    {
        genome.fitness = 0.0f;
    }
}
