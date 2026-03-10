using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorManager : MonoBehaviour
{
    public GameObject selectorPrefab;
    public Transform activeSelectors;
    private List<CharactersAndSelectors> CharactersAndSelectors = new List<CharactersAndSelectors>();
    [SerializeField] private Vector2 offset;
    public void Start()
    {
        EventBus.Sub<UnitDeathEvent>(OnUnitDeath);
        EventBus.Sub<AbilitySelectedEvent>(AbilitySelected);
        EventBus.Sub<AbilityUsedEvent>(OnAbilityUsed);
        EventBus.Sub<TurnStartEvent>(OnTurnStart);
        EventBus.Sub<AbilityFinishedEvent>(AfterAbilityUsed);
    }

    private void OnTurnStart(TurnStartEvent ev)
    {
        addSelector(ev.unit, SelectorType.Self);
    }
    private void AfterAbilityUsed(AbilityFinishedEvent ev)
    {
        addSelector(ev.caster, SelectorType.Self);
    }

    private void OnAbilityUsed(AbilityUsedEvent ev)
    {
        List<CharactersAndSelectors> remover = new List<CharactersAndSelectors>();
        
        foreach (CharactersAndSelectors characterAndSelector in CharactersAndSelectors)
        {
            Destroy(characterAndSelector.selector);
            remover.Add(characterAndSelector);
        }

        foreach (CharactersAndSelectors characterAndSelector in remover)
        {
            CharactersAndSelectors.Remove(characterAndSelector);
        } 
    }

    public void AbilitySelected(AbilitySelectedEvent ev)
    {
        OnAbilityUsed(new AbilityUsedEvent
        {
            caster = ev.unit,
            ability = ev.ability,
            targets = ev.targets,
        });

        SelectorType sendType = SelectorType.None;
        switch (ev.ability.targetType)
        {
            case TargetType.Self:
                sendType = SelectorType.Defensive;
                break;
            case TargetType.SingleEnemy or TargetType.AoEEnemy:
                sendType = SelectorType.Offensive;
                break;
            case TargetType.SingleAlly or TargetType.AoEAlly:
                sendType = SelectorType.Buff;
                break;
            default:
                sendType = SelectorType.Self;
                break;
        }

        ApplySelectorToGroup(ev.unit, ev.targets, ev.unit.Team, sendType);
    }

    public void ApplySelectorToGroup(Character caster ,List<Character> targets, CharacterTeam team, SelectorType type)
    {
        //apply selector to self
        if (type != SelectorType.Self)
        {
            addSelector(caster, SelectorType.Self);
        }

        //apply selectors to targets
        for (int i = 0; i < targets.Count; i++)
        {
            addSelector(targets[i], type);
        }
    }

    void addSelector(Character unit, SelectorType type)
    {
        bool itsSaved = false; GameObject selector = null;
        foreach (CharactersAndSelectors charactersAndSelector in CharactersAndSelectors)
        {
            if (unit == charactersAndSelector.character)
            {
                if (type != charactersAndSelector.selectorType)
                {
                    Destroy(charactersAndSelector.selector);

                    selector = getSelector(unit);
                    charactersAndSelector.selector = selector;
                    charactersAndSelector.selectorType = type;

                    selector.GetComponent<SelectorComponent>().SetSelector(type, unit);
                }
                itsSaved = true;
            }
        }

        if (!itsSaved)
        {
            CharactersAndSelectors newInfo = new CharactersAndSelectors();
            newInfo.character = unit;
            newInfo.selectorType = type;
            selector = getSelector(unit);
            newInfo.selector = selector;

            selector.GetComponent<SelectorComponent>().SetSelector(type, unit);
            CharactersAndSelectors.Add(newInfo);
        }
    }

    void OnUnitDeath(UnitDeathEvent ev)
    {
        foreach (CharactersAndSelectors charactersAndSelector in CharactersAndSelectors)
        {
            if (charactersAndSelector.character == ev.unit)
            {
                Destroy(charactersAndSelector.selector);
                CharactersAndSelectors.Remove(charactersAndSelector);
                break;
            }
        }
    }

    private GameObject getSelector(Character character = null)
    {
        GameObject _selector = null;
        for (int i = 0; i < activeSelectors.childCount; i++)
        {
            if (!activeSelectors.GetChild(i).gameObject.activeSelf)
            {
                _selector = transform.GetChild(i).gameObject;
                _selector.SetActive(true);
            }
        }

        if (_selector != null)
        {
            return _selector;
        } else
        {
            if (character != null)
                _selector = Instantiate(selectorPrefab, new Vector3(character.transform.position.x + offset.x, offset.y), Quaternion.identity, activeSelectors);
            else
                _selector = Instantiate(selectorPrefab, activeSelectors);
            return _selector;
        }
    }

    private void refundSelector(GameObject selector)
    {
        selector.SetActive(false);
    }
}

[Serializable]
public class CharactersAndSelectors
{
    public Character character;
    public SelectorType selectorType;
    public GameObject selector;
}

public enum SelectorType
{
    None,      //Only for storage, never on characters
    Offensive, 
    Defensive, //Mostly for heal
    Buff,
    Self       //Only for unit which is actively playing
}
