using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Village
{
    private Color colorCiv = Color.white;

    public List<Villager> populationGOs = new List<Villager>();
    public List<Genome> population = new List<Genome>();
    public List<NeuralNetwork> brains = new List<NeuralNetwork>();

    public int generation;

    public float populationFitness;
    public float bestFitness;
    public float avgFitness;
    public float worstFitness;

    public void UpdateFitness ()
    {
        populationFitness = 0;
        bestFitness = float.MinValue;
        avgFitness = 0;
        worstFitness = float.MaxValue;

        for (int i = 0; i < population.Count; i++)
        {
            float currentFitness = population[i].fitness;

            if (currentFitness > bestFitness)
                bestFitness = currentFitness;

            if (currentFitness < worstFitness)
                worstFitness = currentFitness;

            populationFitness += currentFitness;
        }

        avgFitness = populationFitness / population.Count;
    }

    public void ResetFitness ()
    {
        for (int i = 0; i < population.Count; i++)
        {
            population[i].fitness = 1;
        }

        populationFitness = 0;
        bestFitness = 0;
        avgFitness = 0;
        worstFitness = 0;
    }

    public void SetColorCivTeam (Team team)
    {
        colorCiv = GetColorCiv(team);
    }

    public static Color GetColorCiv (Team team)
    {
        switch (team)
        {
            case Team.Red:
                return Color.red;
            case Team.Magenta:
                return Color.magenta;
            case Team.Blue:
                return Color.blue;
            case Team.Green:
                return Color.green;
        }

        return Color.black;
    }

    public void SetVillager ()
    {
        for (int i = 0; i < populationGOs.Count; i++)
        {
            for (int j = 0; j < populationGOs[i].meshRenderer.Length; j++)
            {
                populationGOs[i].meshRenderer[j].material.color = colorCiv;
            }
        }
    }

    public void SetColorVillager (Villager villager)
    {
        for (int j = 0; j < villager.meshRenderer.Length; j++)
        {
            villager.meshRenderer[j].material.color = colorCiv;
        }
    }

#if UNITY_EDITOR
    public void OnDrawGizmos ()
    {
        Gizmos.color = colorCiv;
        for (int i = 0; i < populationGOs.Count; i++)
        {
            if (populationGOs[i].targetFood)
                Gizmos.DrawLine(populationGOs[i].transform.position, populationGOs[i].targetFood.transform.position);
        }

        Gizmos.color = Color.white;
    }
#endif
}