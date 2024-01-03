using UnityEngine;
using System.Collections;

public class CharacterStats : MonoBehaviour {

    public int maxHealth = 100;
    public int currentHealth { get; private set; }

    public Stat damage;
    public Stat armour;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        damage -= (int)armour.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);  // if armour val > damage, prevent this resulting in a heal

        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        // this can trigger list of IEffects, like drop loot or trigger cutscene etc
        Debug.Log(transform.name + " died");
        Destroy(gameObject);
    }

}
