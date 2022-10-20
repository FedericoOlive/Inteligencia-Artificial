using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadAndSaveGenome : MonoBehaviour
{
    [SerializeField] private Button btnLoadGenome;
    [SerializeField] private Button btnSaveGenome;

    void Start()
    {
        btnLoadGenome.onClick.AddListener(LoadGenome);
        btnSaveGenome.onClick.AddListener(SaveGenome);
    }

    private void SaveGenome()
    {

    }

    private void LoadGenome()
    {

    }
}