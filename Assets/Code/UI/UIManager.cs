//
//  UIManager.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/4/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;

public sealed class UIManager 
{
	private Dictionary<string, GameObject> graphics = new Dictionary<string, GameObject>();

	public UIManager(GameObject[] graphics)
	{
		for (int i = 0; i < graphics.Length; i++)
			this.graphics.Add(graphics[i].name, graphics[i]);

		EventManager.StartListening("StateChanged", StateChangedHandler);
	}

	public GameObject GetGraphic(string name)
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

	public T GetGraphic<T>(string name)
	{
		return GetGraphic(name).GetComponent<T>();
	}

	public void EnableGraphic(string name)
	{
		GetGraphic(name).SetActive(true);
	}

	public void DisableGraphic(string name)
	{
		GameObject graphic = GetGraphic(name);

		if (graphic.activeSelf)
			graphic.SetActive(false);
	}

	public void ToggleGraphic(string name)
	{
		GameObject graphic = GetGraphic(name);
		graphic.SetActive(!graphic.activeSelf);
	}

	private void StateChangedHandler(object state)
	{
		GameState newState = (GameState)state;

		switch (newState)
		{
		case GameState.Editing:
			EnableGraphic("PlayButton");
			EnableGraphic("ExitButton");
			break;

		case GameState.Playing:
			DisableGraphic("PlayButton");
			DisableGraphic("ExitButton");
			DisableGraphic("TilePanel");
			break;
		}
	}
}
