using UnityEngine;

[CreateAssetMenu(fileName = "Island", menuName = "New Island")]
public class Island : ScriptableObject
{
    [Header("Island Info")]
    public string displayName = "Island";
    public Vector2Int location;
    [Header("Generation Settings")]
    public int growthDelay = 5;
    public int lagoonThreshold = 3;
    public string ruleset = "island";
}
