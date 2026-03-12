using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for (mostly) manager scripts to easily set them up with only one Sub function. 
/// Meaning, if BusRoute is used, you won't need to set them up in OnEnable and OnDisable everytime.
/// </summary>
public abstract class BusRoute : MonoBehaviour
{
    private List<(Type type, Delegate action)> routes = new();

    /// <summary>
    /// Reference when called on awake
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    protected void Sub<T>(Action<T> action) where T : EventData
    {
        routes.Add((typeof(T), action));
    }
    /// <summary>
    /// Reference if not called on awake
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    protected void SubnApply<T>(Action<T> action) where T : EventData
    {
        routes.Add((typeof(T), action));
        EventBus.Sub(action);
    }

    protected void UnSub<T>(Action<T> action) where T : EventData
    {
        routes.RemoveAll(r => r.type == typeof(T) && r.action.Equals(action));
    }
    protected void UnSubnApply<T>(Action<T> action) where T : EventData
    {
        routes.RemoveAll(r => r.type == typeof(T) && r.action.Equals(action));
        EventBus.UnSub(action);
    }

    protected virtual void OnEnable()
    {
        foreach (var route in routes)
        {
            var method = typeof(EventBus)
                .GetMethod(nameof(EventBus.Sub))
                .MakeGenericMethod(route.type);

            method.Invoke(null, new object[] { route.action });
        }
    }

    protected virtual void OnDisable()
    {
        foreach (var route in routes)
        {
            var method = typeof(EventBus)
                .GetMethod(nameof(EventBus.UnSub))
                .MakeGenericMethod(route.type);

            method.Invoke(null, new object[] { route.action });
        }
    }
}
