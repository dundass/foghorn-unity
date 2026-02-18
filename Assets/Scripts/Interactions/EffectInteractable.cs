using System.Collections.Generic;
using UnityEngine;

public class EffectInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] List<IEffect> effects = new List<IEffect>();

    public void Interact(GameObject target = null)
    {
        foreach(IEffect effect in effects)
        {
            effect.Apply(target);   // is this gonna be the right target ?
        }
    }
}
