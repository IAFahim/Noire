using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CutGrass : MonoBehaviour
{
    [SerializeField] private Terrain t;
    [SerializeField] private bool[][] grassImage;
    [SerializeField] private int terrainX;
    [SerializeField] private int terrainZ;
    [SerializeField] private Texture2D grassImageShader;
    
    [SerializeField] private Material GrassMaterial;

    [SerializeField] private AbilitySO attackAbilityLight;
    [SerializeField] private AbilitySO attackAbilityHeavy;
    
    [SerializeField] private Material grassVariation;
    public Transform ps;
    [SerializeField] private Vector2 pos;
    [SerializeField] private float radius;
    [SerializeField] private float forwardModifier;
    [Tooltip("This determines the detail of the grassTexture. Higher runs faster")]
    [SerializeField] private int detailPrecision = 4;
    [Tooltip("This determines which details is put into the texture and which details arent")]
    [SerializeField] private int detailCuttoff = 4;

    [SerializeField] private int[] detailLayers;
    private bool canAttack = true;

    [SerializeField] private int cutGrassThreshold = 2;
    [SerializeField] private int particleGrassThreshold = 8;
    void Start()
    {
        t = GetComponent<Terrain>();
        
        terrainX = (int)t.terrainData.size.x;
        terrainZ = (int)t.terrainData.size.z;

        grassImage = new bool[terrainX][];

        for (int i = 0; i < terrainX; i++)
        {
            grassImage[i] = new bool[terrainZ];
        }
        
        InitializeGrassImage();

        GenerateTexture2D(terrainX, terrainZ);

        GrassMaterial.SetTexture("_SliceTexture", grassImageShader);
        grassVariation.SetTexture("_SliceTexture", grassImageShader);
    }
    
    private void InitializeGrassImage()
    {
        int[][,] details = new int[detailLayers.Length][,];
        int counter = 0;
        foreach (int detailLayer in detailLayers)
        {
            details[counter] = t.terrainData.GetDetailLayer(0, 0, t.terrainData.detailWidth,
                t.terrainData.detailHeight, detailLayer);
            counter++;
        }
        
        for (int i = 0; i < t.terrainData.detailWidth; i+= detailPrecision)
        {
            for (int j = 0; j < t.terrainData.detailHeight; j+= detailPrecision)
            {
                int terrainXNew = (int) (((float) i / (float) t.terrainData.detailWidth) * (float) terrainX);
                int terrainZNew = (int) (((float) j / (float) t.terrainData.detailHeight) * (float) terrainZ);
                int grassCounter = 0;
                for (int k = 0; k < counter; k++)
                {
                    grassCounter += details[k][j, i];
                }
                if (grassCounter > detailCuttoff)
                    grassImage[terrainXNew][terrainZNew] = false;
                else
                    grassImage[terrainXNew][terrainZNew] = true;
            }
        }
    }
    public void GenerateTexture2D(int terrainX, int terrainZ){
        grassImageShader = new Texture2D(terrainX, terrainZ);

        for (int h = 0; h < terrainX; h++)
        {
            for (int w = 0; w < terrainZ; w++)
            {
                grassImageShader.SetPixel(w, h, grassImage[w][h] ? Color.black : Color.white);
            } 
        }

        grassImageShader.filterMode = FilterMode.Point;
        grassImageShader.Apply();
    }

    void Update()
    {
        if (!Player.Instance.IsCasting() || !canAttack) return;
        if (Player.Instance.GetAbility().abilityID != attackAbilityLight.abilityID && Player.Instance.GetAbility().abilityID != attackAbilityHeavy.abilityID) return;
        
        canAttack = false;
        Transform playerPos = Player.Instance.transform;
        Vector2 playerCastPos = new Vector2(playerPos.position.x - transform.position.x, playerPos.position.z - transform.position.z) + new Vector2(playerPos.forward.x, playerPos.forward.z) * forwardModifier;
        
        Invoke("CutCooldown", 0.5f);
        
        if (!IsPlayerInTerrain(playerCastPos)) return;
        bool changed = false;  
        
        OnSlice(playerCastPos, radius, out changed);
        if (!changed) return;
    
        ps.GetComponent<ParticleSystem>().Play();
        ps.position = transform.position + new Vector3(playerCastPos.x, 0, playerCastPos.y);
    }

    bool IsPlayerInTerrain(Vector2 playerCastPos)
    {
        return playerCastPos.x > -2 && playerCastPos.y > -2 && playerCastPos.x < terrainX + 2 && playerCastPos.y < terrainZ + 2;
    }

    void CutCooldown()
    {
        canAttack = true;
    }
    /// This method is normally very expensive because of the grassImageShader.Apply(); call but since we are only
    /// writing to a small image of size 90x90 it doesnt handle too badly I will likely rewrite this on the GPU
    /// fully at some point because we are only modifying a small area of the image, and the Apply method overwrites
    /// the whole gpu image. I think the optimal solution would be to pass position and radius to a compute shader
    /// and return anyChanged becuase then we arent passing nearly as much info to and from the gpu. However, like
    /// this is not an issue until we deal with larger terrains.
    /// 
    /// My big issue with this approach will be passing grassImage[] to the gpu although because
    /// all texture2ds are stored on the gpu I may not need to and I may be able to do it very quickly.
    void OnSlice(Vector2 position, float radius, out bool anyChanged)
    {
        float squaredRadius = radius * radius;
        anyChanged = false;

        int minX = Mathf.Max(0, Mathf.FloorToInt(position.x - radius));
        int maxX = Mathf.Min(grassImage.Length, Mathf.CeilToInt(position.x + radius));
        int minY = Mathf.Max(0, Mathf.FloorToInt(position.y - radius));
        int maxY = Mathf.Min(grassImage[0].Length, Mathf.CeilToInt(position.y + radius));
        int updateCounter = 0;
        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                Vector2 deltaPos = new Vector2(i, j) - position;
                if (deltaPos.sqrMagnitude <= squaredRadius && grassImageShader.GetPixel(i, j) == Color.white)
                {
                    updateCounter++;
                    grassImage[i][j] = false;
                    grassImageShader.SetPixel(i, j, Color.black);
                }
            }
        }

        if (updateCounter > particleGrassThreshold)
        {
            anyChanged = true;
        }
        if (updateCounter > cutGrassThreshold)
        {
            grassImageShader.Apply();
        }
    }
}
