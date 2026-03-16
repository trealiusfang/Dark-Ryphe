using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButtonHandler : BusRoute
{
    private Character currentUnit;
    [SerializeField] private List<Button> AbilityButtons = new List<Button>();
    void Awake()
    {
        Sub<TurnStartEvent>(SetButtons);
        Sub<UnitReadyEvent>(CharacterReady);
        Sub<CombatStartEvent>(SetButtons);

        Sub<AbilityUsedEvent>(OnAbilityUsed);
        Sub<AbilityFinishedEvent>(OnAbilityFinished);
        Sub<CombatEndEvent>(OnCombatEnded);

        Sub<AbilitySelectedEvent>(AbilityFirer.AbilitySelected);
        Sub<TargetSelectedEvent>(AbilityFirer.TargetSelected);
    }

    private void Update()
    {
        //for now
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentUnit != null && currentUnit.abilityHolder.GetActiveAbilities().Count > 0)
            EventBus.Raise(new AbilitySelectedEvent { ability = currentUnit.abilityHolder.GetActiveAbilities()[0], unit = currentUnit });
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (currentUnit != null && currentUnit.abilityHolder.GetActiveAbilities().Count > 1)
            EventBus.Raise(new AbilitySelectedEvent { ability = currentUnit.abilityHolder.GetActiveAbilities()[1], unit = currentUnit });
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (currentUnit != null && currentUnit.abilityHolder.GetActiveAbilities().Count > 2)
            EventBus.Raise(new AbilitySelectedEvent { ability = currentUnit.abilityHolder.GetActiveAbilities()[2], unit = currentUnit });
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (currentUnit != null && currentUnit.abilityHolder.GetActiveAbilities().Count > 3)
            EventBus.Raise(new AbilitySelectedEvent { ability = currentUnit.abilityHolder.GetActiveAbilities()[3], unit = currentUnit });
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (currentUnit != null && currentUnit.abilityHolder.GetActiveAbilities().Count > 4)
            EventBus.Raise(new AbilitySelectedEvent { ability = currentUnit.abilityHolder.GetActiveAbilities()[4], unit = currentUnit });
        }
    }

    private void SetButtons(CombatStartEvent ev)
    {
        for (int i = 0; i < AbilityButtons.Count; i++)
        {
            var button = AbilityButtons[i];
            button.onClick.AddListener(() => AbilityButtonPressed(button));
        }
    }

    private void SetButtons(TurnStartEvent ev)
    {
        currentUnit = ev.unit;

        for (int i = 0; i < currentUnit.getActiveAbilities().Count; i++)
        {
            //There are not more than 5 available button spots, we want maximum of 4 active aiblities for now.
            if (i == 5 || i >= AbilityButtons.Count) break;

            ShowButton(AbilityButtons[i]);
            AbilityButtons[i].GetComponent<Image>().sprite = currentUnit.getActiveAbilities()[i].sprite;
            AbilityButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentUnit.getActiveAbilities()[i].abilityName;
        }

        for (int i = currentUnit.getActiveAbilities().Count; i < 5; i++)
        {
            if (i >= AbilityButtons.Count) break;

            HideButton(AbilityButtons[i]);
        }

        LockAllButtons();
    }

    private void CharacterReady(UnitReadyEvent ev)
    {
        EnableAllButtons();
    }

    private void AbilityButtonPressed(Button button)
    {
        if (currentUnit == null) return;

        for (int i = 0; i < AbilityButtons.Count; i++)
        {
            if (button == AbilityButtons[i])
            {
                EventBus.Raise(new AbilitySelectedEvent { ability = currentUnit.abilityHolder.GetActiveAbilities()[i], unit = currentUnit});
            }
        }
    }

    private void LockAllButtons()
    {
        for (int i = 0; i < currentUnit.getActiveAbilities().Count; i++)
        {
            if (i == 5 || i >= AbilityButtons.Count) break;

            AbilityButtons[i].interactable = false;
        }
    }

    private void EnableAllButtons()
    {
        for (int i = 0; i < currentUnit.getActiveAbilities().Count; i++)
        {
            if (i == 5 || i >= AbilityButtons.Count) break;

            //Check if ability is available to CAST
            if (currentUnit.abilityHolder.abilityAvailable(currentUnit.getActiveAbilities()[i]))
            {
                AbilityButtons[i].interactable = true;
            } else
            {
                AbilityButtons[i].interactable = false;
            }
        }
    }

    private void OnCombatEnded(CombatEndEvent ev)
    {
        LockAllButtons();
        for (int i = 0; i < AbilityButtons.Count; i++)
        {
            AbilityButtons[i].onClick.RemoveAllListeners();
        }
    }

    private void OnAbilityUsed(AbilityUsedEvent ev)
    {
        LockAllButtons();
    }

    private void OnAbilityFinished(AbilityFinishedEvent ev)
    {
        EnableAllButtons();
    }

    private void HideButton(Button button)
    {
        button.gameObject.SetActive(false);
    }

    private void ShowButton(Button button)
    {
        button.gameObject.SetActive(true);
    }
}
