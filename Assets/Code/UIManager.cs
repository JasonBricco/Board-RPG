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
	}

	public GameObject GetGraphic(string name)
	{
		if (graphics.ContainsKey(name))
			return graphics[name];

		Debug.Log("Could not find a graphic with the name: " + name);
		return null;
	}

	public void EnableGraphic(string name)
	{
		if (graphics.ContainsKey(name))
			graphics[name].SetActive(true);
		else
			Debug.Log("Failed to enable graphic. " + name + " doesn't exist.");
	}

	public void DisableGraphic(string name)
	{
		if (graphics.ContainsKey(name))
			graphics[name].SetActive(false);
		else
			Debug.Log("Failed to disable graphic. " + name + " doesn't exist.");
	}

	public void ToggleGraphic(string name)
	{
		if (graphics.ContainsKey(name))
		{
			GameObject graphic = graphics[name];
			graphic.SetActive(!graphic.activeSelf);
		}
		else
			Debug.Log("Failed to toggle graphic. " + name + " doesn't exist.");
	}
}
