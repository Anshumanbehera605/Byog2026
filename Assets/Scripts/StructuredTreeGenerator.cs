using UnityEngine;
using System.Collections.Generic;

public class NaturalForestGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public Terrain terrain;
    public int treePrototypeIndex = 0;

    [Header("Forest Density & Size")]
    [Tooltip("How many total trees to try spawning across the terrain.")]
    public int attemptCount = 20000;

    [Header("Organic Clustering (Perlin Noise)")]
    [Tooltip("Higher value = smaller, more frequent clusters of trees.")]
    public float noiseScale = 5f;
    [Tooltip("0 = forest everywhere. 0.8 = small sparse patches. 1 = no trees.")]
    [Range(0f, 1f)]
    public float densityThreshold = 0.45f;

    void Start()
    {
        GenerateNaturalForest();
    }

    public void GenerateNaturalForest()
    {
        if (terrain == null) terrain = Terrain.activeTerrain;
        if (terrain == null) return;

        TerrainData terrainData = terrain.terrainData;
        List<TreeInstance> newTrees = new List<TreeInstance>(terrainData.treeInstances);

        // We use a random offset so the noise pattern is different every time you hit play
        float offsetX = Random.Range(0f, 9999f);
        float offsetZ = Random.Range(0f, 9999f);

        for (int i = 0; i < attemptCount; i++)
        {
            // 1. Pick a random normalized coordinate (0.0 to 1.0) on the terrain
            float normX = Random.Range(0f, 1f);
            float normZ = Random.Range(0f, 1f);

            // 2. Sample Perlin Noise to see if a tree should grow here (creates natural clusters)
            float noiseValue = Mathf.PerlinNoise((normX * noiseScale) + offsetX, (normZ * noiseScale) + offsetZ);

            if (noiseValue > densityThreshold)
            {
                // 3. Find the exact height of the terrain at this X/Z coordinate
                float worldX = terrain.transform.position.x + (normX * terrainData.size.x);
                float worldZ = terrain.transform.position.z + (normZ * terrainData.size.z);
                float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));
                float normY = worldY / terrainData.size.y;

                // 4. Create the tree with randomized rotation and scale
                TreeInstance tree = new TreeInstance
                {
                    position = new Vector3(normX, normY, normZ),
                    widthScale = Random.Range(0.7f, 1.4f),  // Natural thickness variation
                    heightScale = Random.Range(0.7f, 1.4f), // Natural height variation
                    rotation = Random.Range(0f, 2f * Mathf.PI), // Random rotation (0 to 360 degrees in radians)
                    color = Color.white,
                    lightmapColor = Color.white,
                    prototypeIndex = treePrototypeIndex
                };

                newTrees.Add(tree);
            }
        }

        // Apply to terrain
        terrainData.treeInstances = newTrees.ToArray();
        terrain.Flush();
    }
}