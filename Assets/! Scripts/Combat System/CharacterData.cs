using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "unassigned_character", menuName = "Create Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite charSprite;
    public CombatStats characterStats;
    public List<AbilitySO> Abilities;
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
    public int currentHP = 30;
    public int currentMana = 10;
}

public enum CharacterTeam
{
    Light,
    Dark
}