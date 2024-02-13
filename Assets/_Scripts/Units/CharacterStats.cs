using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; private set; }

    public Stat damage;
    public Stat armor;

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    

    public virtual void TakeDamage(int damage)
    {
        damage -= armor.GetValue();
        //damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHealth -= damage;
        //Debug.Log(transform.name + " takes " + damage + " damage");
        if(currentHealth>100){
            currentHealth = 100;
        }
        else if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        //Debug.Log(transform.name + " died");
    }
}
