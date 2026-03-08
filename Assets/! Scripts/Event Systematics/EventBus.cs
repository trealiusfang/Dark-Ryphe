using System.Collections.Generic;
using System;

public static class EventBus 
{
    private static Dictionary<Type, Delegate> assignedActions = new();

    public static void Raise(EventData data)
    {
        Type type = data.GetType();

        if (assignedActions.TryGetValue(type, out Delegate action))
        {
            action?.DynamicInvoke(data);
        }
    }

    public static void Sub<T>(Action<T> action) where T : EventData
    {
        Type type = typeof(T);

        if (assignedActions.ContainsKey(type))
        {
            assignedActions[type] = Delegate.Combine(assignedActions[type], action);
        } else
        {
            assignedActions[type] = action;
        }
    }
    public static void UnSub<T>(Action<T> action) where T : EventData
    {
        Type type = typeof(T);

        if (assignedActions.ContainsKey(type))
        {
            assignedActions[type] = Delegate.Remove(assignedActions[type], action);
        }
    }

    /*public static void AddDictionary<T>(Delegate action) where T : Type
    {
        Type type = typeof(T);

        if (assignedActions.ContainsKey(type))
        {
            assignedActions[type] = Delegate.Combine(assignedActions[type], action);
        }
        else
        {
            assignedActions[type] = action;
        }
    }
    public static void RemoveDictionary<T>(Delegate action)
    {
        Type type = typeof(T);

        if (assignedActions.ContainsKey(type))
        {
            assignedActions[type] = Delegate.Remove(assignedActions[type], action);
        }
    } */
}
