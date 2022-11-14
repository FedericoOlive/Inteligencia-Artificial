using UnityEngine;

public class Villager : VillagerBase
{
    private float fitness = 0;

    protected override void OnReset ()
    {
        fitness = 1;
    }

    protected override void OnThink (float dt)
    {
        Vector3 posFood = nearFood.transform.position;
        Vector3 pos = transform.position;

        inputs[0] = pos.x;
        inputs[1] = pos.z;
        inputs[2] = posFood.x;
        inputs[3] = posFood.z;

        float[] output = brain.Synapsis(inputs);

        Direction dir = GetDirection(output[0], output[1], output[2], output[3], output[4]);
        SetDirection(dir);
    }

    protected override void OnTakeFood (Food food)
    {
        if (team == food.team)
        {
            fitness *= 2;
            genome.fitness = fitness;
        }
    }
    
    private Direction GetDirection (float forceUp, float forceDown, float forceLeft, float forceRight, float forceStandBy)
    {
        Direction dir = Direction.None;

        if (forceUp > forceDown && forceUp > forceLeft && forceUp > forceRight && forceUp > forceStandBy)
            return Direction.Up;
        if (forceDown > forceUp && forceDown > forceLeft && forceDown > forceRight && forceDown > forceStandBy)
            return Direction.Down;
        if (forceLeft > forceUp && forceLeft > forceDown && forceLeft > forceRight && forceLeft > forceStandBy)
            return Direction.Left;
        if (forceRight > forceUp && forceRight > forceDown && forceRight > forceLeft && forceRight > forceStandBy)
            return Direction.Right;
        if (forceStandBy > forceUp && forceStandBy > forceDown && forceStandBy > forceLeft && forceStandBy > forceRight)
            return Direction.None;

        return dir;
    }
}