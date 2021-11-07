using System;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    IncrementAchievementProgress = 0,
    SetAchievementProgress = 1,
    ResetAchievementProgress = 2,
    EnableStepCounter = 3,
    TogglePinButtons = 4
}

public static class EventManager<T, U>
{
    private static readonly Dictionary<EventType, Action<T, U>> EventDictionary = new Dictionary<EventType, Action<T, U>>();

    public static void AddListener(EventType type, Action<T, U> action)
    {
        if (!EventDictionary.ContainsKey(type))
        {
            EventDictionary.Add(type, null);
        }

        // If an Exception has an EventManager<>.Invoke() call at its root, chances are a delegate
        // has been subscribed to the EventType twice, e.g. was not removed for every time it was added
        EventDictionary[type] += action;
    }

    public static void RemoveListener(EventType type, Action<T, U> action)
    {
        if (EventDictionary.ContainsKey(type) && EventDictionary[type] != null)
        {
            // ReSharper disable once DelegateSubtraction
            EventDictionary[type] -= action;
        }
    }

    public static void Invoke(EventType type, T arg1, U arg2)
    {
        if (EventDictionary.ContainsKey(type))
        {
            EventDictionary[type]?.Invoke(arg1, arg2); 
        }
        // else
        // {
        //     Debug.LogWarning("No listener registered for event ["+type+"]");
        // }
    }
}