using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RulesetExplorer : EditorWindow
{
    private CA2D previewCA;
    private Texture2D previewTexture;
    
    private Vector2Int previewSize = new Vector2Int(128, 128);
    private int numStates = 4;
    private int iterations = 5;
    private float randomFillProbability = 0.3f;
    
    private Dictionary<string, int[]> customRulesets = new();
    private string currentRulesetName = "island";
    private bool showRulesetEditor = true;
    private bool isPlaying = false;
    private int currentIteration = 0;
    
    private int[] currentRuleset;
    private int[] editingRuleset;
    private Vector2 rulesetScrollPosition;
    private Vector2 scrollPosition;
    
    private double lastUpdateTime;
    private bool shouldRefresh = true;

    [MenuItem("Window/Ruleset Explorer")]
    public static void ShowWindow()
    {
        GetWindow<RulesetExplorer>("Ruleset Explorer");
    }

    private void OnEnable()
    {
        TerraGenerator terraGen = FindObjectOfType<TerraGenerator>();
        if (terraGen != null)
        {
            LoadPresetsFromTerraGenerator(terraGen);
        }
        
        previewCA = new CA2D(numStates, previewSize.x, previewSize.y);
        previewTexture = new Texture2D(previewSize.x, previewSize.y, TextureFormat.RGBA32, false);
        
        if (customRulesets.ContainsKey(currentRulesetName))
        {
            currentRuleset = (int[])customRulesets[currentRulesetName].Clone();
        }
        else
        {
            currentRuleset = new int[numStates * 8];
        }
        
        editingRuleset = (int[])currentRuleset.Clone();
        previewCA.ruleSet = (int[])editingRuleset.Clone();
        previewCA.SetRandomStates(randomFillProbability);
        shouldRefresh = true;
    }

    private void OnDisable()
    {
        if (previewTexture != null)
            DestroyImmediate(previewTexture);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Ruleset Explorer", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Preset selection
        EditorGUILayout.LabelField("Presets", EditorStyles.boldLabel);
        var presetNames = customRulesets.Keys.ToList();
        int selectedIndex = presetNames.IndexOf(currentRulesetName);
        selectedIndex = EditorGUILayout.Popup("Ruleset", selectedIndex, presetNames.ToArray());
        
        if (selectedIndex >= 0 && selectedIndex < presetNames.Count)
        {
            string newName = presetNames[selectedIndex];
            if (newName != currentRulesetName)
            {
                currentRulesetName = newName;
                currentRuleset = (int[])customRulesets[currentRulesetName].Clone();
                editingRuleset = (int[])currentRuleset.Clone();
                previewCA.ruleSet = (int[])editingRuleset.Clone();
                currentIteration = 0;
                shouldRefresh = true;
            }
        }

        EditorGUILayout.Space();

        // CA Parameters
        EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
        
        int newNumStates = EditorGUILayout.IntSlider("States", numStates, 2, 10);
        if (newNumStates != numStates)
        {
            numStates = newNumStates;
            previewCA = new CA2D(numStates, previewSize.x, previewSize.y);
            editingRuleset = new int[numStates * 8];
            previewCA.ruleSet = (int[])editingRuleset.Clone();
            currentIteration = 0;
            shouldRefresh = true;
        }

        Vector2Int newPreviewSize = EditorGUILayout.Vector2IntField("Preview Size", previewSize);
        if (newPreviewSize != previewSize)
        {
            previewSize = newPreviewSize;
            previewCA = new CA2D(numStates, previewSize.x, previewSize.y);
            previewCA.ruleSet = (int[])editingRuleset.Clone();
            DestroyImmediate(previewTexture);
            previewTexture = new Texture2D(previewSize.x, previewSize.y, TextureFormat.RGBA32, false);
            currentIteration = 0;
            shouldRefresh = true;
        }

        randomFillProbability = EditorGUILayout.Slider("Initial Fill %", randomFillProbability, 0f, 1f);
        iterations = EditorGUILayout.IntSlider("Iterations", iterations, 1, 20);

        EditorGUILayout.Space();

        // Preview controls
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Initialize", GUILayout.Width(100)))
        {
            previewCA.ruleSet = (int[])editingRuleset.Clone();
            previewCA.SetRandomStates(randomFillProbability);
            currentIteration = 0;
            Debug.Log($"CA Initialized: {previewCA.GetXsize()}x{previewCA.GetYsize()}, States: {numStates}, LiveCells: {CountLiveCells()}");
        }
        if (GUILayout.Button("Step", GUILayout.Width(100)))
        {
            previewCA.Update();
            currentIteration++;
            Debug.Log($"Step {currentIteration}: {CountLiveCells()} live cells");
        }
        if (GUILayout.Button("Run All", GUILayout.Width(100)))
        {
            for (int i = 0; i < iterations; i++)
            {
                previewCA.Update();
            }
            currentIteration += iterations;
            Debug.Log($"Ran {iterations} iterations. Now at generation {currentIteration}: {CountLiveCells()} live cells");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField($"Generation: {currentIteration}");

        EditorGUILayout.Space();

        // Ruleset editor
        showRulesetEditor = EditorGUILayout.Foldout(showRulesetEditor, "Edit Ruleset");
        if (showRulesetEditor)
        {
            rulesetScrollPosition = EditorGUILayout.BeginScrollView(rulesetScrollPosition, GUILayout.Height(200));
            
            EditorGUILayout.LabelField($"Neighbourhood Total -> Next State", EditorStyles.miniLabel);
            
            for (int i = 0; i < editingRuleset.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Total {i}", GUILayout.Width(60));
                int newValue = EditorGUILayout.IntSlider(editingRuleset[i], 0, numStates - 1, GUILayout.ExpandWidth(true));
                if (newValue != editingRuleset[i])
                {
                    editingRuleset[i] = newValue;
                    shouldRefresh = true;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply Ruleset"))
            {
                currentRuleset = (int[])editingRuleset.Clone();
                previewCA.ruleSet = (int[])editingRuleset.Clone();
                previewCA.Clear();
                currentIteration = 0;
                shouldRefresh = true;
            }
            if (GUILayout.Button("Random Ruleset"))
            {
                previewCA.SetLambdaRuleset(0.38f);
                editingRuleset = (int[])previewCA.ruleSet.Clone();
                currentRuleset = (int[])editingRuleset.Clone();
                previewCA.Clear();
                currentIteration = 0;
                shouldRefresh = true;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        // Save/Load
        EditorGUILayout.LabelField("Save/Load", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        var customName = EditorGUILayout.TextField("Name", currentRulesetName);
        if (customName != currentRulesetName)
        {
            currentRulesetName = customName;
        }
        
        if (GUILayout.Button("Save", GUILayout.Width(60)))
        {
            customRulesets[currentRulesetName] = (int[])editingRuleset.Clone();
            EditorUtility.DisplayDialog("Saved", $"Ruleset '{currentRulesetName}' saved!", "OK");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Export as JSON"))
        {
            string json = RulesetToJSON(currentRulesetName, editingRuleset);
            EditorGUIUtility.systemCopyBuffer = json;
            EditorUtility.DisplayDialog("Copied", "Ruleset JSON copied to clipboard!", "OK");
        }
        if (GUILayout.Button("Copy to TerraGenerator"))
        {
            CopyRulesetToTerraGenerator(currentRulesetName, editingRuleset);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Preview texture display
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
        
        // Always update the texture to show current CA state
        UpdatePreviewTexture();
        
        Rect textureRect = GUILayoutUtility.GetRect(256, 256);
        if (previewTexture != null)
        {
            GUI.DrawTexture(textureRect, previewTexture, ScaleMode.ScaleToFit);
        }

        EditorGUILayout.EndScrollView();
    }

    private void UpdatePreviewTexture()
    {
        if (previewCA == null || previewTexture == null)
            return;

        Color[] colors = new Color[previewSize.x * previewSize.y];
        
        for (int i = 0; i < previewSize.x; i++)
        {
            for (int j = 0; j < previewSize.y; j++)
            {
                int cellValue = previewCA.GetCell(i, j);
                
                if (cellValue == 0)
                {
                    // Dead cells are nearly black
                    colors[j * previewSize.x + i] = new Color(0.1f, 0.1f, 0.1f, 1f);
                }
                else
                {
                    // Living cells colored by state
                    float hue = (float)(cellValue - 1) / (numStates - 1);
                    colors[j * previewSize.x + i] = Color.HSVToRGB(hue, 0.9f, 0.95f);
                }
            }
        }

        previewTexture.SetPixels(colors);
        previewTexture.Apply();
    }

    private void LoadPresetsFromTerraGenerator(TerraGenerator terraGen)
    {
        // We'll access the rulesets through reflection since they're private
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

    private string RulesetToJSON(string name, int[] ruleset)
    {
        return $"{{ \"name\": \"{name}\", \"ruleset\": [{string.Join(", ", ruleset)}] }}";
    }

    private void CopyRulesetToTerraGenerator(string name, int[] ruleset)
    {
        TerraGenerator terraGen = FindObjectOfType<TerraGenerator>();
        if (terraGen == null)
        {
            EditorUtility.DisplayDialog("Error", "TerraGenerator not found in scene!", "OK");
            return;
        }

        var rulesetField = typeof(TerraGenerator).GetField("rulesets",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (rulesetField != null && rulesetField.GetValue(terraGen) is Dictionary<string, int[]> rulesets)
        {
            rulesets[name] = (int[])ruleset.Clone();
            EditorUtility.DisplayDialog("Success", $"Ruleset '{name}' copied to TerraGenerator!", "OK");
        }
    }

    private int CountLiveCells()
    {
        int count = 0;
        for (int i = 0; i < previewCA.GetXsize(); i++)
        {
            for (int j = 0; j < previewCA.GetYsize(); j++)
            {
                if (previewCA.GetCell(i, j) > 0)
                    count++;
            }
        }
        return count;
    }
}