using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationScreen : MonoBehaviour
{
    private List<UiPanelDataVillager> panelDataVillager = new List<UiPanelDataVillager>();

    public Text timerTxt;
    public Slider timerSlider;
    public Text timeElapsed;
    public Button pauseBtn;
    public Button stopBtn;
    public GameObject startConfigurationScreen;

    string timerText;

    private bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        timerText = timerTxt.text;

        pauseBtn.onClick.AddListener(OnPauseButtonClick);
        stopBtn.onClick.AddListener(OnStopButtonClick);
        timerSlider.onValueChanged.AddListener(UpdateDeltaTime);
    }

    private void OnEnable ()
    {
        isRunning = true;
    }

    private void OnDisable ()
    {
        isRunning = false;
    }

    private void Update ()
    {
        timeElapsed.text = string.Format(timerText, GameManager.Get().accumTime.ToString("F2"));
    }

    void UpdateDeltaTime (float value)
    {
        Time.timeScale = value;
        timerTxt.text = string.Format(timerText, value.ToString("F2"));
    }

    void OnPauseButtonClick ()
    {
        isRunning = !isRunning;

        for (int i = 0; i < panelDataVillager.Count; i++)
        {
            panelDataVillager[i].populationManager.PauseSimulation(isRunning);
        }

        Time.timeScale = isRunning ? 1 : 0;
    }

    void OnStopButtonClick ()
    {
        //populationManager.StopSimulation();
        this.gameObject.SetActive(false);
        startConfigurationScreen.SetActive(true);
        for (int i = 0; i < panelDataVillager.Count; i++)
        {
            panelDataVillager[i].lastGeneration = 0;
        }
    }
}