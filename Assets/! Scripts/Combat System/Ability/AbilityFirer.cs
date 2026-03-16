using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
public class AbilityFirer
{
    private static Character lastCaster;
    private static List<Character> lastTargets;
    private static Ability lastAbility;
    public static void FireAbility(Ability ability, Character caster, Character target)
    {
        if (ability == null ||caster == null)
        {
            Debug.WriteLine("Missing crucial ability information!");
            return;
        }

        List<Character> targets = TargetSetter.SetTarget(caster, ability);

        if (target != null)
        {
            if (ability.targetType == TargetType.SingleEnemy || ability.targetType == TargetType.SingleAlly || ability.targetType == TargetType.SingleAll)
            {
                targets = new List<Character> { target };
            }
        }

        caster.abilityHolder.NotifyCooldownChecks(ability);

        GameInitializer.instance.StartCoroutine(ability.Execute(caster, targets, ability));

        lastAbility = ability;
        lastTargets = targets;
        lastCaster = caster;
    }

    public static void StopLastUsedAbility()
    {
        if (lastCaster == null || lastTargets == null || lastAbility == null) return;
        GameInitializer.instance.StopCoroutine(lastAbility.Execute(lastCaster, lastTargets, lastAbility));
    }

    public static void AbilitySelected(AbilitySelectedEvent ev)
    {
        lastCaster = ev.unit;
        lastAbility = ev.ability;

        if (ev.ability.fireType == AbilityFireType.Instant)
        {

            FireAbility(ev.ability, ev.unit, null);
        }
    }

    public static void TargetSelected(TargetSelectedEvent ev)
    {
        if (lastCaster == null || lastAbility == null) return;
        FireAbility(lastAbility, lastCaster, ev.target);
    }
}
