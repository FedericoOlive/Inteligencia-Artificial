using UnityEngine;

public class AntSelectionRepresentationUi : MonoBehaviour
{
    [SerializeField] private RectTransform rectSelectionUnits;
    private Vector2 initialMousePos;
    private Vector2 currentMousePos;
    private Vector4 corners;

    private void OnEnable ()
    {
        initialMousePos = Input.mousePosition;
        currentMousePos = initialMousePos;
    }

    void Update ()
    {
        currentMousePos = Input.mousePosition;
        CalculateCorners();
        SetRectangle();
    }

    void CalculateCorners ()
    {
        corners.x = Mathf.Min(initialMousePos.x, currentMousePos.x);
        corners.y = Mathf.Min(initialMousePos.y, currentMousePos.y);
        corners.z = Mathf.Max(initialMousePos.x, currentMousePos.x) - corners.x;
        corners.w = Mathf.Max(initialMousePos.y, currentMousePos.y) - corners.y;
    }

    void SetRectangle ()
    {
        rectSelectionUnits.anchoredPosition = new Vector2(corners.x, corners.y);
        rectSelectionUnits.sizeDelta = new Vector2(corners.z, corners.w);
    }
}