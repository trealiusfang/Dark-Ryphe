using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityReader : MonoBehaviour
{
    [SerializeField] private Transform abilityHolderUI;
    [SerializeField] private int abilityAmount;
    private void Awake()
    {
        EventBus.Sub<TurnStartEvent>(ReadAbility);
        EventBus.Sub<AbilityUsedEvent>(LockSelection);
        EventBus.Sub<AbilityFinishedEvent>(OpenSelection);
    }
    Character currentCharacter;
    List<Ability> abilities;
    AbilityHolder abilityHolder;
    void ReadAbility(TurnStartEvent ev)
    {
        currentCharacter = ev.unit;
        abilityHolder = ev.unit.GetComponent<AbilityHolder>();
        abilities = abilityHolder.GetActiveAbilities();

        int count = abilities.Count > abilityAmount ? abilityAmount : abilities.Count;
        for (int i = 0; i < count; i++)
        {
            abilityHolderUI.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = abilities[i].abilityName;
            abilityHolderUI.GetChild(i).GetComponent<Image>().sprite = abilities[i].sprite;
        }

        OpenSelection(new AbilityFinishedEvent { });
    }

    public void PressAbility(Button button)
    {
        for (int i = 0; i < abilityHolderUI.childCount; i++)
        {
            if (abilityHolderUI.GetChild(i).GetComponent<Button>() == button && abilities.Count > i)
            {
                FireAbility(abilities[i]);
            }
        }
    }

    private void LockSelection(AbilityUsedEvent ev)
    {
        for (int i = abilityAmount - 1; i >= 0; i--)
        {
            Button abilityButton = abilityHolderUI.GetChild(i).GetComponent<Button>();
            abilityButton.interactable = false;
        }
    }

    private void OpenSelection(AbilityFinishedEvent ev)
    { 
        int count = abilities.Count > abilityAmount ? abilityAmount : abilities.Count;
        for (int i = 0; i < abilityAmount; i++)
        {
            Button abilityButton = abilityHolderUI.GetChild(i).GetComponent<Button>();
            abilityButton.interactable = true;
        }
        for (int i = 0; i < count; i++)
        {
            Button abilityButton = abilityHolderUI.GetChild(i).GetComponent<Button>();

            if (!abilityHolder.abilityAvailable(abilities[i]))
            {
                abilityButton.interactable = false;
            }
        }
        for (int i = abilityAmount - 1; i >= 0; i--)
        {
            Button abilityButton = abilityHolderUI.GetChild(i).GetComponent<Button>();
            if (i >= count)
            {
                abilityButton.interactable = false;
                abilityHolderUI.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }

    public void FireAbility(Ability ability)
    {
        Debug.Log(ability.abilityName + " as " + currentCharacter.name);
        List<Character> targets = TargetSetter.SetTarget(currentCharacter, ability);
        StartCoroutine(ability.Execute(currentCharacter, targets));
    }
}
