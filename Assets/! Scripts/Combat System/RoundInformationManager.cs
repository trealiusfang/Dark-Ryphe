using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundInformationManager : BusRoute
{
    [SerializeField] private GameObject UpComingAbility;
    [SerializeField] private Animator UCA_Animator;

    private void Awake()
    {
        Sub<AbilityUsedEvent>(onAbilityUsed);
    }

    private void onAbilityUsed(AbilityUsedEvent ev)
    {
        string abilityName = ev.ability.abilityName;
        if (ev.ability.fireType == AbilityFireType.Instant)
        {
            return;
        }

        UpComingAbility.GetComponentInChildren<TextMeshProUGUI>().text = abilityName;
        UCA_Animator.SetTrigger("Fire");
    } 
}
