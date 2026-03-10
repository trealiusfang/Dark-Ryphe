using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class EventData
{
    public EventData()
    {

    }
}
public class CombatStartEvent : EventData{ }
public class CombatEndEvent : EventData 
{
    public CharacterTeam winningTeam;
}

public class TurnStartEvent : EventData 
{
    public Character unit;
}

public class AbilitySelectedEvent : EventData
{
    public Character unit;
    public Ability ability;
    public List<Character> targets;
}

public class AbilityUsedEvent : EventData
{
    public Character caster;
    public Ability ability;
    public List<Character> targets;
}
public class AbilityFinishedEvent : EventData
{
    public Character caster;
    public Ability ability;
    public List<Character> targets;
}

public class TargetSelectedEvent : EventData
{
    public Character target;
}

public class DamageEvent : EventData
{
    public Character attacker;
    public Character target;
    public int damage;
}

public class UnitDeathEvent : EventData
{
    public Character unit;
}

public class TurnEndEvent : EventData
{
    public Character unit;
}

public class SFXEvent : EventData 
{
    public string sfx_string;
    public AudioClip sfx_clip;
}
public class MusicEvent : EventData 
{
    public string music_string;
    public AudioClip music_clip;
}

public class BattleTextEvent : EventData
{
    public string text;
    public Vector2 position;
    public TextAnimType textAnimType;
}


