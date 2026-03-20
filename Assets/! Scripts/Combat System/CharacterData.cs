using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "unassigned_character", menuName = "Create Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public string alternativeInfo;
    public CombatStats characterStats;
    public List<AbilitySO> Abilities;
    [Header("Visuals")]
    public Sprite baseSprite;
    public bool LookRight = true;
    public List<AnimationClip> AttackAnimations = new List<AnimationClip>();
    public List<CharacterAnimationAndFrequency> IdleAnimations = new List<CharacterAnimationAndFrequency>();
    public List<CharacterAnimationAndFrequency> HurtAnimations =new List<CharacterAnimationAndFrequency>();
    public List<CharacterAnimationAndFrequency> DeathAnimations = new List<CharacterAnimationAndFrequency>();
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

[Serializable]
public class CharacterAnimationAndFrequency
{
    public AnimationClip clip;
    [Range(0.01f,1)]
    public float frequency;
}

public enum CharAnimationType
{
    Idle,
    Attack,
    Hurt,
    Death
}

public enum CharacterTeam
{
    Light,
    Dark
}