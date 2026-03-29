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
    public List<PassiveSO> Passives;
    [Header("Visuals")]
    public Sprite baseSprite;
    public bool LookRight = true;
    public Vector2 spriteOffset;
    [Range(.5f, 5)]
    public float DeathTimer = 1;
    [Header("Animations")]
    public List<AnimationClip> AttackAnimations = new List<AnimationClip>();
    public List<CharacterAnimationAndFrequency> IdleAnimations = new List<CharacterAnimationAndFrequency>();
    public List<CharacterAnimationAndFrequency> HurtAnimations =new List<CharacterAnimationAndFrequency>();
    public List<CharacterAnimationAndFrequency> DeathAnimations = new List<CharacterAnimationAndFrequency>();
}
[Serializable]
public class CombatStats
{
    public int maxHP = 30;
    public int maxMana = 10;
    public int manaRegen = 2;
    public int speed = 4;
    public int power = 2;
    public int luck = 3;
    public int accuracy = 100; // => value between 0 - 100
    public int dodge = 0;      // => value between 0 - 100
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