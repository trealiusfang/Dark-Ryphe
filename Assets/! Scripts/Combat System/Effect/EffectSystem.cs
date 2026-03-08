using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EffectSystem : MonoBehaviour
{
    static Queue<Effect> effectQueue = new Queue<Effect>();

    private void Awake()
    {
        EventBus.Sub<CombatEndEvent>(ClearQueue);
    }

    public static void Apply(Effect effect)
    {
        effectQueue.Enqueue(effect);
        Process();
    }

    public static void Process()
    {
        while (effectQueue.Count > 0)
        {
            var effect = effectQueue.Dequeue();

            effect.Resolve(EffectContext.Current);
        }
    }

    public static void ClearQueue(CombatEndEvent ev)
    {
        effectQueue = new Queue<Effect>();
    }
}