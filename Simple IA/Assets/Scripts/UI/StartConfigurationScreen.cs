using UnityEngine;
using UnityEngine.UI;

public class StartConfigurationScreen : MonoBehaviour
{
    [SerializeField] private PopulationManager populationManager;
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
    public GameObject simulationScreen;
    
    string populationText;
    string FoodsText;
    string generationDurationText;
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
        FoodsCountSlider.onValueChanged.AddListener(OnFoodsCountChange);
        generationDurationSlider.onValueChanged.AddListener(OnGenerationDurationChange);
        eliteCountSlider.onValueChanged.AddListener(OnEliteCountChange);
        mutationChanceSlider.onValueChanged.AddListener(OnMutationChanceChange);
        mutationRateSlider.onValueChanged.AddListener(OnMutationRateChange);
        hiddenLayersCountSlider.onValueChanged.AddListener(OnHiddenLayersCountChange);
        neuronsPerHLSlider.onValueChanged.AddListener(OnNeuronsPerHLChange);
        biasSlider.onValueChanged.AddListener(OnBiasChange);
        sigmoidSlopeSlider.onValueChanged.AddListener(OnSigmoidSlopeChange);

        populationText = populationCountTxt.text;
        FoodsText = FoodsCountTxt.text;
        generationDurationText = generationDurationTxt.text;
        elitesText = eliteCountTxt.text;
        mutationChanceText = mutationChanceTxt.text;
        mutationRateText = mutationRateTxt.text;
        hiddenLayersCountText = hiddenLayersCountTxt.text;
        neuronsPerHLCountText = neuronsPerHLCountTxt.text;
        biasText = biasTxt.text;
        sigmoidSlopeText = sigmoidSlopeTxt.text;

        populationCountSlider.value = populationManager.PopulationCount;
        FoodsCountSlider.value = populationManager.FoodsCount;
        generationDurationSlider.value = populationManager.GenerationDuration;
        eliteCountSlider.value = populationManager.EliteCount;
        mutationChanceSlider.value = populationManager.MutationChance * 100.0f;
        mutationRateSlider.value = populationManager.MutationRate * 100.0f;
        hiddenLayersCountSlider.value = populationManager.HiddenLayers;
        neuronsPerHLSlider.value = populationManager.NeuronsCountPerHL;
        biasSlider.value = -populationManager.Bias;
        sigmoidSlopeSlider.value = populationManager.P;

        startButton.onClick.AddListener(OnStartButtonClick);        
    }

    void OnPopulationCountChange(float value)
    {
        populationManager.PopulationCount = (int)value;
        
        populationCountTxt.text = string.Format(populationText, populationManager.PopulationCount);
    }

    void OnFoodsCountChange(float value)
    {
        populationManager.FoodsCount = (int)value;        

        FoodsCountTxt.text = string.Format(FoodsText, populationManager.FoodsCount);
    }

    void OnGenerationDurationChange(float value)
    {
        populationManager.GenerationDuration = (int)value;
        
        generationDurationTxt.text = string.Format(generationDurationText, populationManager.GenerationDuration);
    }

    void OnEliteCountChange(float value)
    {
        populationManager.EliteCount = (int)value;

        eliteCountTxt.text = string.Format(elitesText, populationManager.EliteCount);
    }

    void OnMutationChanceChange(float value)
    {
        populationManager.MutationChance = value / 100.0f;

        mutationChanceTxt.text = string.Format(mutationChanceText, (int)(populationManager.MutationChance * 100));
    }

    void OnMutationRateChange(float value)
    {
        populationManager.MutationRate = value / 100.0f;

        mutationRateTxt.text = string.Format(mutationRateText, (int)(populationManager.MutationRate * 100));
    }

    void OnHiddenLayersCountChange(float value)
    {
        populationManager.HiddenLayers = (int)value;
        

        hiddenLayersCountTxt.text = string.Format(hiddenLayersCountText, populationManager.HiddenLayers);
    }

    void OnNeuronsPerHLChange(float value)
    {
        populationManager.NeuronsCountPerHL = (int)value;

        neuronsPerHLCountTxt.text = string.Format(neuronsPerHLCountText, populationManager.NeuronsCountPerHL);
    }

    void OnBiasChange(float value)
    {
        populationManager.Bias = -value;

        biasTxt.text = string.Format(biasText, populationManager.Bias.ToString("0.00"));
    }

    void OnSigmoidSlopeChange(float value)
    {
        populationManager.P = value;

        sigmoidSlopeTxt.text = string.Format(sigmoidSlopeText, populationManager.P.ToString("0.00"));
    }


    void OnStartButtonClick()
    {
        populationManager.StartSimulation();
        this.gameObject.SetActive(false);
        simulationScreen.SetActive(true);
    }
    
}
