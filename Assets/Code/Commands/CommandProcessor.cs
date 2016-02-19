//
//  CommandProcessor.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/18/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;

public sealed class CommandProcessor : MonoBehaviour
{
	[SerializeField] private FunctionLibrary library;
	[SerializeField] private BoardEditor boardEditor;
	[SerializeField] private BoardManager boardManager;

	private Char[] delimiters = new char[] { '(', ',', ')' };

	private InputField commandField;
	private bool editorOpen = false;

	private void Awake()
	{
		EventManager.StartListening("LoadCode", LoadCode);
		EventManager.StartListening("SaveCode", SaveCode);
		EventManager.StartListening("CancelCode", CancelCode);
		EventManager.StartListening("Quit", QuitHandler);

		commandField = UIManager.GetGraphic<InputField>("CodeEditor");
	}

	private void LoadCode(int data)
	{
		UIManager.EnableWindow("CodeEditor");
		editorOpen = true;
	
		var triggerDict = boardManager.GetData().triggerData;

		TriggerData triggerData;

		if (triggerDict.TryGetValue(boardEditor.LastFunctionPos(), out triggerData))
			commandField.text = triggerData.triggerCode;
	}

	private void SaveCode(int data)
	{
		Process(commandField.text);
	}

	private void CancelCode(int data)
	{
		CreateTriggerData(boardEditor.LastFunctionPos(), null);
		commandField.text = String.Empty;
		UIManager.DisableWindow("CodeEditor", GameState.Editing);
		editorOpen = false;
	}

	private void QuitHandler(int data)
	{
		if (editorOpen) CreateTriggerData(boardEditor.LastFunctionPos(), null);
	}

	private void Process(string commandText)
	{
		UIManager.DisableGraphic("CommandError");

		List<string[]> functions = new List<string[]>();

		int commandCount = 0;

		for (int i = 0; i < commandText.Length; i++)
		{
			if (commandText[i] == ')')
				commandCount++;
		}

		if (commandCount == 0)
		{
			DisplayError("No commands found!");
			return;
		}

		StringBuilder[] commands = new StringBuilder[commandCount];
		int count = 0;

		for (int i = 0; i < commandText.Length; i++)
		{
			Char nextChar = commandText[i];

			if (!Char.IsWhiteSpace(nextChar))
				commands[count].Append(commandText[i]);

			if (nextChar == ')')
				count++;
		}

		for (int i = 0; i < commands.Length; i++)
		{
			string[] args = commands[i].ToString().Split(delimiters);

			Function function;
			bool success = library.TryGetFunction(args[0], out function);

			if (success)
			{
				CommandError error = function.ValidateArguments(args);

				switch (error)
				{
				case CommandError.InvalidArgCount:
					DisplayError("Invalid number of arguments found.");
					return;

				case CommandError.InvalidArgType:
					DisplayError("Invalid argument types supplied.");
					return;

				case CommandError.InvalidArgValue:
					DisplayError("Invalid value for an argument found.");
					return;

				default:
					functions.Add(args);
					break;
				}
			}
			else
			{
				DisplayError("Function doesn't exist.");
				return;
			}
		}

		CreateTriggerData(boardEditor.LastFunctionPos(), functions);
		UIManager.DisableWindow("CodeEditor", GameState.Editing);
	}

	private void CreateTriggerData(Vector2i pos, List<string[]> functions)
	{
		BoardData boardData = boardManager.GetData();

		TriggerData triggerData = new TriggerData();
		triggerData.triggerCode = String.Copy(commandField.text);
		triggerData.functions = functions;

		boardData.triggerData[pos] = triggerData;
	}

	private void DisplayError(string message)
	{
		GameObject error = UIManager.GetGraphic("CommandError");
		error.SetActive(true);
		error.GetComponent<Text>().text = message;
	}
}
