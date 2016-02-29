using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public sealed class CommandProcessor : MonoBehaviour
{
	[SerializeField] private FunctionLibrary library;
	[SerializeField] private BoardEditor boardEditor;
	[SerializeField] private BoardManager boardManager;

	private Char[] delimiters = new char[] { '[', ':', ',', ']' };

	private GameObject codeEditor;
	private CodeEditor editorField;
	private bool editorOpen = false;

	private Vector2i currentPos;

	private void Awake()
	{
		EventManager.StartListening("SaveCode", SaveCode);
		EventManager.StartListening("Quit", QuitHandler);

		codeEditor = UIStore.GetGraphic("CodeEditor");
		editorField = codeEditor.GetComponent<CodeEditor>();
	}

	private string LoadCommands(Vector2i pos)
	{
		var triggerDict = boardManager.GetData().triggerData;

		string data;

		if (triggerDict.TryGetValue(pos, out data))
			return data;

		return String.Empty;
	}

	public void LoadEditor(Vector2i pos)
	{
		currentPos = pos;

		codeEditor.SetActive(true);
		editorOpen = true;
	
		string data = LoadCommands(pos);
		editorField.Load(data);
	}

	private void SaveCode(int data)
	{
		CreateTriggerData(currentPos, editorField.text);
		editorField.text = String.Empty;
		codeEditor.SetActive(false);
		editorOpen = false;
	}

	private void QuitHandler(int data)
	{
		if (editorOpen) SaveCode(0);
	}

	public void Process(int tX, int tY, Entity entity)
	{
		string input = LoadCommands(new Vector2i(tX, tY));

		if (input.Length == 0) return;

		if (input[0] != '[') 
		{
			ErrorHandler.LogText("Command Error: invalid command format. Commands must start with \"[\".");
			return;
		}

		List<string> commands = new List<string>();
		StringBuilder current = new StringBuilder();

		int bracketCount = 0; 

		for (int i = 0; i < input.Length; i++)
		{
			Char nextChar = input[i];

			if (Char.IsWhiteSpace(nextChar)) 
				continue;

			if (nextChar == '[')
			{
				if (bracketCount == 0)
					current = new StringBuilder();
				
				bracketCount++;
			}
			else if (nextChar == ']')
			{
				bracketCount--;

				if (bracketCount == 0)
				{
					current.Append(']');
					commands.Add(current.ToString());
					continue;
				}
			}

			current.Append(nextChar);
		}

		for (int command = 0; command < commands.Count; command++)
		{
			input = commands[command];

			for (int i = 0; i < 1000; i++) // SAFEGUARD
			{
				int startIndex = GetInnermostIndex(input);

				if (startIndex == -1)
					break;
				
				int endIndex = input.IndexOf(']', startIndex);

				string funcString = input.Substring(startIndex, (endIndex - startIndex) + 1);
				string[] args = funcString.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

				Function function;
				bool success = args.Length == 0 ? false : library.TryGetFunction(args[0], out function);

				if (success)
				{
					if (function.Type == FunctionType.Value)
					{
						string value = function.GetValue(args, entity);
						input = input.Replace(funcString, value);
					}
					else
					{
						string noBrackets = funcString.Substring(1, funcString.Length - 2);
						input = input.Replace(funcString, noBrackets);
					}
				}
				else
				{
					ErrorHandler.LogText("Command Error: invalid command entered, skipping.");
					input = input.Remove(startIndex, (endIndex - startIndex) + 1);
				}
			}

			string[] finalArgs = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

			if (finalArgs.Length == 0)
			{
				ErrorHandler.LogText("Command Error: no commands found!");
				return;
			}

			Function mainFunction = library.GetFunction(finalArgs[0]);
			mainFunction.Compute(finalArgs, entity);
		}
	}

	private int GetInnermostIndex(string input)
	{
		bool found = false;

		int deepestIndex = 0;
		int deepestCounter = 0;

		int counter = 0;

		for (int i = 0; i < input.Length; i++)
		{
			Char nextChar = input[i];

			if (nextChar == '[') 
			{
				found = true;
				counter++;

				if (counter > deepestCounter)
				{
					deepestIndex = i;
					deepestCounter = counter;
				}
			}

			if (nextChar == ']') 
				counter--;
		}

		return found ? deepestIndex : -1;
	}

	private void CreateTriggerData(Vector2i pos, string commands)
	{
		var triggerDict = boardManager.GetData().triggerData;
		triggerDict[pos] = String.Copy(editorField.text);
	}
}
	