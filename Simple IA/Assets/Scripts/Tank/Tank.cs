using UnityEngine;

public class Tank : TankBase
{
    float fitness = 0;

    protected override void OnReset ()
    {
        fitness = 1;
    }

    protected override void OnThink (float dt)
    {
        Vector3 dirToTeamMine = GetDirToMine(goodMine);
        Vector3 dirToEnemyMine = GetDirToMine(badMine);
        Vector3 dir = transform.forward;

        inputs[0] = dir.x;
        inputs[1] = dir.z;
        inputs[2] = dirToTeamMine.x;
        inputs[3] = dirToTeamMine.z;
        inputs[4] = dirToEnemyMine.z;
        inputs[5] = dirToEnemyMine.z;

        float[] output = brain.Synapsis(inputs);

        SetForces(output[0], output[1], dt);
    }

    protected override void OnTakeMine (Mine mine)
    {
        if (team == mine.team)
        {
            fitness *= 2;
            genome.fitness = fitness;
        }
    }

}