using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButtonHandler : BusRoute
{
    private Character currentUnit;
    [SerializeField] private List<AbilityButton> AbilityButtons = new List<AbilityButton>();
    void Awake()
    {
        Sub<TurnStartEvent>(SetButtons);
        Sub<UnitReadyEvent>(CharacterReady);
        Sub<CombatStartEvent>(SetButtons);

        Sub<AbilitySetChanged>(OnAbilitySetChanged);
        Sub<AbilityUsedEvent>(OnAbilityUsed);
        Sub<AbilityFinishedEvent>(OnAbilityFinished);
        Sub<CombatEndEvent>(OnCombatEnded);

        Sub<AbilitySelectedEvent>(AbilityFirer.AbilitySelected);
        Sub<TargetSelectedEvent>(AbilityFirer.TargetSelected);
    }


    private void SetButtons(CombatStartEvent ev)
    {
        for (int i = 0; i < AbilityButtons.Count; i++)
        {
            AbilityButtons[i].SetButton(this);
        }
    }

    private void SetButtons(TurnStartEvent ev)
    {
        currentUnit = ev.unit;

        buttonsReady();

        LockAllButtons();
    }

    private void CharacterReady(UnitReadyEvent ev)
    {
        EnableAllButtons();
    }

    public void AbilityButtonPressed(Ability ability)
    {
        if (currentUnit == null || ability == null) return;
        EventBus.Raise(new AbilitySelectedEvent { ability = ability, unit = currentUnit });
    }

    private void LockAllButtons()
    {
        for (int i = 0; i < currentUnit.getActiveAbilities().Count; i++)
        {
            if (i == 5 || i >= AbilityButtons.Count) break;

            AbilityButtons[i].GetComponent<Button>().interactable = false;
        }
    }

    private void EnableAllButtons()
    {
        if (currentUnit == null) return;

        for (int i = 0; i < currentUnit.getActiveAbilities().Count; i++)
        {
            if (i == 5 || i >= AbilityButtons.Count) break;

            //Check if ability is available to CAST
            if (currentUnit.abilityHolder.abilityAvailable(currentUnit.getActiveAbilities()[i]))
            {
                AbilityButtons[i].GetComponent<Button>().interactable = true;
            } else
            {
                AbilityButtons[i].GetComponent<Button>().interactable = false;
            }
        }
    }
    private void buttonsReady()
    {
        if (currentUnit == null) return;

        for (int i = 0; i < currentUnit.getActiveAbilities().Count; i++)
        {
            //There are not more than 5 available button spots, we want maximum of 4 active aiblities for now.
            if (i == 5 || i >= AbilityButtons.Count || currentUnit.getActiveAbilities()[i] == null) break;

            AbilityButtons[i].SetAbilityInfo(currentUnit.getActiveAbilities()[i], currentUnit);
            ShowButton(AbilityButtons[i]);
        }

        for (int i = currentUnit.getActiveAbilities().Count; i < 5; i++)
        {
            if (i >= AbilityButtons.Count) break;

            HideButton(AbilityButtons[i]);
        }
    }


    private void OnCombatEnded(CombatEndEvent ev)
    {
        LockAllButtons();
        for (int i = 0; i < AbilityButtons.Count; i++)
        {
            AbilityButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void OnAbilityUsed(AbilityUsedEvent ev)
    {
        LockAllButtons();
    }

    private void OnAbilitySetChanged(AbilitySetChanged ev)
    {
        if (currentUnit == ev.unit)
        {
            buttonsReady();

            if (ev.selectionEnabled) EnableAllButtons();
        }
    }

    private void OnAbilityFinished(AbilityFinishedEvent ev)
    {
        EnableAllButtons();
    }

    private void HideButton(AbilityButton button)
    {
        button.gameObject.SetActive(false);
    }

    private void ShowButton(AbilityButton button)
    {
        button.gameObject.SetActive(true);
    }
}
