using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class IslandGrowthTester : EditorWindow
{
    private CA2D islandCA;
    private Texture2D previewTexture;
    
    private Vector2Int gridSize = new Vector2Int(128, 128);
    private int numStates = 10;
    private int maxIterations = 11;
    private int currentIteration = 0;
    
    // Seed parameters
    private int numSeeds = 12;
    private int minSeedState = 1;
    private int maxSeedState = 4;
    private int seedSpreadRange = 10;
    private long randomSeed = 0;
    private bool autoRandomizeSeed = true;
    
    // Growth delay (controls when seeds are placed)
    private int growthDelay = 5;
    
    // Ruleset management
    private Dictionary<string, int[]> customRulesets = new();
    private string currentRulesetName = "island";
    private int[] currentRuleset;
    private int[] editingRuleset;
    
    // Tile color mapping
    private bool showTileMapping = false;
    private Vector2 tileMapScrollPosition;
    private Color[] tileColors = new Color[10];
    
    // UI state
    private Vector2 scrollPosition;
    private bool showSeedEditor = true;
    private bool showRulesetEditor = true;
    private Vector2 rulesetScrollPosition;
    
    // Seeds storage
    private List<(Vector2Int pos, int state)> currentSeeds = new();

    [MenuItem("Window/Island Growth Tester")]
    public static void ShowWindow()
    {
        GetWindow<IslandGrowthTester>("Island Growth Tester");
    }

    private void OnEnable()
    {
        TerraGenerator terraGen = FindObjectOfType<TerraGenerator>();
        if (terraGen != null)
        {
            LoadPresetsFromTerraGenerator(terraGen);
        }
        
        InitializeCA();
        InitializeTileColors();
    }

    private void OnDisable()
    {
        if (previewTexture != null)
            DestroyImmediate(previewTexture);
    }

    private void InitializeCA()
    {
        islandCA = new CA2D(numStates, gridSize.x, gridSize.y);
        
        if (customRulesets.ContainsKey(currentRulesetName))
        {
            currentRuleset = (int[])customRulesets[currentRulesetName].Clone();
        }
        else
        {
            currentRuleset = new int[numStates * 8];
        }
        
        editingRuleset = (int[])currentRuleset.Clone();
        islandCA.ruleSet = (int[])editingRuleset.Clone();
        
        if (previewTexture == null)
        {
            previewTexture = new Texture2D(gridSize.x, gridSize.y, TextureFormat.RGBA32, false);
        }
    }

    private void InitializeTileColors()
    {
        // Default colors for 10-state CA
        tileColors[0] = new Color(0.2f, 0.4f, 0.6f, 1f); // Water - blue
        tileColors[1] = new Color(0.34f, 0.68f, 0.34f, 1f); // Ground 1 - green
        tileColors[2] = new Color(0.40f, 0.75f, 0.40f, 1f); // Ground 2 - light green
        tileColors[3] = new Color(0.46f, 0.82f, 0.46f, 1f); // Ground 3 - lighter green
        tileColors[4] = new Color(0.2f, 0.55f, 0.2f, 1f); // Forest 1 - dark green
        tileColors[5] = new Color(0.24f, 0.62f, 0.24f, 1f); // Forest 2
        tileColors[6] = new Color(0.28f, 0.69f, 0.28f, 1f); // Forest 3
        tileColors[7] = new Color(0.6f, 0.5f, 0.3f, 1f); // Mountain 1 - brown
        tileColors[8] = new Color(0.68f, 0.58f, 0.38f, 1f); // Mountain 2 - light brown
        tileColors[9] = new Color(0.76f, 0.66f, 0.46f, 1f); // Mountain 3 - lighter brown
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Island Growth Tester", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Grid parameters
        EditorGUILayout.LabelField("Grid Parameters", EditorStyles.boldLabel);
        Vector2Int newGridSize = EditorGUILayout.Vector2IntField("Grid Size", gridSize);
        if (newGridSize != gridSize)
        {
            gridSize = newGridSize;
            InitializeCA();
            currentIteration = 0;
        }

        EditorGUILayout.Space();

        // Seed parameters
        EditorGUILayout.LabelField("Seed Parameters", EditorStyles.boldLabel);
        
        numSeeds = EditorGUILayout.IntSlider("Number of Seeds", numSeeds, 1, 30);
        minSeedState = EditorGUILayout.IntSlider("Min Seed State", minSeedState, 1, numStates - 1);
        maxSeedState = EditorGUILayout.IntSlider("Max Seed State", maxSeedState, minSeedState, numStates - 1);
        seedSpreadRange = EditorGUILayout.IntSlider("Seed Spread Range", seedSpreadRange, 1, gridSize.x / 2);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Random Seed", EditorStyles.label, GUILayout.Width(100));
        randomSeed = long.TryParse(EditorGUILayout.TextField(randomSeed.ToString()), out long parsed) ? parsed : randomSeed;
        autoRandomizeSeed = EditorGUILayout.Toggle(autoRandomizeSeed, GUILayout.Width(20));
        EditorGUILayout.LabelField("Auto", GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Growth delay
        EditorGUILayout.LabelField("Growth Control", EditorStyles.boldLabel);
        growthDelay = EditorGUILayout.IntSlider("Growth Delay (seed placement iteration)", growthDelay, 0, maxIterations - 1);
        maxIterations = EditorGUILayout.IntSlider("Max Iterations", maxIterations, 1, 20);

        EditorGUILayout.Space();

        // Ruleset selection
        EditorGUILayout.LabelField("Ruleset", EditorStyles.boldLabel);
        var presetNames = customRulesets.Keys.ToList();
        int selectedIndex = presetNames.IndexOf(currentRulesetName);
        selectedIndex = EditorGUILayout.Popup("Preset", selectedIndex, presetNames.ToArray());
        
        if (selectedIndex >= 0 && selectedIndex < presetNames.Count)
        {
            string newName = presetNames[selectedIndex];
            if (newName != currentRulesetName)
            {
                currentRulesetName = newName;
                currentRuleset = (int[])customRulesets[currentRulesetName].Clone();
                editingRuleset = (int[])currentRuleset.Clone();
                islandCA.ruleSet = (int[])editingRuleset.Clone();
                currentIteration = 0;
            }
        }

        EditorGUILayout.Space();

        // Ruleset editor
        showRulesetEditor = EditorGUILayout.Foldout(showRulesetEditor, "Edit Ruleset");
        if (showRulesetEditor)
        {
            rulesetScrollPosition = EditorGUILayout.BeginScrollView(rulesetScrollPosition, GUILayout.Height(150));
            
            EditorGUILayout.LabelField($"Neighbourhood Total -> Next State", EditorStyles.miniLabel);
            
            for (int i = 0; i < editingRuleset.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Total {i}", GUILayout.Width(60));
                int newValue = EditorGUILayout.IntSlider(editingRuleset[i], 0, numStates - 1, GUILayout.ExpandWidth(true));
                if (newValue != editingRuleset[i])
                {
                    editingRuleset[i] = newValue;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply Ruleset"))
            {
                currentRuleset = (int[])editingRuleset.Clone();
                islandCA.ruleSet = (int[])editingRuleset.Clone();
            }
            if (GUILayout.Button("Random Ruleset"))
            {
                islandCA.SetLambdaRuleset(0.38f);
                editingRuleset = (int[])islandCA.ruleSet.Clone();
                currentRuleset = (int[])editingRuleset.Clone();
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        // Tile color mapping
        showTileMapping = EditorGUILayout.Foldout(showTileMapping, "Tile Color Mapping");
        if (showTileMapping)
        {
            tileMapScrollPosition = EditorGUILayout.BeginScrollView(tileMapScrollPosition, GUILayout.Height(150));
            
            for (int i = 0; i < tileColors.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"State {i}", GUILayout.Width(60));
                tileColors[i] = EditorGUILayout.ColorField(tileColors[i]);
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space();

        // Control buttons
        EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Initialize Island", GUILayout.Height(30)))
        {
            InitializeIsland();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Step", GUILayout.Width(100)))
        {
            if (currentIteration == growthDelay)
            {
                PlaceSeeds();
            }
            islandCA.Update();
            currentIteration++;
        }
        if (GUILayout.Button("Run to Growth Point", GUILayout.Width(150)))
        {
            while (currentIteration < growthDelay)
            {
                islandCA.Update();
                currentIteration++;
            }
            PlaceSeeds();
            islandCA.Update();
            currentIteration++;
        }
        if (GUILayout.Button("Complete Growth", GUILayout.Width(130)))
        {
            while (currentIteration < maxIterations)
            {
                if (currentIteration == growthDelay && currentSeeds.Count == 0)
                {
                    PlaceSeeds();
                }
                islandCA.Update();
                currentIteration++;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset", GUILayout.Width(100)))
        {
            islandCA.Clear();
            currentSeeds.Clear();
            currentIteration = 0;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField($"Iteration: {currentIteration}/{maxIterations}", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Seeds Placed: {currentSeeds.Count}");
        EditorGUILayout.LabelField($"Live Cells: {CountLiveCells()}");

        EditorGUILayout.Space();

        // Seed display
        showSeedEditor = EditorGUILayout.Foldout(showSeedEditor, $"Current Seeds ({currentSeeds.Count})");
        if (showSeedEditor)
        {
            EditorGUILayout.LabelField("Seed List", EditorStyles.miniLabel);
            foreach (var seed in currentSeeds)
            {
                EditorGUILayout.LabelField($"Position: {seed.pos}, State: {seed.state}");
            }
        }

        EditorGUILayout.Space();

        // Preview
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
        UpdatePreviewTexture();
        
        Rect textureRect = GUILayoutUtility.GetRect(256, 256);
        if (previewTexture != null)
        {
            GUI.DrawTexture(textureRect, previewTexture, ScaleMode.ScaleToFit);
        }

        EditorGUILayout.EndScrollView();
    }

    private void InitializeIsland()
    {
        islandCA.Clear();
        currentSeeds.Clear();
        currentIteration = 0;
        
        if (autoRandomizeSeed)
        {
            randomSeed = System.DateTime.Now.Ticks;
        }

        GenerateSeeds();
        Debug.Log($"Island initialized with {currentSeeds.Count} seeds. Growth will occur at iteration {growthDelay}");
    }

    private void GenerateSeeds()
    {
        System.Random rnd = new System.Random((int)(randomSeed % int.MaxValue));
        currentSeeds.Clear();

        Vector2Int center = new Vector2Int(gridSize.x / 2, gridSize.y / 2);

        for (int i = 0; i < numSeeds; i++)
        {
            var seedPos = new Vector2Int(
                center.x + rnd.Next(-seedSpreadRange, seedSpreadRange),
                center.y + rnd.Next(-seedSpreadRange, seedSpreadRange)
            );

            // Clamp to grid
            seedPos.x = Mathf.Clamp(seedPos.x, 0, gridSize.x - 1);
            seedPos.y = Mathf.Clamp(seedPos.y, 0, gridSize.y - 1);

            int seedState = rnd.Next(minSeedState, maxSeedState + 1);
            currentSeeds.Add((seedPos, seedState));
        }
    }

    private void PlaceSeeds()
    {
        foreach (var seed in currentSeeds)
        {
            islandCA.SetCell(seed.pos.x, seed.pos.y, seed.state);
        }
    }

    private void UpdatePreviewTexture()
    {
        if (islandCA == null || previewTexture == null)
            return;

        Color[] colors = new Color[gridSize.x * gridSize.y];
        
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                int cellValue = islandCA.GetCell(i, j);
                colors[j * gridSize.x + i] = tileColors[cellValue];
            }
        }

        previewTexture.SetPixels(colors);
        previewTexture.Apply();
    }

    private void LoadPresetsFromTerraGenerator(TerraGenerator terraGen)
    {
        var rulesetField = typeof(TerraGenerator).GetField("rulesets", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (rulesetField != null && rulesetField.GetValue(terraGen) is Dictionary<string, int[]> rulesets)
        {
            foreach (var kvp in rulesets)
            {
                customRulesets[kvp.Key] = (int[])kvp.Value.Clone();
            }
        }
    }

    private int CountLiveCells()
    {
        int count = 0;
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                if (islandCA.GetCell(i, j) > 0)
                    count++;
            }
        }
        return count;
    }
}
