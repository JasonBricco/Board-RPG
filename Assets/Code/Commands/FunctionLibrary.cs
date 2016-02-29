using UnityEngine;
using System.Collections.Generic;

public sealed class FunctionLibrary : MonoBehaviour
{
	private Dictionary<string, Function> functions = new Dictionary<string, Function>();

	private void Awake()
	{
		functions.Add("Teleport", new TeleportFunction(this));
		functions.Add("SetTile", new SetTileFunction(this));
		functions.Add("SetMoves", new SetMovesFunction(this));
		functions.Add("Random", new RandomFunction(this));
		functions.Add("RandomTeleport", new RandomTeleportFunction(this));
		functions.Add("Timer", new TimerFunction(this));
		functions.Add("SetLine", new SetLineFunction(this));
		functions.Add("SetSquare", new SetSquareFunction(this));
		functions.Add("Flip", new FlipFunction(this));
		functions.Add("Flipped", new FlippedFunction(this));
		functions.Add("If", new IfFunction(this));
		functions.Add("TeleportInArea", new TeleportInAreaFunction(this));
		functions.Add("Repeat", new RepeatFunction(this));
		functions.Add("Swap", new SwapFunction(this));
		functions.Add("Skip", new SkipFunction(this));
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
