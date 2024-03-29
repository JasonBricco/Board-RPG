﻿using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public sealed class FPS : MonoBehaviour 
{
	private float updateInterval = 0.5f; 
	private Text myLabel;

	private float accum = 0; 
	private int frames = 0;
	private float timeleft = 0.5f;

	private void Start()
	{
		myLabel = SceneItems.GetItem<Text>("DebugFPS");
	}

	private void Update()
	{
		if (StateManager.CurrentState != GameState.Window && Input.GetKeyDown(KeyCode.Tab))
			myLabel.enabled = !myLabel.enabled;

		if (myLabel.enabled)
		{
			timeleft -= Time.deltaTime;
			accum += Time.timeScale / Time.deltaTime;
			++frames;

			if (timeleft <= 0.0f)
			{
				float fps = accum / frames;
				string format = System.String.Format("FPS " + fps.ToString("F0"));
				myLabel.text = format;

				if (fps < 30)
					myLabel.color = Color.yellow;
				else if (fps < 10)
					myLabel.color = Color.red;
				else
					myLabel.color = Color.green;

				timeleft = updateInterval;
				accum = 0.0f;
				frames = 0;
			}
		}
	}
}
