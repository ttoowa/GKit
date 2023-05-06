using System;
using System.Collections.Generic;

namespace GKit;

public class EventDictionary {
    private Dictionary<string, List<Action>> eventDictionary = new();

    public EventDictionary() { }

    public void Clear() { eventDictionary.Clear(); }

    public bool HasListener(string eventName) { return eventDictionary.ContainsKey(eventName); }

    public void AddListener(string eventName, Action listener) {
        if (!eventDictionary.ContainsKey(eventName))
            eventDictionary.Add(eventName, new List<Action>());
        eventDictionary[eventName].Add(listener);
    }

    public void RemoveListener(string eventName, Action listener) {
        if (!eventDictionary.ContainsKey(eventName))
            return;
        eventDictionary[eventName].Remove(listener);

        if (eventDictionary[eventName].Count == 0)
            eventDictionary.Remove(eventName);
    }

    public void ClearListener(string eventName) {
        if (!eventDictionary.ContainsKey(eventName))
            return;
        eventDictionary[eventName].Clear();
        eventDictionary.Remove(eventName);
    }

    public void Invoke(string eventName) {
        if (!eventDictionary.ContainsKey(eventName))
            return;
        foreach (Action listener in eventDictionary[eventName]) { listener?.Invoke(); }
    }
}

public class EventDictionary<T> {
    private Dictionary<string, List<Action<T>>> eventDictionary = new();

    public EventDictionary() { }

    public void Clear() { eventDictionary.Clear(); }

    public bool HasListener(string eventName) { return eventDictionary.ContainsKey(eventName); }

    public void AddListener(string eventName, Action<T> listener) {
        if (!eventDictionary.ContainsKey(eventName))
            eventDictionary.Add(eventName, new List<Action<T>>());
        eventDictionary[eventName].Add(listener);
    }

    public void RemoveListener(string eventName, Action<T> listener) {
        if (!eventDictionary.ContainsKey(eventName))
            return;
        eventDictionary[eventName].Remove(listener);

        if (eventDictionary[eventName].Count == 0)
            eventDictionary.Remove(eventName);
    }

    public void ClearListener(string eventName) {
        if (!eventDictionary.ContainsKey(eventName))
            return;
        eventDictionary[eventName].Clear();
        eventDictionary.Remove(eventName);
    }

    public void Invoke(string eventName, T arg) {
        if (!eventDictionary.ContainsKey(eventName))
            return;
        foreach (Action<T> listener in eventDictionary[eventName]) { listener?.Invoke(arg); }
    }
}