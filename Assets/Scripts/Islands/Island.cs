using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Island", menuName = "New Island")]
public class Island : ScriptableObject
{
    public string displayName;
    public Vector2Int location;
}
