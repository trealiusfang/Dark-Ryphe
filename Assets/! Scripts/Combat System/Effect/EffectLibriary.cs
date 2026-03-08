using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectLibrary
{
    public class DamageEffect : Effect
    {
        public int Amount;

        public DamageEffect(Character caster, Character target, int amount)
        {
            Source = caster;
            Target = target;
            Amount = amount;
        }

        public override void Resolve(EffectContext context)
        {
            Target.TakeDamage(Amount);

            context.Notify(EffectType.DamageDealt, this);
        }
    }
}
