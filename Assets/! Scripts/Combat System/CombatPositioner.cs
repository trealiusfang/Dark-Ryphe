using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPositioner : BusRoute
{
    [Header("Configurations")]
    public float centerDistance = 1.5f;
    public float perCharacterDistance = 1;
    public float groundDistance = 1;
    public float charMoveSpeed = 3;
    [Header("Debug")]
    public bool resetpositioning;

    List<Character> UnitsOnTheMove = new List<Character>();
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
            currentCharacter.renderer.spriteRenderer.flipX = !currentCharacter.charData.LookRight;

            if (currentCharacter == null) continue;
            ResetPosition(currentCharacter);
        }

        for (int i = 0; i < poses.DarkCharacters().Count; i++)
        {
            Character currentCharacter = poses.DarkCharacters()[i];
            currentCharacter.renderer.spriteRenderer.flipX = currentCharacter.charData.LookRight;

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

        if (UnitsOnTheMove.Contains(character))
        {
            StopCoroutine(resetCoroutine(character));
            StartCoroutine(resetCoroutine(character));
        } else
        {
            UnitsOnTheMove.Add(character);
            StartCoroutine(resetCoroutine(character));
        }
    }

    private IEnumerator resetCoroutine(Character character)
    {
        CharacterLister poses = GameInitializer.instance._combatManagers.GetComponent<CharacterLister>();
        List<Character> characters = character.Team == CharacterTeam.Light ? poses.LightCharacters() : poses.DarkCharacters();

        Vector3 startPos = character.transform.position;
        Vector3 wantedPosition = Vector2.zero;
        for (int i = 0; i < characters.Count; i++)
        {
            if (character == characters[i])
            {
                float tallness = character.renderer.spriteRenderer.bounds.size.y;

                wantedPosition = new Vector3((centerDistance + (perCharacterDistance * i)) * (character.Team == CharacterTeam.Dark ? 1 : -1), (tallness / 2) - groundDistance, character.transform.position.z);
                break;
            }
        }

        float duration = 1f;
        float timeElapsed = 0f;


        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            t = 1f - Mathf.Pow(1f - t, 3f);

            character.transform.position = Vector3.Lerp(startPos, wantedPosition, t);

            timeElapsed += Time.deltaTime * charMoveSpeed;
            yield return null;
        }

        transform.position = wantedPosition;

        if (UnitsOnTheMove.Contains(character)) UnitsOnTheMove.Remove(character);
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
                float tallness = character.renderer.spriteRenderer.bounds.size.y;

               savedVector = new Vector3((centerDistance + (perCharacterDistance * i)) * (character.Team == CharacterTeam.Dark ? 1 : -1), (tallness / 2) - groundDistance, character.transform.position.z);
                break;
            }
        }

        return savedVector;
    }
}
