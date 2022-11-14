using UnityEngine;
using UnityEngine.UI;

public class UiPanelDataVillager : MonoBehaviour
{
    public int indexVillage;
    public int lastGeneration = 0;
    
    public Text generationsCountTxt;
    public Text bestFitnessTxt;
    public Text avgFitnessTxt;
    public Text worstFitnessTxt;

    private string generationsCountText;
    private string bestFitnessText;
    private string avgFitnessText;
    private string worstFitnessText;
    [SerializeField] private PopulationManager populationManager;

    private void Start ()
    {
        if (string.IsNullOrEmpty(generationsCountText))
            generationsCountText = generationsCountTxt.text;
        if (string.IsNullOrEmpty(bestFitnessText))
            bestFitnessText = bestFitnessTxt.text;
        if (string.IsNullOrEmpty(avgFitnessText))
            avgFitnessText = avgFitnessTxt.text;
        if (string.IsNullOrEmpty(worstFitnessText))
            worstFitnessText = worstFitnessTxt.text;
    }

    void OnEnable ()
    {
        if (populationManager.village.Count <= indexVillage)
        {
            gameObject.SetActive(false);
            return;
        }

        generationsCountTxt.color = Village.GetColorCiv((Team) indexVillage);

        if (string.IsNullOrEmpty(generationsCountText))
            generationsCountText = generationsCountTxt.text;
        if (string.IsNullOrEmpty(bestFitnessText))
            bestFitnessText = bestFitnessTxt.text;
        if (string.IsNullOrEmpty(avgFitnessText))
            avgFitnessText = avgFitnessTxt.text;
        if (string.IsNullOrEmpty(worstFitnessText))
            worstFitnessText = worstFitnessTxt.text;

        generationsCountTxt.text = string.Format(generationsCountText, 0);
        bestFitnessTxt.text = string.Format(bestFitnessText, 0);
        avgFitnessTxt.text = string.Format(avgFitnessText, 0);
        worstFitnessTxt.text = string.Format(worstFitnessText, 0);
    }

    void Update()
    {
        lastGeneration = populationManager.village[indexVillage].generation;
        generationsCountTxt.text = string.Format(generationsCountText, populationManager.village[indexVillage].generation);
        bestFitnessTxt.text = string.Format(bestFitnessText, populationManager.village[indexVillage].bestFitness);
        avgFitnessTxt.text = string.Format(avgFitnessText, populationManager.village[indexVillage].avgFitness);
        worstFitnessTxt.text = string.Format(worstFitnessText, populationManager.village[indexVillage].worstFitness);
    }
}