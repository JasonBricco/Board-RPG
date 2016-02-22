using UnityEngine;
using System.Text;
using System.IO;

public sealed class ErrorHandler : MonoBehaviour, IUpdatable
{
	private static string dataPath;
	private bool quit = false;

	private void Awake()
	{
		Engine.StartUpdating(this);

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

	public static void LogText(params string[] items)
	{
		StringBuilder text = new StringBuilder(System.DateTime.Now.ToString() + System.Environment.NewLine);

		for (int i = 0; i < items.Length; i++)
			text.AppendLine(items[i]);

		File.AppendAllText(dataPath + "/Log.txt", text.ToString() + System.Environment.NewLine);
	}

	public void UpdateFrame()
	{
		if (quit) Application.Quit();
	}
}
