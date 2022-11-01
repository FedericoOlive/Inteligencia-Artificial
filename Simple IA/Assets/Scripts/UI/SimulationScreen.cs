using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationScreen : MonoBehaviour
{
    private List<UiPanelDataVillager> panelDataVillager = new List<UiPanelDataVillager>();

    public Text timerTxt;
    public Slider timerSlider;
    public Button pauseBtn;
    public Button stopBtn;
    public GameObject startConfigurationScreen;

    
    string timerText;

    // Start is called before the first frame update
    void Start()
    {
        timerSlider.onValueChanged.AddListener(OnTimerChange);
        timerText = timerTxt.text;

        timerTxt.text = string.Format(timerText, PopulationManager.Instance.IterationCount);

        pauseBtn.onClick.AddListener(OnPauseButtonClick);
        stopBtn.onClick.AddListener(OnStopButtonClick);
    }

    void OnTimerChange(float value)
    {
        PopulationManager.Instance.IterationCount = (int)value;
        timerTxt.text = string.Format(timerText, PopulationManager.Instance.IterationCount);
    }

    void OnPauseButtonClick()
    {
        PopulationManager.Instance.PauseSimulation();
    }

    void OnStopButtonClick()
    {
        PopulationManager.Instance.StopSimulation();
        this.gameObject.SetActive(false);
        startConfigurationScreen.SetActive(true);
        for (int i = 0; i < panelDataVillager.Count; i++)
        {
            panelDataVillager[i]. lastGeneration = 0;
        }
    }
}