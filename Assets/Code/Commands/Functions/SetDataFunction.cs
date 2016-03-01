using UnityEngine;

public class SetDataFunction : Function 
{
	public SetDataFunction(FunctionLibrary library) : base(library) {}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 5, "Usage: [SetData: x, y, data]")) return;

		int layer, x, y, data;

		if (!GetInteger(args[1], out layer)) return;
		if (!GetInteger(args[2], out x)) return;
		if (!GetInteger(args[3], out y)) return;
		if (!GetInteger(args[4], out data)) return;

		if (layer != 0 && layer != 1)
		{
			ErrorHandler.LogText("Command Error: layer must be 0 for main tiles or 1 for overlay tiles.");
			return;
		}

		if (data < 0 || data > ushort.MaxValue)
		{
			ErrorHandler.LogText("Command Error: data must be between 0 and 65536.");
			return;
		}

		BoardManager manager = Engine.BoardManager;

		if (!manager.InTileBounds(x, y))
		{
			ErrorHandler.LogText("Command Error: tried to set data to a tile outside of the board.");
			return;
		}

		Tile oldTile = manager.GetTile(layer, x, y);
		Tile newTile = new Tile(oldTile.ID, (ushort)data);

		Vector2i pos = new Vector2i(x, y);
		Engine.BoardEditor.SetSingleTile(pos, newTile);
	}
}
