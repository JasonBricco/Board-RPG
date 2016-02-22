//
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

	private Char[] delimiters = new char[] { '(', ',', ')' };

	private GameObject codeEditor;
	private InputField commandField;
	private bool editorOpen = false;

	private void Awake()
	{
		EventManager.StartListening("SaveCode", SaveCode);
		EventManager.StartListening("Quit", QuitHandler);

		codeEditor = UIStore.GetGraphic("CodeEditor");
		commandField = codeEditor.GetComponent<InputField>();
	}

	private string LoadCommands(Vector2i pos)
	{
		var triggerDict = boardManager.GetData().triggerData;

		string triggerData;

		if (triggerDict.TryGetValue(pos, out triggerData))
			return triggerData;

		return String.Empty;
	}

	public void LoadEditor()
	{
		codeEditor.SetActive(true);
		editorOpen = true;
	
		commandField.text = LoadCommands(boardEditor.LastFunctionPos());

		commandField.ActivateInputField();
		commandField.Select();
		StartCoroutine(MoveTextToEnd());
	}

	private IEnumerator MoveTextToEnd()
	{
		yield return new WaitForEndOfFrame();
		commandField.MoveTextEnd(false);
	}

	private void SaveCode(int data)
	{
		CreateTriggerData(boardEditor.LastFunctionPos(), commandField.text);
		commandField.text = String.Empty;
		codeEditor.SetActive(false);
		editorOpen = false;
	}

	private void QuitHandler(int data)
	{
		if (editorOpen) SaveCode(0);
	}

	public void Process(int tX, int tY, Entity entity)
	{
		string commands = LoadCommands(new Vector2i(tX, tY));

		int commandCount = 0;

		for (int i = 0; i < commands.Length; i++)
		{
			if (commands[i] == ')')
				commandCount++;
		}

		if (commandCount == 0) 
		{
			ErrorHandler.LogText("Command Error: no commands found!");
			return;
		}

		StringBuilder[] commandList = new StringBuilder[commandCount];

		for (int i = 0; i < commandList.Length; i++)
			commandList[i] = new StringBuilder();
		
		int count = 0;
		bool foundBracket = false;

		for (int i = 0; i < commands.Length; i++)
		{
			Char nextChar = commands[i];

			if (!Char.IsWhiteSpace(nextChar))
			{
				if (nextChar == '[') foundBracket = true;
				if (nextChar == ']') foundBracket = false;

				if (foundBracket && nextChar == '[')
				{
					ErrorHandler.LogText("Command Error: found an opening bracket before closing the previous.");
					return;
				}
				
				if (foundBracket && nextChar == ',')
					commandList[count].Append('/');
				else
					commandList[count].Append(commands[i]);
			}

			if (nextChar == ')') count++;
		}

		for (int i = 0; i < commandList.Length; i++)
		{
			string[] args = commandList[i].ToString().Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

			Function function;
			bool success = library.TryGetFunction(args[0], out function);

			if (!success) 
			{
				ErrorHandler.LogText("Command Error: function doesn't exist: " + args[0] + ".");
				continue;
			}

			function.Compute(args, entity);
		}
	}

	private void CreateTriggerData(Vector2i pos, string commands)
	{
		var triggerDict = boardManager.GetData().triggerData;
		triggerDict[pos] = String.Copy(commandField.text);
	}
}
