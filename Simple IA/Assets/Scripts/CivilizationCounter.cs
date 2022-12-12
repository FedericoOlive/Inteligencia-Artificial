using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CivilizationCounter : MonoBehaviour
{
    [SerializeField] private List<GameObject> canvasShower = new List<GameObject>();
    [SerializeField] private Slider sliderCivCounts;
    [SerializeField] private LevelSettings levelSettings;
    [SerializeField] private Button btnStart;
    [SerializeField] private Button btnPause;
    [SerializeField] private GameObject panelConfiguration;
    private bool isPaused;

    void Start ()
    {
        ChangeCivAmount((int) sliderCivCounts.value);
        sliderCivCounts.onValueChanged.AddListener(ChangeCivAmount);
        btnStart.onClick.AddListener(OnStart);
        btnPause.onClick.AddListener(OnPause);
    }

    private void ChangeCivAmount (float civilizations)
    {
        levelSettings.maxCivilizations = (int) civilizations;
        for (int i = 0; i < canvasShower.Count; i++)
        {
            if (i < civilizations)
                canvasShower[i].SetActive(true);
            else
                canvasShower[i].SetActive(false);
        }
    }

    void OnStart ()
    {
        GameManager.Get().Init();
        SetConfigurationPanel(false);
    }

    void OnPause ()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    void SetConfigurationPanel (bool isEnable)
    {
        panelConfiguration.SetActive(isEnable);
        btnStart.gameObject.SetActive(isEnable);
        sliderCivCounts.gameObject.SetActive(isEnable);
    }
}