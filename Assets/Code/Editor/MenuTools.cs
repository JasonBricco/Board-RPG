using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public static class MenuTools 
{
	[MenuItem("Tools/General/Open Save Folder")]
	private static void OpenSaveFolder()
	{
		EditorUtility.RevealInFinder(Application.persistentDataPath);
	}

	[MenuItem("Tools/General/Clear Console %#m")]
	private static void ClearConsole()
	{
		var logEntries = Type.GetType("UnityEditorInternal.LogEntries, UnityEditor.dll");
		var clearMethod = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
		clearMethod.Invoke(null, null);
	}

	[MenuItem("Tools/General/Clear PlayerPrefs")]
	private static void ClearPlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
	}
}
