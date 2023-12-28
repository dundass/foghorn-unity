using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Stat {

    // todo - serialize might not be needed as loading characters from data, not changing in unity editor
    // todo - StatModifier class to give them a name ...
    // also - probs shouldn't iterate modifiers every time u get value ...

    [SerializeField]
    private float baseValue;

    private List<float> modifiers = new List<float>();

    public float getValue() {
        float finalValue = baseValue;
        modifiers.ForEach(x => finalValue += x);
        return baseValue;
    }

    public void addModifier(float mod) {
        if (mod != 0) modifiers.Add(mod);
    }

    public void removeModifier(float mod) {
        if (mod != 0) modifiers.Remove(mod);
    }

}
