//
//  TileWindow.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/20/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public sealed class TileWindow : MonoBehaviour, IUpdatable
{
	private void Awake()
	{
		Engine.StartUpdating(this);
		gameObject.SetActive(false);
	}

	public void OnEnable()
	{
		StateManager.ChangeState(GameState.Window);
	}

	public void OnDisable()
	{
		StateManager.ChangeState(GameState.Editing);
	}

	public void UpdateFrame()
	{
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			if (StateManager.CurrentState == GameState.Window)
			{
				if (gameObject.activeSelf)
					gameObject.SetActive(false);
			}
			else
				gameObject.SetActive(true);
		}
	}
}
