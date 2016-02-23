using UnityEngine;
using System.Collections.Generic;

public sealed class FunctionLibrary : MonoBehaviour
{
	private Dictionary<string, Function> functions = new Dictionary<string, Function>();

	private void Awake()
	{
		functions.Add("MoveEntity", new MoveEntityFunction(this));
		functions.Add("SetTile", new SetTileFunction(this));
		functions.Add("SetMoves", new SetMovesFunction(this));
		functions.Add("Random", new RandomFunction(this));
		functions.Add("RandomTeleport", new RandomTeleportFunction(this));
		functions.Add("Timer", new TimerFunction(this));
		functions.Add("SetLine", new SetLineFunction(this));
		functions.Add("SetSquare", new SetSquareFunction(this));
		functions.Add("Flip", new FlipFunction(this));
		functions.Add("Flipped", new FlippedFunction(this));
	}

	public bool TryGetFunction(string name, out Function function)
	{
		return functions.TryGetValue(name, out function);
	}
}
