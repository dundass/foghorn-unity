using UnityEngine;
using System.Collections.Generic;

public class IslandTestComponent : MonoBehaviour
{
    [SerializeField] public Vector2 islandLocation = new Vector2(150, 100);
    [SerializeField] public int gridSize = 384;
    [SerializeField] public int growthDelay = 5;
    [SerializeField] public int maxIterations = 11;
    
    // Seed parameters
    [SerializeField] public int numSeeds = 12;
    [SerializeField] public int minSeedState = 1;
    [SerializeField] public int maxSeedState = 4;
    [SerializeField] public int seedSpreadRange = 10;
    [SerializeField] public long randomSeed = 0;
    [SerializeField] public bool autoRandomizeSeed = true;
    
    // Ruleset
    [SerializeField] public string rulesetName = "island";
    
    // References
    [SerializeField] public TerraGenerator terraGenerator;
    [SerializeField] public WorldRenderer worldRenderer;
    
    // Cache for preview
    [HideInInspector] public CA2D previewCA;
    [HideInInspector] public Dictionary<Vector2Int, int> generatedSeeds = new();
    [HideInInspector] public int lastGenerationIteration = -1;
    
    // For editor preview
    [HideInInspector] public Texture2D previewTexture;
    [HideInInspector] public Color[] tileColors = new Color[10];
    
    private void OnValidate()
    {
        // Clamp values
        growthDelay = Mathf.Max(0, growthDelay);
        maxIterations = Mathf.Max(1, maxIterations);
        numSeeds = Mathf.Max(1, numSeeds);
        minSeedState = Mathf.Max(1, minSeedState);
        maxSeedState = Mathf.Max(minSeedState, maxSeedState);
        seedSpreadRange = Mathf.Max(1, seedSpreadRange);
        gridSize = Mathf.Max(64, gridSize);
    }
}
