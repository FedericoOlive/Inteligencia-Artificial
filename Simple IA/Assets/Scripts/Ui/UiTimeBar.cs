using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleIA
{
    public class UiTimeBar : MonoBehaviour
    {
        [SerializeField] private Ant aiController;
        [SerializeField] private Image currentBar;
        [SerializeField] private TMP_Text textState;

        void Update ()
        {
            //textState.text = aiController.GetNameCurrentState();
            //currentBar.fillAmount = aiController.GetCurrentLerpTime();
        }
    }
}