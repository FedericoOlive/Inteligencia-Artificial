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
            //SetTextCurrentAction(aiController.GetNameCurrentState());
            //SetBarFill(aiController.GetCurrentLerpTime());
        }

        void SetTextCurrentAction (string text)
        {
            textState.text = text;
        }

        void SetBarFill (float lerp)
        {
            currentBar.fillAmount = lerp;
        }
    }
}