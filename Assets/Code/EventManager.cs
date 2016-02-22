using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public delegate void EventDelegate(int data);

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

	public void Invoke(int data)
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

	public static void TriggerEvent(string eventName, int data = 0)
	{
		Event thisEvent;

		if (events.TryGetValue(eventName, out thisEvent))
			thisEvent.Invoke(data);
	}
}
