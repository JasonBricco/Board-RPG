//
//  EventManager.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/6/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public delegate void EventDelegate(object data);

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

	public void Invoke(object data)
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

	public static void TriggerEvent(string eventName, object data)
	{
		Event thisEvent;

		if (events.TryGetValue(eventName, out thisEvent))
			thisEvent.Invoke(data);
	}
}
