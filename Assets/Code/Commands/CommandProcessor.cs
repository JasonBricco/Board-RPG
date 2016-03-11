using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public sealed class CommandProcessor : MonoBehaviour
{
	private Dictionary<Vector2i, string> triggerData = new Dictionary<Vector2i, string>();

	private Char[] delimiters = new char[] { '[', ':', ',', ']' };

	private GameObject codeEditor;
	private CodeEditor editorField;
	private bool editorOpen = false;

	private Vector2i currentPos;

	private int caretPos;
	private string selectedText;

	private void Awake()
	{
		Serializer.ListenForSave(SaveCommands);
		Serializer.ListenForLoad(LoadCommands);

		EventManager.StartListening("CodeFinished", SaveCode);
		EventManager.StartListening("MapCleared", ClearCommands);
		EventManager.StartListening("GetCommandCoords", GetCommandCoords);

		codeEditor = UIStore.GetGraphic("CodeEditor");
		editorField = codeEditor.GetComponent<CodeEditor>();
	}

	private void SaveCommands(MapData data)
	{
		if (editorOpen) CreateTriggerData(currentPos, editorField.text);

		foreach (KeyValuePair<Vector2i, string> pair in triggerData)
		{
			data.triggerKeys.Add(pair.Key);
			data.triggerValues.Add(pair.Value);
		}
	}

	private void LoadCommands(MapData data)
	{
		triggerData.Clear();

		for (int i = 0; i < data.triggerKeys.Count; i++)
			triggerData.Add(data.triggerKeys[i], data.triggerValues[i]);
	}

	private void GetCommandCoords(Data data)
	{
		// TODO: Support when we figure out how caret position works.
		//caretPos = editorField.caretPosition; 
		SaveCode(null);
		StateManager.ChangeState(GameState.SelectingCoords);

		StartCoroutine(WaitForCoords());
	}

	private IEnumerator WaitForCoords()
	{
		while (!codeEditor.activeSelf)
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector2i tilePos = Utils.GetCursorTilePos();
				// TODO: Support when we figure out how caret position works.
				//string result = triggerData[currentPos].Insert(caretPos, tilePos.x + ", " + tilePos.y);
				string result = triggerData[currentPos] + tilePos.x + ", " + tilePos.y;
				triggerData[currentPos] = result;
				LoadEditor(currentPos);
			}

			yield return null;
		}
	}

	private void ClearCommands(Data data)
	{
		triggerData.Clear();
	}

	private string GetCommands(Vector2i pos)
	{
		string data;

		if (triggerData.TryGetValue(pos, out data))
			return data;

		return String.Empty;
	}
		
	private void SaveCode(Data data)
	{
		CreateTriggerData(currentPos, editorField.text);
		editorField.text = String.Empty;
		codeEditor.SetActive(false);
		editorOpen = false;
	}

	public void DeleteCommands(Vector2i pos)
	{
		triggerData.Remove(pos);
	}

	public void LoadEditor(Vector2i pos)
	{
		currentPos = pos;

		codeEditor.SetActive(true);
		editorOpen = true;
	
		string data = GetCommands(pos);
		editorField.Load(data);
	}

	public void Process(int tX, int tY, Entity entity)
	{
		string input = GetCommands(new Vector2i(tX, tY));

		if (input.Length == 0) return;

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

			while (true)
			{
				int startIndex = GetInnermostIndex(input);

				if (startIndex == -1)
					break;
				
				int endIndex = input.IndexOf(']', startIndex);

				string funcString = input.Substring(startIndex, (endIndex - startIndex) + 1);
				string[] args = funcString.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

				Function function = null;

				bool success = args.Length == 0 ? false : Function.Library.TryGetFunction(args[0], out function);

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

			Function mainFunction = Function.Library.GetFunction(finalArgs[0]);
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
		triggerData[pos] = String.Copy(editorField.text);
	}
}
	