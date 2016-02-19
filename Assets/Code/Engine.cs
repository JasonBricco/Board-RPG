//
//  Engine.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections.Generic;

public delegate void Method();

public sealed class Engine : MonoBehaviour
{
	[SerializeField] private CameraControl camControl;

	private void Awake()
	{
		StateManager.ChangeState(GameState.Editing);
		EventManager.StartListening("ExitPressed", ExitPressedHandler);
	}

	private void ExitPressedHandler(int data)
	{
		Application.Quit();
	}

	private void OnApplicationQuit()
	{
		EventManager.TriggerEvent("Quit");
	}
}
