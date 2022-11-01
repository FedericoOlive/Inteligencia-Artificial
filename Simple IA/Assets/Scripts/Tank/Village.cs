using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Village
{
    public Team team;
    private Color colorCiv = Color.white;

    public List<Tank> populationGOs = new List<Tank>();
    public List<Genome> population = new List<Genome>();
    public List<NeuralNetwork> brains = new List<NeuralNetwork>();

    public int generation;

    public float bestFitness;
    public float avgFitness;
    public float worstFitness;




    public void ResetFitness ()
    {
        for (int i = 0; i < population.Count; i++)
        {
            population[i].fitness = 0;
        }
    }

    public void SetTeam (int i)
    {
        team = (Team) i;

        colorCiv = GetColorCiv(team);
    }

    public static Color GetColorCiv (Team team)
    {
        switch (team)
        {
            case Team.Blue:
                return Color.blue;
            case Team.Red:
                return Color.red;
            case Team.Yellow:
                return Color.yellow;
            case Team.Green:
                return Color.green;
            case Team.Magenta:
                return Color.magenta;
        }

        return Color.black;
    }

    public void SetTanks ()
    {
        for (int i = 0; i < populationGOs.Count; i++)
        {
            populationGOs[i].team = team;
            for (int j = 0; j < populationGOs[i].meshRenderer.Length; j++)
            {
                populationGOs[i].meshRenderer[j].material.color = colorCiv;
            }
        }
    }

    public void OnDrawGizmos ()
    {
        Gizmos.color = colorCiv;
        for (int i = 0; i < populationGOs.Count; i++)
        {
            if (populationGOs[i].nearMine)
                Gizmos.DrawLine(populationGOs[i].transform.position, populationGOs[i].nearMine.transform.position);
        }

        Gizmos.color = Color.white;
    }
}