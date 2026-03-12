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
    }

    private void AbilityFired()
    {

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

    private void HideButton(Button button)
    {
        button.gameObject.SetActive(false);
    }

    private void ShowButton(Button button)
    {
        button.gameObject.SetActive(true);
    }
}
