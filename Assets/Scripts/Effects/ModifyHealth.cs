using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyHealth : MonoBehaviour, IEffect
{
    public void Apply(GameObject target)
    {
        CharacterStats stats = target.GetComponent<CharacterStats>();   // todo - make generic for any Stats (and make Stats Interface/abstract class!)
        if (stats != null)
        {
            //stats.health.
            // ummm change it somehow ?? not just add/remove a modifier...
        }
    }
}
