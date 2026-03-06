using System;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class Character : MonoBehaviour
{
    public List<Ability> abilities = new();

    public CombatStats baseStats;
    public Stats stats;
    public float currentHP;
    bool canPlay = false;
    void Awake()
    {
        currentHP = baseStats.maxHP;
    }

    private void StartTurn(TurnStartEvent turnStartEvent)
    {
        canPlay = true;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        EventBus.Raise(new UnitDeathEvent
        {
            unit = this
        });

        Destroy(gameObject);
    }
}

[Serializable]
public class CombatStats
{
    public short maxHP = 30;
    public short maxMana = 10;
    public short manaRegen = 2;
    public short speed = 4;
    public short power = 2;
    public short luck = 3;
}

[Serializable]
public class Stats
{
    public short currentHP = 30;
    public short currentMana = 10;
}