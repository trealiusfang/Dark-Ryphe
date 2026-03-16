using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPositioner : BusRoute
{
    [Header("Configurations")]
    public float centerDistance = 1.5f;
    public float perCharacterDistance = 1;
    public float groundDistance = 1;
    public bool resetpositioning;
    private void Awake()
    {
        Sub<CombatStartEvent>(SetArea);
        Sub<UnitDeathEvent>(OnUnitDeath);
    }

    private void Update()
    {
        if (resetpositioning)
        {
            SetAll();
            resetpositioning = false;
        }
    }

    private void SetArea(CombatStartEvent ev)
    {
        StartCoroutine(ExecuteStart());
    }

    private IEnumerator ExecuteStart()
    {
        yield return new WaitForSeconds(.1f);
        CharacterLister poses = GameInitializer.instance._combatManagers.GetComponent<CharacterLister>();

        //These should be down with effects
        for (int i = 0; i < poses.LightCharacters().Count; i++)
        {
            Character currentCharacter = poses.LightCharacters()[i];

            if (currentCharacter == null) continue;
            ResetPosition(currentCharacter);
        }

        for (int i = 0; i < poses.DarkCharacters().Count; i++)
        {
            Character currentCharacter = poses.DarkCharacters()[i];

            if (currentCharacter == null) continue;
            ResetPosition(currentCharacter);
        }
    }

    private void SetAll()
    {
        CharacterLister poses = GameInitializer.instance._combatManagers.GetComponent<CharacterLister>();

        //These should be down with effects
        for (int i = 0; i < poses.LightCharacters().Count; i++)
        {
            Character currentCharacter = poses.LightCharacters()[i];

            if (currentCharacter == null) continue;
            ResetPosition(currentCharacter);
        }

        for (int i = 0; i < poses.DarkCharacters().Count; i++)
        {
            Character currentCharacter = poses.DarkCharacters()[i];

            if (currentCharacter == null) continue;
            ResetPosition(currentCharacter);
        }
    }

    private void OnUnitDeath(UnitDeathEvent ev)
    {
        StartCoroutine(RepositionPostMortem(ev.unit.Team));
    }
    private IEnumerator RepositionPostMortem(CharacterTeam team)
    {
        CharacterLister poses = GameInitializer.instance._combatManagers.GetComponent<CharacterLister>();
        List<Character> characters = team == CharacterTeam.Light ? poses.LightCharacters() : poses.DarkCharacters();
        for (int i = 0; i < characters.Count; i++)
        {
            Character currentCharacter = characters[i];

            if (currentCharacter == null) continue;
            ResetPosition(currentCharacter);
        }

        yield return null;
    }

    public void ResetPosition(Character character)
    {
        if (character == null) return;

        CharacterLister poses = GameInitializer.instance._combatManagers.GetComponent<CharacterLister>();
        List<Character> characters = character.Team == CharacterTeam.Light ? poses.LightCharacters() : poses.DarkCharacters();

        for (int i = 0; i < characters.Count; i++)
        {
            if (character == characters[i])
            {
                float tallness = character.GetComponent<SpriteRenderer>().bounds.size.y;

                character.transform.position = new Vector3((centerDistance + (perCharacterDistance * i)) * (character.Team == CharacterTeam.Dark ? 1 : -1), (tallness / 2) - groundDistance, character.transform.position.z);
                break;
            }
        }
    }

    public void PushPull(Character character, int howMuch)
    {
        CharacterLister poses = GameInitializer.instance._combatManagers.GetComponent<CharacterLister>();
        List<Character> characters = character.Team == CharacterTeam.Light ? poses.LightCharacters() : poses.DarkCharacters();

        for (int i = 0; i < characters.Count; i++)
        {
            if (character == characters[i])
            {
                int index = i + howMuch;

                if (index < characters.Count && index > -1)
                {
                    characters.Remove(character);
                    characters.Insert(index ,character);
                    poses.SetTeam(character.Team, characters);
                    break;
                }
            }
        }

        SetAll();
    }

    public Vector2 getPosition(Character character)
    {
        if (character == null) return Vector2.zero;

        CharacterLister poses = GameInitializer.instance._combatManagers.GetComponent<CharacterLister>();
        List<Character> characters = character.Team == CharacterTeam.Light ? poses.LightCharacters() : poses.DarkCharacters();
        Vector3 savedVector = Vector2.zero;

        for (int i = 0; i < characters.Count; i++)
        {
            if (character == characters[i])
            {
                float tallness = character.GetComponent<SpriteRenderer>().bounds.size.y;

               savedVector = new Vector3((centerDistance + (perCharacterDistance * i)) * (character.Team == CharacterTeam.Dark ? 1 : -1), (tallness / 2) - groundDistance, character.transform.position.z);
                break;
            }
        }

        return savedVector;
    }
}
