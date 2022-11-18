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
        Vector3 posFood = targetFood.transform.position;
        Vector3 pos = transform.position;

        Vector3 distanceToFood = pos - posFood;
        Vector3 enemyDistanceToFood = pos - posFood;
        float enemyLifeNearTargetFood = 0;

        inputs[0] = distanceToFood.x;
        inputs[1] = distanceToFood.z;
        //inputs[2] = life;
        //inputs[3] = enemyDistanceToFood.x;
        //inputs[4] = enemyDistanceToFood.z;
        //inputs[5] = enemyLifeNearTargetFood;
        //inputs[6] = allyDistanceToFood.x;
        //inputs[7] = allyDistanceToFood.z;
        //inputs[8] = allyLifeNearTargetFood;

        float[] output = brain.Synapsis(inputs);

        if (output[0] > 0.8f)
        {
            TryEatOrRun();
        }
        else if (output[0] > 0.6f)
        {
            TryEatAndFight();
        }
        //else      // Descomentar en caso de querer que no se mueva cuando intente comer
        {
            Direction nextDir = Direction.None;

                 if (output[1] > 0.8f) nextDir = Direction.Up;
            else if (output[1] > 0.6f) nextDir = Direction.Right;
            else if (output[1] > 0.4f) nextDir = Direction.Down;
            else if (output[1] > 0.2f) nextDir = Direction.Left;

            SetDirection(nextDir);
        }
    }

    private void TryEatOrRun()
    {
        if (IsInPositionFood())
        {
            stateAttack = StateAttack.EatOrRun;
        }
        else
        {
            stateAttack = StateAttack.None;
        }
    }

    private void TryEatAndFight()
    {
        if (IsInPositionFood())
        {
            stateAttack = StateAttack.FightAndEat;
        }
        else
        {
            stateAttack = StateAttack.None;
        }
    }

    bool IsInPositionFood ()
    {
        Vector3 pos = transform.position;
        Vector3 posNearFood = GameManager.Get().GetNearFood(pos).transform.position;
        return posNearFood == pos;
    }

    public override void TakeFood (Food food)
    {
        bool eatingFood = food != null;

        if (eatingFood)
        {
            food.EatFood();
            foodsEatsInGeneration++;
            fitness += 1000;
            genome.fitness = fitness;
            life += 50;
        }
        else // Eating Grass
        {
            fitness -= 10;
            genome.fitness = fitness;
        }
    }

    private Villager GetEnemyDistanceToTargetFood ()
    {
        //targetFood

        return null;
    }

    public void GoBackPosition ()
    {
        transform.position = lastPosition;
    }
}