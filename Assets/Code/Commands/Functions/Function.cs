using UnityEngine;
using System;

public enum FunctionType { Main, Value }

public sealed class WaitingFunction 
{
	public Function function;
	public string[] args;
	public int turnsRemaining;
	public Entity entity;

	public WaitingFunction(Function func, string[] args, int turnsRemaining, Entity entity)
	{
		this.function = func;
		this.args = args;
		this.turnsRemaining = turnsRemaining;
		this.entity = entity;
	}
}

public class Function
{
	protected Map boardManager;
	private static GameObject engine;

	protected FunctionType type = FunctionType.Main;
	public FunctionType Type { get { return type; } }

	protected Char[] bracketSeparators = new char[] { '[', '/', ']' };

	public Function(Map manager)
	{
		this.boardManager = manager;
	}

	public virtual void Compute(string[] args, Entity entity)
	{
	}

	public virtual string GetValue(string[] args, Entity entity)
	{
		return "null";
	}

	protected bool GetInteger(string arg, out int num)
	{
		if (int.TryParse(arg, out num))
			return true;

		ErrorHandler.LogText("Command Error: argument must be an integer.");
		return false;
	}

	protected bool GetBool(string arg, out bool b)
	{
		b = false; 

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

		ErrorHandler.LogText("Command Error: argument must be true or false.");
		return false;
	}

	protected bool CheckArgCount(string[] args, int required, string usage)
	{
		if (args.Length < required)
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
			if (!GetInteger(arg, out entityID))
			{
				ErrorHandler.LogText("Command Error: entity ID must be an integer or \"@\".");
				return false;
			}
		}

		return true;
	}

	protected bool TryGetEntity(int entityID, Entity entity)
	{
		entity = boardManager.GetEntity(entityID);

		if (entity == null)
		{
			ErrorHandler.LogText("Command Error: could not find the entity with ID " + entityID + ".");
			return false;
		}

		return true;
	}
}
