using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    AbilityButtonHandler buttonHandler;
    Ability currentAbility;
    public void SetButton(AbilityButtonHandler handler)
    {
        buttonHandler = handler;
    }
    public void SetAbilityInfo(Ability newAbility)
    {
        currentAbility = newAbility;    
    }

    public void OnButtonPressed()
    {
        if (buttonHandler != null)
        {
            buttonHandler.AbilityButtonPressed(currentAbility);
        }
    }

    private void ShowAblityInfo()
    {
        if (currentAbility == null) return;
        //SHOW ABILITY INFO
    }

    public void OnPointerEnter(PointerEventData data)
    {
        ShowAblityInfo();
    }
    public void OnPointerExit(PointerEventData data)
    {

    }
}
