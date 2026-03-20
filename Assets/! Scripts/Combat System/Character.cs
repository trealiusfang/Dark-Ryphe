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
    [HideInInspector] public CombatStats baseStats;
    public Stats currentStats;

    bool dead = false;

    void Awake()
    {
        abilityHolder = GetComponent<AbilityHolder>();
        renderer = GetComponent<CharacterRenderer>();
        effectHolder = GetComponent<EffectHolder>();

        if (charData != null)
        {
            baseStats = charData.characterStats;
            transform.name = charData.characterName + " (Character)";
        }

        currentStats.currentHP = baseStats.maxHP;
        currentStats.currentMana = baseStats.maxMana;

        Sub<CombatStartEvent>(OnCombatStart);
    }

    private void OnCombatStart(CombatStartEvent ev)
    {
        renderer.PlayActionAnimation(CharAnimationType.Idle, 0);
    }

    public void TakeDamage(int dmg)
    {
        currentStats.currentHP -= dmg;
        renderer.PlayRandomActionClip(CharAnimationType.Hurt);

        if (currentStats.currentHP <= 0)
            Die();
    }

    void Die()
    {
        dead = true;
        StartCoroutine(deathFunc());
    }

    private IEnumerator deathFunc()
    {
        yield return
        renderer.DeathVisuals();

        EventBus.Raise(new UnitDeathEvent
        {
            unit = this
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

