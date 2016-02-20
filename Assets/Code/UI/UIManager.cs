//
//  UIManager.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/4/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections.Generic;

public sealed class UIManager : MonoBehaviour
{
	[SerializeField] private GameObject[] graphicObjects;

	private static Dictionary<string, GameObject> graphics = new Dictionary<string, GameObject>();

	private string eventToCall;

	private void Awake()
	{
		for (int i = 0; i < graphicObjects.Length; i++)
			graphics.Add(graphicObjects[i].name, graphicObjects[i]);

		EventManager.StartListening("StateChanged", StateChangedHandler);
	}

	private void StateChangedHandler(int state)
	{
		switch ((GameState)state)
		{
		case GameState.Editing:
			GetGraphic("MainButtons").SetActive(true);
			break;

		case GameState.Playing:
			GetGraphic("MainButtons").SetActive(false);
			break;
		}
	}

	public void ButtonPressedPrimer(string buttonEvent)
	{
		eventToCall = buttonEvent;
	}

	public void ButtonPressed(int data)
	{
		EventManager.TriggerEvent(eventToCall, data);
	}

	public static bool IsActive(string name)
	{
		return GetGraphic(name).activeSelf;
	}

	public static void EnableGraphic(string name)
	{
		GetGraphic(name).SetActive(true);
	}

	public static bool DisableGraphic(string name)
	{
		GameObject graphic = GetGraphic(name);

		if (graphic.activeSelf)
		{
			graphic.SetActive(false);
			return true;
		}

		return false;
	}

	public static void EnableWindow(string name)
	{
		EnableGraphic(name);
		StateManager.ChangeState(GameState.Window);
	}

	public static void DisableWindow(string name, GameState returnState)
	{
		if (DisableGraphic(name))
			StateManager.ChangeState(returnState);
	}

	public static void ToggleWindow(string name, GameState returnState)
	{
		GameObject window = GetGraphic(name);

		if (window.activeSelf)
		{
			window.SetActive(false);
			StateManager.ChangeState(returnState);
		}
		else
		{
			window.SetActive(true);
			StateManager.ChangeState(GameState.Window);
		}
	}

	public static void SetText(string name, string text)
	{
		Text textObj = GetGraphic(name).GetComponent<Text>();
		Assert.IsNotNull<Text>(textObj, "There is no text component on object: " + name);
		textObj.text = text;
	}

	public static void SetInteractable(string name, bool interactable)
	{
		Button button = GetGraphic(name).GetComponent<Button>();
		Assert.IsNotNull<Button>(button, "There is no button component on object: " + name);
		button.interactable = interactable;
	}

	public static GameObject GetGraphic(string name)
	{
		GameObject graphic;

		if (graphics.TryGetValue(name, out graphic))
			return graphic;
		else
		{
			Debug.LogError("Could not find a graphic with the name: " + name);
			return null;
		}
	}

	public static T GetGraphic<T>(string name)
	{
		return GetGraphic(name).GetComponent<T>();
	}
}
