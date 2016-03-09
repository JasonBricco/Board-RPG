using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public delegate void EventDelegate(Data data);

public sealed class Event
{
	private event EventDelegate action;

	public void AddListener(EventDelegate listener)
	{
		action += listener;
	}

	public void RemoveListener(EventDelegate listener)
	{
		action -= listener;
	}

	public void Invoke(Data data)
	{
		if (action != null) action.Invoke(data);
	}
}

public sealed class EventManager 
{
	private static Dictionary<string, Event> events = new Dictionary<string, Event>();

	public static void StartListening(string eventName, EventDelegate listener)
	{
		Event thisEvent;

		if (events.TryGetValue(eventName, out thisEvent))
			thisEvent.AddListener(listener);
		else
		{
			thisEvent = new Event();
			thisEvent.AddListener(listener);
			events.Add(eventName, thisEvent);
		}
	}

	public static void StopListening(string eventName, EventDelegate listener)
	{
		Event thisEvent;

		if (events.TryGetValue(eventName, out thisEvent))
			thisEvent.RemoveListener(listener);
	}

	public static void Notify(string eventName, Data data)
	{
		Event thisEvent;

		if (events.TryGetValue(eventName, out thisEvent))
			thisEvent.Invoke(data);
	}
}
