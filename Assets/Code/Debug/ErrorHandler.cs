//
//  ErrorHandler.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/7/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Text;
using System.IO;

public sealed class ErrorHandler : IUpdatable
{
	private string dataPath;
	private bool quit = false;

	public ErrorHandler()
	{
		dataPath = Application.persistentDataPath;
		Application.logMessageReceived += HandleError;
	}

	private void HandleError(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Assert || type == LogType.Error || type == LogType.Exception)
		{
			LogText(logString, stackTrace);
			quit = true;
		}
	}

	private void LogText(params string[] items)
	{
		StringBuilder text = new StringBuilder(System.DateTime.Now.ToString() + System.Environment.NewLine);

		for (int i = 0; i < items.Length; i++)
			text.AppendLine(items[i]);

		File.AppendAllText(dataPath + "/Log.txt", text.ToString() + System.Environment.NewLine);
	}

	public void UpdateTick()
	{
		if (quit) Application.Quit();
	}
}
