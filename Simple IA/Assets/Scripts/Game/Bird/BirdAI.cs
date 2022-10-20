using UnityEngine;

public class BirdAI : BirdBase
{
    protected override void OnThink (float dt, BirdBehaviour birdBehaviour, ObstacleBase obstacleBase)
    {
        Vector3 birdPos = birdBehaviour.transform.position;
        Vector3 obstaclePos = obstacleBase.transform.position;

        float[] inputs = new float[12];
        inputs[1] = birdPos.y;
        inputs[2] = (float) obstacleBase.obstacleType + 1;
        inputs[3] = obstaclePos.y;
        inputs[4] = (obstaclePos - birdPos).x;
        inputs[5] = (obstaclePos - birdPos).y;
        inputs[6] = obstacleBase.velocity;
        inputs[7] = obstacleBase.maxPos;
        inputs[8] = obstacleBase.minPos;
        inputs[9] = obstacleBase.IsDestroyable(this) ? 1 : 0;

        float[] outputs = brain.Synapsis(inputs);

        if (outputs[0] > 0.5f)
        {
            birdBehaviour.Flap();
        }

        if (outputs[1] > 0.5f)
        {
            if (birdBehaviour.Shoot())
            {
                genome.fitness *= 2;
            }
            else
            {
                genome.fitness *= 3/5.0f;
            }
        }

        genome.fitness += Time.deltaTime * 1000;
    }



    protected override void OnDead()
    {

    }

    protected override void OnReset()
    {
        genome.fitness = 0.0f;
    }
}
