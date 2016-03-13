using UnityEngine;

public class TeleportInAreaFunction : Function 
{
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

		startX = Mathf.Clamp(startX, 0, 255);
		startY = Mathf.Clamp(startY, 0, 255);
		endX = Mathf.Clamp(endX, 0, 255);
		endY = Mathf.Clamp(endY, 0, 255);

		int totalX = Mathf.Abs(endX - startX);
		int totalY = Mathf.Abs(endY - startY);
		int checkAmount = (totalX * totalY) * 2;
			
		Tile tile = Tiles.Air;
	
		for (int i = 0; i < checkAmount; i++)
		{
			int randX = Random.Range(startX, endX + 1);
			int randY = Random.Range(startY, endY + 1);

			if (Map.GetTileType(1, randX, randY).IsPassable(randX, randY))
			{
				randX *= Tile.Size;
				randY *= Tile.Size;

				Data data = new Data(entityID);
				data.position = new Vector3(randX, randY);

				EventManager.Notify("Teleport", data);

				break;
			}
		}

		if (tile.ID == 0)
			ErrorHandler.LogText("Command Error: failed to find a tile (TeleportInArea).");
	}
}
