using UnityEngine;
using System.Collections.Generic;

public sealed class FunctionLibrary 
{
	private Dictionary<string, Function> functions = new Dictionary<string, Function>();

	public FunctionLibrary()
	{
		functions.Add("Teleport", new TeleportFunction());
		functions.Add("SetTile", new SetTileFunction());
		functions.Add("SetMoves", new SetMovesFunction());
		functions.Add("Random", new RandomFunction());
		functions.Add("RandomTeleport", new RandomTeleportFunction());
		functions.Add("Timer", new TimerFunction());
		functions.Add("SetLine", new SetLineFunction());
		functions.Add("SetSquare", new SetSquareFunction());
		functions.Add("If", new IfFunction());
		functions.Add("TeleportInArea", new TeleportInAreaFunction());
		functions.Add("Repeat", new RepeatFunction());
		functions.Add("Swap", new SwapFunction());
		functions.Add("Skip", new SkipFunction());
		functions.Add("SetData", new SetDataFunction());
		functions.Add("Direction", new DirectionFunction());
		functions.Add("Respawn", new RespawnFunction());
	}

	public Function GetFunction(string name)
	{
		return functions[name];
	}

	public bool TryGetFunction(string name, out Function function)
	{
		return functions.TryGetValue(name, out function);
	}
}
