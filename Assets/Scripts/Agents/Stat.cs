using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Stat {

    // todo - int rather than float ?
    // todo - StatModifier class to give them a name ...
    // also - probs shouldn't iterate modifiers every time u get value ...

    [SerializeField]
    private float baseValue;

    private List<float> _modifiers = new List<float>();

    public float GetValue() {
        float finalValue = baseValue;
        _modifiers.ForEach(x => finalValue += x);
        return baseValue;
    }

    public void AddModifier(float mod) {
        if (mod != 0) _modifiers.Add(mod);
    }

    public void RemoveModifier(float mod) {
        if (mod != 0) _modifiers.Remove(mod);
    }

}
