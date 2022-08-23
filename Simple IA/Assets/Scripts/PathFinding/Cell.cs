using UnityEngine;

[ExecuteAlways]
public class Cell : MonoBehaviour
{
    private CellType cellType;
    private MeshRenderer meshRenderer;

    private void Awake ()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Init (CellType newCellType)
    {
        cellType = newCellType;
        switch (this.cellType)
        {
            case CellType.Terrain:
                meshRenderer.material.color = Color.green;
                break;
            case CellType.Water:
                meshRenderer.material.color = Color.cyan;
                break;
            case CellType.Mountain:
                meshRenderer.material.color = Color.yellow;
                break;
            case CellType.Last:
                break;
        }
    }
}