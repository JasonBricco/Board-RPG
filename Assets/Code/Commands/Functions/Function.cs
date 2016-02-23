using UnityEngine;
using System;

public class Function
{
	protected FunctionLibrary library;
	private static GameObject engine;

	protected Char[] bracketSeparators = new char[] { '[', '/', ']' };

	protected PlayerManager playerManager
	{
		get { return Engine.Instance.GetComponent<PlayerManager>(); }
	}

	protected BoardManager boardManager
	{
		get { return Engine.Instance.GetComponent<BoardManager>(); }
	}

	protected BoardEditor boardEditor
	{
		get { return Engine.Instance.GetComponent<BoardEditor>(); }
	}

	public Function(FunctionLibrary library)
	{
		this.library = library;
	}

	public virtual void Compute(string[] args, Entity entity)
	{
	}

	public virtual bool GetValue(string[] args, Entity entity, out Value value)
	{
		value = new Value();
		return true;
	}

	private bool TryGetFunctionValue(string functionString, Entity entity, out Value value)
	{
		value = new Value();

		string[] args = functionString.Split(bracketSeparators, StringSplitOptions.RemoveEmptyEntries);

		Function function;

		if (library.TryGetFunction(args[0], out function))
		{
			if (function.GetValue(args, entity, out value))
				return true;
		}

		return false;
	}

	protected bool GetInteger(string arg, Entity entity, out int num)
	{
		if (int.TryParse(arg, out num))
			return true;

		Value value;

		if (TryGetFunctionValue(arg, entity, out value))
		{
			num = value.Int;
			return true;
		}

		num = 0;
		return false;
	}

	protected bool GetBool(string arg, Entity entity, out bool b)
	{
		if (arg == "true")
		{
			b = true;
			return true;
		}
		else if (arg == "false")
		{
			b = false;
			return true;
		}

		Value value;

		if (TryGetFunctionValue(arg, entity, out value))
		{
			b = value.Bool;
			return true;
		}

		b = false;
		return false;
	}

	protected bool CheckArgCount(string[] args, int required, string usage)
	{
		if (args.Length != required)
		{
			ErrorHandler.LogText("Command Error: invalid argument count.", usage);
			return false;
		}

		return true;
	}

	protected bool TryGetEntityID(string arg, Entity entity, out int entityID)
	{
		if (arg == "@")
			entityID = entity.EntityID;
		else
		{
			if (!GetInteger(arg, entity, out entityID))
			{
				ErrorHandler.LogText("Command Error: entity ID must be an integer or \"@\".");
				return false;
			}
		}

		return true;
	}

	protected bool TryGetEntity(int entityID, Entity entity)
	{
		entity = playerManager.GetEntity(entityID);

		if (entity == null)
		{
			ErrorHandler.LogText("Command Error: could not find the entity with ID " + entityID + ".");
			return false;
		}

		return true;
	}
}
