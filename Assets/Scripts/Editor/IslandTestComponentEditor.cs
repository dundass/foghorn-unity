using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(IslandTestComponent))]
public class IslandTestComponentEditor : Editor
{
    private IslandTestComponent component;
    private Dictionary<string, int[]> availableRulesets = new();
    
    private Vector2 scrollPosition;
    private bool showSeedInfo = true;
    private bool showRulesetInfo = false;
    private bool showPreview = true;
    private Vector2 rulesetScrollPosition;
    
    // Cached CA for preview
    private CA2D previewCA;
    private int currentPreviewIteration = 0;

    private void OnEnable()
    {
        component = (IslandTestComponent)target;
        LoadRulesets();
        InitializePreview();
    }

    private void OnDisable()
    {
        if (component.previewTexture != null)
            DestroyImmediate(component.previewTexture);
        if (previewCA != null)
            previewCA = null;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Island Test Component", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Island Location
        EditorGUILayout.LabelField("Island Location", EditorStyles.boldLabel);
        component.islandLocation = EditorGUILayout.Vector2Field("Location", component.islandLocation);

        EditorGUILayout.Space();

        // Grid parameters
        EditorGUILayout.LabelField("Grid Parameters", EditorStyles.boldLabel);
        component.gridSize = EditorGUILayout.IntSlider("Grid Size", component.gridSize, 64, 512);

        EditorGUILayout.Space();

        // Seed parameters
        EditorGUILayout.LabelField("Seed Parameters", EditorStyles.boldLabel);
        
        component.numSeeds = EditorGUILayout.IntSlider("Number of Seeds", component.numSeeds, 1, 30);
        component.minSeedState = EditorGUILayout.IntSlider("Min Seed State", component.minSeedState, 1, 9);
        component.maxSeedState = EditorGUILayout.IntSlider("Max Seed State", component.maxSeedState, component.minSeedState, 9);
        component.seedSpreadRange = EditorGUILayout.IntSlider("Seed Spread Range", component.seedSpreadRange, 1, component.gridSize / 2);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Random Seed", EditorStyles.label, GUILayout.Width(100));
        component.randomSeed = long.TryParse(EditorGUILayout.TextField(component.randomSeed.ToString()), out long parsed) ? parsed : component.randomSeed;
        component.autoRandomizeSeed = EditorGUILayout.Toggle(component.autoRandomizeSeed, GUILayout.Width(20));
        EditorGUILayout.LabelField("Auto", GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Growth control
        EditorGUILayout.LabelField("Growth Control", EditorStyles.boldLabel);
        component.growthDelay = EditorGUILayout.IntSlider("Growth Delay", component.growthDelay, 0, component.maxIterations - 1);
        component.maxIterations = EditorGUILayout.IntSlider("Max Iterations", component.maxIterations, 1, 20);

        EditorGUILayout.Space();

        // Ruleset selection
        EditorGUILayout.LabelField("Ruleset", EditorStyles.boldLabel);
        var presetNames = availableRulesets.Keys.ToList();
        int selectedIndex = presetNames.IndexOf(component.rulesetName);
        selectedIndex = EditorGUILayout.Popup("Preset", selectedIndex, presetNames.ToArray());
        
        if (selectedIndex >= 0 && selectedIndex < presetNames.Count)
        {
            component.rulesetName = presetNames[selectedIndex];
        }

        EditorGUILayout.Space();

        // References
        EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
        component.terraGenerator = (TerraGenerator)EditorGUILayout.ObjectField("Terra Generator", component.terraGenerator, typeof(TerraGenerator), true);
        component.worldRenderer = (WorldRenderer)EditorGUILayout.ObjectField("World Renderer", component.worldRenderer, typeof(WorldRenderer), true);

        EditorGUILayout.Space();

        // Seed info
        showSeedInfo = EditorGUILayout.Foldout(showSeedInfo, $"Generated Seeds ({component.generatedSeeds.Count})");
        if (showSeedInfo && component.generatedSeeds.Count > 0)
        {
            EditorGUILayout.LabelField("Seed List", EditorStyles.miniLabel);
            int displayCount = 0;
            foreach (var seed in component.generatedSeeds)
            {
                EditorGUILayout.LabelField($"Pos: {seed.Key}, State: {seed.Value}", EditorStyles.miniLabel);
                displayCount++;
                if (displayCount >= 20)
                {
                    EditorGUILayout.LabelField($"... and {component.generatedSeeds.Count - 20} more", EditorStyles.miniLabel);
                    break;
                }
            }
        }

        EditorGUILayout.Space();

        // Ruleset display
        showRulesetInfo = EditorGUILayout.Foldout(showRulesetInfo, "Current Ruleset");
        if (showRulesetInfo && availableRulesets.ContainsKey(component.rulesetName))
        {
            int[] ruleset = availableRulesets[component.rulesetName];
            rulesetScrollPosition = EditorGUILayout.BeginScrollView(rulesetScrollPosition, GUILayout.Height(150));
            EditorGUILayout.LabelField($"Ruleset: {component.rulesetName}", EditorStyles.miniLabel);
            
            for (int i = 0; i < ruleset.Length; i++)
            {
                if (i % 8 == 0)
                {
                    EditorGUILayout.LabelField($"Neighbourhood totals 0-{Mathf.Min(7, ruleset.Length - i - 1)}: [{string.Join(", ", ruleset.Skip(i).Take(8))}]", EditorStyles.miniLabel);
                }
            }
            
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space();

        // Preview
        showPreview = EditorGUILayout.Foldout(showPreview, "Live Preview");
        if (showPreview)
        {
            EditorGUILayout.LabelField($"Preview Iteration: {currentPreviewIteration}/{component.maxIterations}", EditorStyles.miniLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Preview", GUILayout.Width(120)))
            {
                GeneratePreview();
            }
            if (GUILayout.Button("Step", GUILayout.Width(60)))
            {
                if (currentPreviewIteration == component.growthDelay && component.generatedSeeds.Count > 0)
                {
                    PlacePreviewSeeds();
                }
                previewCA.Update();
                currentPreviewIteration++;
            }
            if (GUILayout.Button("Complete", GUILayout.Width(70)))
            {
                while (currentPreviewIteration < component.maxIterations)
                {
                    if (currentPreviewIteration == component.growthDelay && component.generatedSeeds.Count > 0)
                    {
                        PlacePreviewSeeds();
                    }
                    previewCA.Update();
                    currentPreviewIteration++;
                }
            }
            EditorGUILayout.EndHorizontal();

            UpdatePreviewTexture();
            Rect textureRect = GUILayoutUtility.GetRect(256, 256);
            if (component.previewTexture != null)
            {
                GUI.DrawTexture(textureRect, component.previewTexture, ScaleMode.ScaleToFit);
            }
        }

        EditorGUILayout.Space();

        // Main control buttons
        EditorGUILayout.LabelField("Apply Changes", EditorStyles.boldLabel);
        
        if (GUILayout.Button("APPLY CHANGES TO WORLD", GUILayout.Height(40)))
        {
            ApplyChangesToWorld();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Warning: This regenerates the entire chunkCA and landCA. For efficiency, apply changes once per testing session.", MessageType.Info);

        EditorGUILayout.EndScrollView();

        serializedObject.ApplyModifiedProperties();
    }

    private void LoadRulesets()
    {
        if (component.terraGenerator == null)
            component.terraGenerator = FindObjectOfType<TerraGenerator>();

        if (component.terraGenerator != null)
        {
            var rulesetField = typeof(TerraGenerator).GetField("rulesets",
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (rulesetField != null && rulesetField.GetValue(component.terraGenerator) is Dictionary<string, int[]> rulesets)
            {
                availableRulesets.Clear();
                foreach (var kvp in rulesets)
                {
                    availableRulesets[kvp.Key] = (int[])kvp.Value.Clone();
                }
            }
        }
    }

    private void InitializePreview()
    {
        if (component.previewTexture == null)
        {
            component.previewTexture = new Texture2D(component.gridSize, component.gridSize, TextureFormat.RGBA32, false);
        }

        if (component.tileColors == null || component.tileColors.Length != 10)
        {
            component.tileColors = new Color[10];
            component.tileColors[0] = new Color(0.2f, 0.4f, 0.6f, 1f); // Water
            component.tileColors[1] = new Color(0.34f, 0.68f, 0.34f, 1f); // Ground 1
            component.tileColors[2] = new Color(0.40f, 0.75f, 0.40f, 1f); // Ground 2
            component.tileColors[3] = new Color(0.46f, 0.82f, 0.46f, 1f); // Ground 3
            component.tileColors[4] = new Color(0.2f, 0.55f, 0.2f, 1f); // Forest 1
            component.tileColors[5] = new Color(0.24f, 0.62f, 0.24f, 1f); // Forest 2
            component.tileColors[6] = new Color(0.28f, 0.69f, 0.28f, 1f); // Forest 3
            component.tileColors[7] = new Color(0.6f, 0.5f, 0.3f, 1f); // Mountain 1
            component.tileColors[8] = new Color(0.68f, 0.58f, 0.38f, 1f); // Mountain 2
            component.tileColors[9] = new Color(0.76f, 0.66f, 0.46f, 1f); // Mountain 3
        }

        previewCA = new CA2D(10, component.gridSize);
        if (availableRulesets.ContainsKey(component.rulesetName))
        {
            previewCA.ruleSet = (int[])availableRulesets[component.rulesetName].Clone();
        }
        currentPreviewIteration = 0;
    }

    private void GeneratePreview()
    {
        InitializePreview();

        if (component.autoRandomizeSeed)
        {
            component.randomSeed = System.DateTime.Now.Ticks;
        }

        GenerateSeeds();
        currentPreviewIteration = 0;
        Debug.Log($"Preview generated with {component.generatedSeeds.Count} seeds");
    }

    private void GenerateSeeds()
    {
        System.Random rnd = new System.Random((int)(component.randomSeed % int.MaxValue));
        component.generatedSeeds.Clear();

        Vector2Int center = new Vector2Int(component.gridSize / 2, component.gridSize / 2);

        for (int i = 0; i < component.numSeeds; i++)
        {
            var seedPos = new Vector2Int(
                center.x + rnd.Next(-component.seedSpreadRange, component.seedSpreadRange),
                center.y + rnd.Next(-component.seedSpreadRange, component.seedSpreadRange)
            );

            seedPos.x = Mathf.Clamp(seedPos.x, 0, component.gridSize - 1);
            seedPos.y = Mathf.Clamp(seedPos.y, 0, component.gridSize - 1);

            int seedState = rnd.Next(component.minSeedState, component.maxSeedState + 1);
            component.generatedSeeds[seedPos] = seedState;
        }
    }

    private void PlacePreviewSeeds()
    {
        foreach (var seed in component.generatedSeeds)
        {
            previewCA.SetCell(seed.Key.x, seed.Key.y, seed.Value);
        }
    }

    private void UpdatePreviewTexture()
    {
        if (previewCA == null || component.previewTexture == null)
            return;

        Color[] colors = new Color[component.gridSize * component.gridSize];
        
        for (int i = 0; i < component.gridSize; i++)
        {
            for (int j = 0; j < component.gridSize; j++)
            {
                int cellValue = previewCA.GetCell(i, j);
                colors[j * component.gridSize + i] = component.tileColors[cellValue];
            }
        }

        component.previewTexture.SetPixels(colors);
        component.previewTexture.Apply();
    }

    private void ApplyChangesToWorld()
    {
        if (component.terraGenerator == null)
        {
            EditorUtility.DisplayDialog("Error", "TerraGenerator not found. Please assign it in the inspector.", "OK");
            return;
        }

        if (component.worldRenderer == null)
        {
            EditorUtility.DisplayDialog("Error", "WorldRenderer not found. Please assign it in the inspector.", "OK");
            return;
        }

        // Get the TerraGenerator's private fields
        var rulesetsField = typeof(TerraGenerator).GetField("rulesets",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var chunkCAField = typeof(TerraGenerator).GetField("chunkCA",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var landCAField = typeof(TerraGenerator).GetField("landCA",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var blockSizeField = typeof(TerraGenerator).GetField("blockSize",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (rulesetsField == null || chunkCAField == null || landCAField == null || blockSizeField == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not access TerraGenerator private fields. Script structure may have changed.", "OK");
            return;
        }

        Dictionary<string, int[]> rulesets = (Dictionary<string, int[]>)rulesetsField.GetValue(component.terraGenerator);
        CA2D chunkCA = (CA2D)chunkCAField.GetValue(component.terraGenerator);
        CA2D landCA = (CA2D)landCAField.GetValue(component.terraGenerator);
        int blockSize = (int)blockSizeField.GetValue(component.terraGenerator);

        if (!availableRulesets.ContainsKey(component.rulesetName))
        {
            EditorUtility.DisplayDialog("Error", $"Ruleset '{component.rulesetName}' not found.", "OK");
            return;
        }

        // Ensure chunkCA exists
        if (chunkCA == null)
        {
            chunkCA = new CA2D(4, 384);
            chunkCAField.SetValue(component.terraGenerator, chunkCA);
        }

        // Ensure landCA exists
        if (landCA == null)
        {
            landCA = new CA2D(10, 384 * blockSize);
            landCAField.SetValue(component.terraGenerator, landCA);
        }

        // Set the ruleset
        chunkCA.ruleSet = (int[])availableRulesets[component.rulesetName].Clone();

        // Generate and apply island seeds
        if (component.autoRandomizeSeed)
        {
            component.randomSeed = System.DateTime.Now.Ticks;
        }
        GenerateSeeds();

        // Clear and regenerate chunkCA with the island
        chunkCA.Clear();
        
        // Apply growth delay and seeds like TerraGenerator does
        for (int i = 0; i < component.maxIterations; i++)
        {
            if (i == component.growthDelay)
            {
                // Place seeds at island location
                foreach (var seed in component.generatedSeeds)
                {
                    int x = (int)component.islandLocation.x + seed.Key.x;
                    int y = (int)component.islandLocation.y + seed.Key.y;
                    
                    // Clamp to chunkCA bounds
                    x = Mathf.Clamp(x, 0, chunkCA.GetXsize() - 1);
                    y = Mathf.Clamp(y, 0, chunkCA.GetYsize() - 1);
                    
                    chunkCA.SetCell(x, y, seed.Value);
                }
            }
            chunkCA.Update();
        }

        // Regenerate landCA from chunkCA (like TerraGenerator does)
        landCA.Clear();
        
        for (int i = 0; i < landCA.GetXsize(); i++)
        {
            for (int j = 0; j < landCA.GetYsize(); j++)
            {
                int chunkX = i / blockSize;
                int chunkY = j / blockSize;
                
                if (chunkCA.cells[chunkX, chunkY] == 0)
                {
                    // Remove some lagoons
                    if (chunkCA.GetLiveNeighbours(chunkX, chunkY) == 8 && UnityEngine.Random.value > 0.05f)
                    {
                        chunkCA.cells[chunkX, chunkY] = 1;
                    }
                    else
                    {
                        continue;
                    }

                    if (UnityEngine.Random.value > 0.05f)
                    {
                        landCA.cells[i, j] = UnityEngine.Random.Range(0, landCA.numStates);
                    }
                }
                else
                {
                    if (UnityEngine.Random.value > 0.3f)
                    {
                        landCA.cells[i, j] = (chunkCA.cells[chunkX, chunkY] * 3) + UnityEngine.Random.Range(0, 3);
                    }
                }
            }
        }

        // Update landCA with upscale iterations
        var upscaleField = typeof(TerraGenerator).GetField("upscaleCellIterations",
            BindingFlags.NonPublic | BindingFlags.Instance);
        int upscaleIterations = upscaleField != null ? (int)upscaleField.GetValue(component.terraGenerator) : 2;
        landCA.Update(upscaleIterations);

        // Render the world
        var houseCAField = typeof(TerraGenerator).GetField("houseCA",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var forestCAField = typeof(TerraGenerator).GetField("forestCA",
            BindingFlags.NonPublic | BindingFlags.Instance);

        CA2D houseCA = (CA2D)houseCAField.GetValue(component.terraGenerator);
        CA2D forestCA = (CA2D)forestCAField.GetValue(component.terraGenerator);

        component.worldRenderer.RenderWorld(landCA, houseCA, forestCA);

        EditorUtility.DisplayDialog("Success", $"Island at {component.islandLocation} with ruleset '{component.rulesetName}' applied to world!", "OK");
        Debug.Log($"Island growth test applied: Location={component.islandLocation}, GrowthDelay={component.growthDelay}, Seeds={component.generatedSeeds.Count}");
    }
}
