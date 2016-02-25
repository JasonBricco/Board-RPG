using UnityEngine;

public class TeleportInAreaFunction : Function 
{
	public TeleportInAreaFunction(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 6, "Usage: [TeleportInArea: entityID, startX, startY, endX, endY]")) return;

		int entityID;

		if (!TryGetEntityID(args[1], entity, out entityID)) return;

		int startX, startY, endX, endY;

		if (!GetInteger(args[2], out startX)) return;
		if (!GetInteger(args[3], out startY)) return;
		if (!GetInteger(args[4], out endX)) return;
		if (!GetInteger(args[5], out endY)) return;

		int totalX = Mathf.Abs(endX - startX);
		int totalY = Mathf.Abs(endY - startY);
		int checkAmount = (totalX * totalY) * 2;
			
		BoardManager manager = boardManager;
		Tile tile = TileStore.Air;

		if (!manager.InTileBounds(startX, startY) || (!manager.InTileBounds(endX, endY)))
		{
			ErrorHandler.LogText("Command Error: coordinates are out of bounds of the board (TeleportInArea).");
			return;
		}
	
		for (int i = 0; i < checkAmount; i++)
		{
			int randX = Random.Range(startX, endX + 1);
			int randY = Random.Range(startY, endY + 1);

			tile = manager.GetTile(0, randX, randY);

			if (tile.ID != 0)
			{
				randX *= Tile.Size;
				randY *= Tile.Size;

				if (TryGetEntity(entityID, entity)) 
					entity.SetTo(new Vector3(randX, randY));

				break;
			}
		}

		if (tile.ID == 0)
			ErrorHandler.LogText("Command Error: failed to find a tile (TeleportInArea).");
	}
}
