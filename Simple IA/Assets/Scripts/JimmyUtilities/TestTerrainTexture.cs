using JimmysUnityUtilities;
using UnityEngine;

public class TestTerrainTexture : MonoBehaviour
{
    public bool findTextureIndex;
    public TerrainTextureDetector terrain;

    private CustomTerrain t;

    void Update ()
    {
        if (!findTextureIndex) return;

        findTextureIndex = false;
        Vector3 pos = transform.position;

        int texture = terrain.GetDominantTextureIndexAt(pos);

        Debug.Log("Texture: " + texture);
    }
}