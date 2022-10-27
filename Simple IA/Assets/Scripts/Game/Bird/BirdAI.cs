using UnityEngine;

public class BirdAI : BirdBase
{
    protected override void OnThink (float dt, BirdBehaviour birdBehaviour, ObstacleBase obstacleBase)
    {
        Vector3 birdPos = birdBehaviour.transform.position;
        Vector3 obstaclePos = obstacleBase.transform.position;

        float[] inputs = new float[16];

        inputs[0] = birdPos.y;
        inputs[1] = obstacleBase.velocity;
        inputs[14] = obstacleBase.velocity;
        inputs[15] = obstacleBase.velocity;
        //inputs[1] = -5;
        //inputs[2] = 5;

        //inputs[3] = (obstaclePos - birdPos).x;
        //inputs[4] = (obstaclePos - birdPos).y;

        inputs[2] = obstacleBase.GetSafeZone().midSafeZoneA;
        inputs[3] = obstacleBase.GetSafeZone().distanceSafeZoneA;
        inputs[4] = obstacleBase.GetSafeZone().midSafeZoneB;
        inputs[5] = obstacleBase.GetSafeZone().distanceSafeZoneB;

        inputs[6] = obstacleBase.GetSafeZone().midSafeZoneA;
        inputs[7] = obstacleBase.GetSafeZone().distanceSafeZoneA;
        inputs[8] = obstacleBase.GetSafeZone().midSafeZoneB;
        inputs[9] = obstacleBase.GetSafeZone().distanceSafeZoneB;

        inputs[10] = obstacleBase.GetSafeZone().midSafeZoneA;
        inputs[11] = obstacleBase.GetSafeZone().distanceSafeZoneA;
        inputs[12] = obstacleBase.GetSafeZone().midSafeZoneB;
        inputs[13] = obstacleBase.GetSafeZone().distanceSafeZoneB;

        //inputs[8] = obstacleBase.IsDestroyable(this) ? 1 : 0;

        float[] outputs = brain.Synapsis(inputs);

        if (outputs[0] > 0.5f)
        {
            birdBehaviour.Flap();
        }

        //if (outputs[1] > 0.9f)
        //{
        //    if (birdBehaviour.Shoot())
        //    {
        //        genome.fitness *= 1000 * 3 * 10;
        //    }
        //    else
        //    {
        //        genome.fitness /= 1000 * (1 / 2.0f);
        //        if (genome.fitness < 0)
        //            genome.fitness = 0;
        //    }
        //}

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
