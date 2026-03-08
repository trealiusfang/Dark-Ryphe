using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BusRoute : MonoBehaviour
{
    /*
    private static List<EventData> assignedTypes = new();
    private static List<Delegate> assignedActions = new();
    public static void Sub<T>(Action<T> action) where T : EventData
    {
        Type type = typeof(T);

        assignedTypes.Add(typeof(T));
        assignedActions.Add(action);

        EventBus.Sub<T>(action);
    }
    public static void UnSub<T>(Action<T> action) where T : EventData
    {
        Type type = typeof(T);

        assignedTypes.Remove(type);
        assignedActions.Remove(action);

        EventBus.UnSub<T>(action);
    }

    public virtual void OnEnable()
    {
        for (int i = 0; i < assignedActions.Count; i++)
        {
            Delegate del = assignedActions[i];
            
            EventBus.AddDictionary<Type>(del);
        }
    }
    */
}
