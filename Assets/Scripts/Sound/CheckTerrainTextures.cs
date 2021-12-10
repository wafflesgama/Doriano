using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTerrainTextures : MonoBehaviour
{

    public Transform playerTransform;
    public Terrain terrain;

    public int posX;
    public int posZ;
    public float[] terrainTypeValues = null;

    private void Start()
    {
        terrainTypeValues = new float[terrain.terrainData.alphamapLayers];
    }

    public float[] CalculateTerrainValues()
    {
        var relativePos = ConvertPosition(playerTransform.position);
        CheckTexture((int)relativePos.x, (int)relativePos.y);
        return terrainTypeValues;
    }

    Vector2 ConvertPosition(Vector3 playerPosition)
    {
        Vector3 terrainPosition = playerPosition - terrain.transform.position;

        Vector3 mapPosition = new Vector3
        (terrainPosition.x / terrain.terrainData.size.x, 0,
        terrainPosition.z / terrain.terrainData.size.z);

        float xCoord = mapPosition.x * terrain.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * terrain.terrainData.alphamapHeight;

        return new Vector2(xCoord, zCoord);
    }

    void CheckTexture(int posX, int posZ)
    {
        float[,,] aMap = terrain.terrainData.GetAlphamaps(posX, posZ, 1, 1);

        for (int i = 0; i < terrainTypeValues.Length; i++)
            terrainTypeValues[i] = aMap[0, 0, i];
    }
}