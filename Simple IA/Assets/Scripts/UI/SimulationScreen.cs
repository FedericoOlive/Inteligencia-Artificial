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
    [SerializeField] private PopulationManager populationManager;

    string timerText;

    // Start is called before the first frame update
    void Start()
    {
        timerText = timerTxt.text;

        pauseBtn.onClick.AddListener(OnPauseButtonClick);
        stopBtn.onClick.AddListener(OnStopButtonClick);
    }

    void OnPauseButtonClick()
    {
        populationManager.PauseSimulation();
    }

    void OnStopButtonClick()
    {
        //populationManager.StopSimulation();
        this.gameObject.SetActive(false);
        startConfigurationScreen.SetActive(true);
        for (int i = 0; i < panelDataVillager.Count; i++)
        {
            panelDataVillager[i]. lastGeneration = 0;
        }
    }
}