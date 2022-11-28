using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartConfigurationScreen : MonoBehaviour
{
    [SerializeField] private PopulationManager populationManager;
    [SerializeField] private DataPopulation dataPopulation;
    [SerializeField] private LevelSettings levelSettings;
    [Space(15)]
    public Text populationCountTxt;
    public Slider populationCountSlider;
    public Text FoodsCountTxt;
    public Slider FoodsCountSlider;
    public Text generationDurationTxt;
    public Slider generationDurationSlider;
    public Text eliteCountTxt;
    public Slider eliteCountSlider;
    public Text mutationChanceTxt;
    public Slider mutationChanceSlider;
    public Text mutationRateTxt;
    public Slider mutationRateSlider;
    public Text hiddenLayersCountTxt;
    public Slider hiddenLayersCountSlider;
    public Text neuronsPerHLCountTxt;
    public Slider neuronsPerHLSlider;
    public Text biasTxt;
    public Slider biasSlider;
    public Text sigmoidSlopeTxt;
    public Slider sigmoidSlopeSlider;
    public Button startButton;
    public Button loadButton;
    public List<Button> saveButton;
    public GameObject simulationScreen;
    
    string populationText;
    string elitesText;
    string mutationChanceText;
    string mutationRateText;
    string hiddenLayersCountText;
    string biasText;
    string sigmoidSlopeText;
    string neuronsPerHLCountText;

    void Start()
    {   
        populationCountSlider.onValueChanged.AddListener(OnPopulationCountChange);
        eliteCountSlider.onValueChanged.AddListener(OnEliteCountChange);
        mutationChanceSlider.onValueChanged.AddListener(OnMutationChanceChange);
        mutationRateSlider.onValueChanged.AddListener(OnMutationRateChange);
        hiddenLayersCountSlider.onValueChanged.AddListener(OnHiddenLayersCountChange);
        neuronsPerHLSlider.onValueChanged.AddListener(OnNeuronsPerHLChange);
        biasSlider.onValueChanged.AddListener(OnBiasChange);
        sigmoidSlopeSlider.onValueChanged.AddListener(OnSigmoidSlopeChange);

        populationText = populationCountTxt.text;
        elitesText = eliteCountTxt.text;
        mutationChanceText = mutationChanceTxt.text;
        mutationRateText = mutationRateTxt.text;
        hiddenLayersCountText = hiddenLayersCountTxt.text;
        neuronsPerHLCountText = neuronsPerHLCountTxt.text;
        biasText = biasTxt.text;
        sigmoidSlopeText = sigmoidSlopeTxt.text;

        populationCountSlider.value = dataPopulation.populationCount;
        eliteCountSlider.value = dataPopulation.eliteCount;
        mutationChanceSlider.value = dataPopulation.mutationChance * 100.0f;
        mutationRateSlider.value = dataPopulation.mutationRate * 100.0f;
        hiddenLayersCountSlider.value = dataPopulation.hiddenLayers;
        neuronsPerHLSlider.value = dataPopulation.neuronsCountPerHL;
        biasSlider.value = -dataPopulation.bias;
        sigmoidSlopeSlider.value = dataPopulation.p;

        startButton.onClick.AddListener(OnStartButtonClick);        
        loadButton.onClick.AddListener(OnLoad);
        for (int i = 0; i < saveButton.Count; i++)
        {
            int index = i;
            saveButton[i].onClick.AddListener(() => OnSave(index));
        }
    }

    void OnPopulationCountChange(float value)
    {
        dataPopulation.populationCount = (int)value;
        populationCountTxt.text = string.Format(populationText, dataPopulation.populationCount);
    }

    void OnEliteCountChange(float value)
    {
        dataPopulation.eliteCount = (int)value;
        eliteCountTxt.text = string.Format(elitesText, dataPopulation.eliteCount);
    }

    void OnMutationChanceChange(float value)
    {
        dataPopulation.mutationChance = value / 100.0f;
        mutationChanceTxt.text = string.Format(mutationChanceText, (int)(dataPopulation.mutationChance * 100));
    }

    void OnMutationRateChange(float value)
    {
        dataPopulation.mutationRate = value / 100.0f;
        mutationRateTxt.text = string.Format(mutationRateText, (int)(dataPopulation.mutationRate * 100));
    }

    void OnHiddenLayersCountChange(float value)
    {
        dataPopulation.hiddenLayers = (int)value;
        hiddenLayersCountTxt.text = string.Format(hiddenLayersCountText, dataPopulation.hiddenLayers);
    }

    void OnNeuronsPerHLChange(float value)
    {
        dataPopulation.neuronsCountPerHL = (int)value;
        neuronsPerHLCountTxt.text = string.Format(neuronsPerHLCountText, dataPopulation.neuronsCountPerHL);
    }

    void OnBiasChange(float value)
    {
        dataPopulation.bias = -value;
        biasTxt.text = string.Format(biasText, dataPopulation.bias.ToString("0.00"));
    }

    void OnSigmoidSlopeChange(float value)
    {
        dataPopulation.p = value;
        sigmoidSlopeTxt.text = string.Format(sigmoidSlopeText, dataPopulation.p.ToString("0.00"));
    }

    void OnStartButtonClick()
    {
        GameManager.Get().Init();
        this.gameObject.SetActive(false);
        simulationScreen.SetActive(true);
    }

    void OnLoad()
    {
        GameManager.Get().Init();
        this.gameObject.SetActive(false);
        simulationScreen.SetActive(true);

        GameManager.Get().LoadData();
    }

    void OnSave(int i)
    {
        GameManager.Get().SaveData(i);
    }
}