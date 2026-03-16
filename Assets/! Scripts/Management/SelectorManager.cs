using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorManager : BusRoute
{
    public GameObject selectorPrefab;
    public Transform activeSelectors;
    private List<CharactersAndSelectors> CharactersAndSelectors = new List<CharactersAndSelectors>();
    [SerializeField] private Vector3 offset;

    private Character character;
    private Ability ability;
    public void Awake()
    {
        Sub<UnitDeathEvent>(OnUnitDeath);
        Sub<AbilitySelectedEvent>(AbilitySelected);
        Sub<AbilityUsedEvent>(OnAbilityUsed);
        Sub<TurnStartEvent>(OnTurnStart);
        Sub<AbilityFinishedEvent>(AfterAbilityUsed);
    }
    private void LateUpdate()
    {
        RectTransform canvasRect = activeSelectors as RectTransform;

        for (int i = 0; i < CharactersAndSelectors.Count; i++)
        {
            CharactersAndSelectors item = CharactersAndSelectors[i];

            if (item.character == null || item.selector == null)
                continue;

            RectTransform selectorRect = item.selector.GetComponent<RectTransform>();

            Vector3 worldPos = new Vector3(
                item.character.transform.position.x,
                0,
                item.character.transform.position.z
            ) + new Vector3(offset.x, offset.y, offset.z);

            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);

            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPoint,
                Camera.main,
                out canvasPos
            );

            selectorRect.localPosition = canvasPos;
        }
    }

    private void OnTurnStart(TurnStartEvent ev)
    {
        addSelector(ev.unit, SelectorType.Self);
    }
    private void AfterAbilityUsed(AbilityFinishedEvent ev)
    {
        addSelector(ev.caster, SelectorType.Self);

        if (character != null && ability != null)
        {
            if (character.abilityHolder.abilityAvailable(ability))
            {
                AbilitySelected(new AbilitySelectedEvent { ability = ability, unit = character });
            }
        }
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
        List<Character> targets = TargetSetter.SetTarget(ev.unit, ev.ability);

        character = ev.unit;
        ability = ev.ability;
        OnAbilityUsed(new AbilityUsedEvent
        {
            caster = ev.unit,
            ability = ev.ability,
            targets = targets
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
            case TargetType.SingleAll or TargetType.AoEAll:
                sendType = SelectorType.Buff;
                break;
            default:
                sendType = SelectorType.Self;
                break;
        }

        ApplySelectorToGroup(ev.unit, targets, ev.unit.Team, sendType);
    }

    public void ApplySelectorToGroup(Character caster ,List<Character> targets, CharacterTeam team, SelectorType type)
    {
        if (caster == null || caster.isDead()) return;
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
        if (unit == null || unit.isDead()) return;
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
        if (_selector == null)
        {
            _selector = Instantiate(selectorPrefab, activeSelectors);
        }
        if (character != null)
        {
            RectTransform canvasRect = activeSelectors as RectTransform;
            RectTransform selectorRect = _selector.GetComponent<RectTransform>();

            Vector3 worldPos = new Vector3(character.transform.position.x, 0, character.transform.position.z) + new Vector3(offset.x, offset.y, offset.z);

            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);

            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPoint,
                Camera.main,
                out canvasPos
            );

            selectorRect.localPosition = canvasPos;
        }

        return _selector;
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
