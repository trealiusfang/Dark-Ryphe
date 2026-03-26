using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    AbilityButtonHandler buttonHandler;
    Ability currentAbility;
    Character master;
    [SerializeField] private TextMeshProUGUI manaCost;
    [SerializeField] private TextMeshProUGUI abilityName;
    public void SetButton(AbilityButtonHandler handler)
    {
        buttonHandler = handler;
    }
    public void SetAbilityInfo(Ability newAbility, Character unit)
    {
        currentAbility = newAbility;
        master = unit;
        GetComponent<Image>().sprite = newAbility.sprite;

        if (newAbility.sprite != null)
        {
            abilityName.gameObject.SetActive(false);
        } else
        {
            abilityName.gameObject.SetActive(true);
            abilityName.text = newAbility.abilityName;
        }

        if (newAbility.manaCost > 0)
        {
            manaCost.gameObject.SetActive(true);
            manaCost.text = newAbility.manaCost.ToString();
        }
        else
        {
            manaCost.gameObject.SetActive(false);
        }
    }

    public void OnButtonPressed()
    {
        if (buttonHandler != null)
        {
            buttonHandler.AbilityButtonPressed(currentAbility);
        }
    }


    public void OnPointerEnter(PointerEventData data)
    {
        EventBus.Raise(new AbilityHoverEvent { ability = currentAbility, character = master, intoHover = true});
    }
    public void OnPointerExit(PointerEventData data)
    {
        EventBus.Raise(new AbilityHoverEvent { ability = currentAbility, character = master, intoHover = false });
    }
}
