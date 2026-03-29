using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : BusRoute, IInspectable
{
    public CharacterData charData;
    public CharacterTeam Team;
    [HideInInspector] public AbilityHolder abilityHolder;
    [HideInInspector] public EffectHolder effectHolder;
    [HideInInspector] public new CharacterRenderer renderer;
    [HideInInspector] public PassiveHolder passiveHolder;
    [SerializeField] private CombatStats baseStats;
    [SerializeField] private Stats currentStats;

    bool dead = false;

    void Awake()
    {
        abilityHolder = GetComponent<AbilityHolder>();
        renderer = GetComponent<CharacterRenderer>();
        effectHolder = GetComponent<EffectHolder>();
        passiveHolder = GetComponent<PassiveHolder>();

        if (charData != null)
        {
            baseStats = charData.characterStats;
            transform.name = charData.characterName + " (Character)";
        }

        currentStats.currentHP = baseStats.maxHP;
        currentStats.currentMana = baseStats.maxMana;

        Sub<CombatStartEvent>(OnCombatStart);
    }

    public float GetStat(statType StatType)
    {
        float value = 0;
        
        //Convert state type to base values
        switch (StatType)
        {
            case statType.HP_Current:
                value = currentStats.currentHP;
                break;
            case statType.HP_Max:
                value = baseStats.maxHP;
                break;
            case statType.Mana_Current:
                value = currentStats.currentMana;
                break;
            case statType.Mana_Max:
                value = baseStats.maxMana;
                break;
            case statType.Mana_Regen:
                value = baseStats.manaRegen;
                break;
            case statType.Power:
                value = baseStats.power;
                break;
            case statType.Luck:
                value = baseStats.luck;
                break;
            case statType.Speed:
                value = baseStats.speed;
                break;
        }

        if (StatType == statType.HP_Current) Debug.Log("pre HP: " + value);
        //check for status effects
        foreach(Effect effect in effectHolder.GetEffects())
        {
            value = effect.statCalc(StatType, value);
        }

        //check for passives
        foreach (Passive passive in passiveHolder.GetPassives())
        {
            value = passive.statCalc(StatType, value);
        }

        if (StatType == statType.HP_Current) Debug.Log("cur HP: " + value);
        return value;
    }
    public int GetStatFloor(statType StatType)
    {
        float value = 0;

        //Convert state type to base values
        switch (StatType)
        {
            case statType.HP_Current:
                value = currentStats.currentHP;
                break;
            case statType.HP_Max:
                value = baseStats.maxHP;
                break;
            case statType.Mana_Current:
                value = currentStats.currentMana;
                break;
            case statType.Mana_Max:
                value = baseStats.maxMana;
                break;
            case statType.Mana_Regen:
                value = baseStats.manaRegen;
                break;
            case statType.Power:
                value = baseStats.power;
                break;
            case statType.Luck:
                value = baseStats.luck;
                break;
            case statType.Speed:
                value = baseStats.speed;
                break;
        }

        //check for status effects
        foreach (Effect effect in effectHolder.GetEffects())
        {
            value = effect.statCalc(StatType, value);
        }

        //check for passives
        foreach (Passive passive in passiveHolder.GetPassives())
        {
            value = passive.statCalc(StatType, value);
        }

        return Mathf.FloorToInt(value);
    }

    public int GetBaseStat(statType StatType)
    {
        int value = 0;

        //Convert state type to base values
        switch (StatType)
        {
            case statType.HP_Current:
                value = currentStats.currentHP;
                break;
            case statType.HP_Max:
                value = baseStats.maxHP;
                break;
            case statType.Mana_Current:
                value = currentStats.currentMana;
                break;
            case statType.Mana_Max:
                value = baseStats.maxMana;
                break;
            case statType.Mana_Regen:
                value = baseStats.manaRegen;
                break;
            case statType.Power:
                value = baseStats.power;
                break;
            case statType.Luck:
                value = baseStats.luck;
                break;
            case statType.Speed:
                value = baseStats.speed;
                break;
        }

        return value;
    }

    public void ChangeStat(statType StatType, int value)
    {
        foreach (Passive p in passiveHolder.GetPassives())
        {
            value = p.statChange(StatType, value);
        }

        switch (StatType)
        {
            case statType.HP_Current:
                currentStats.currentHP += value;
                break;
            case statType.HP_Max:
                baseStats.maxHP += value;
                break;
            case statType.Mana_Current:
                currentStats.currentMana += value;
                break;
            case statType.Mana_Max:
                baseStats.maxMana += value;
                break;
            case statType.Mana_Regen:
                baseStats.manaRegen += value;
                break;
            case statType.Power:
                baseStats.power += value;
                break;
            case statType.Luck:
                baseStats.luck += value;
                break;
            case statType.Speed:
                baseStats.speed += value;
                break;
        }
    }

    private void OnCombatStart(CombatStartEvent ev)
    {
        renderer.PlayActionAnimation(CharAnimationType.Idle, 0);
    }

    public void TakeDamage(int dmg, Character causer)
    {
        currentStats.currentHP -= dmg;
        renderer.PlayRandomActionClip(CharAnimationType.Hurt);

        if (currentStats.currentHP <= 0)
            Die(causer);
    }

    private void Die(Character causer)
    {
        dead = true;
        StartCoroutine(deathFunc(causer));
    }

    private IEnumerator deathFunc(Character causer)
    {
        yield return
        renderer.DeathVisuals();

        EventBus.Raise(new UnitDeathEvent
        {
            unit = this,
            causer = causer,
        });

        yield return new WaitForSeconds(.1f);
        Destroy(gameObject);
    }

    public bool isDead()
    {
        return dead;
    }

    public List<Ability> getActiveAbilities()
    {
        return abilityHolder.GetActiveAbilities();
    }
    public List<Ability> getAllAbilities()
    {
        return abilityHolder.GetAllAbilities();
    }

    public void OnInspect()
    {
        EventBus.Raise(new InspectedCharacterEvent { character = this });
    }
}

public enum statType
{
    HP_Current,
    HP_Max,
    Mana_Current,
    Mana_Max,
    Mana_Regen,
    Power,
    Luck,
    Speed,
}

