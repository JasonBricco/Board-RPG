using UnityEngine;

public class ArrowTile : TileType 
{
	public ArrowTile(ushort ID, int mesh)
	{
		name = "Arrow";
		tileID = ID;
		meshIndex = 7;
		layer = 1;
	}

	public override void OnFunction(Vector2i pos)
	{
		BoardManager manager = Engine.BoardManager;

		Tile tile = manager.GetTile(1, pos.x, pos.y);
		manager.SetTile(pos, new Tile(tile.ID, (ushort)((tile.Data + 1) & 3)));

		manager.FlagChunkForRebuild(pos);
		manager.RebuildChunks();
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		BoardManager manager = Engine.BoardManager;

		ushort data = manager.GetTile(1, tX, tY).Data;
		Vector2i dir = Vector2i.zero;

		switch (data)
		{
		case 0:
			dir = Vector2i.up;
			break;

		case 1:
			dir = Vector2i.right;
			break;

		case 2:
			dir = Vector2i.down;
			break;

		case 3: 
			dir = Vector2i.left;
			break;
		}

		Vector2i current = new Vector2i(tX, tY);
		Vector2i end = Vector2i.zero;

		while (true)
		{
			Vector2i next = current + dir;
			Tile nextTile = manager.GetTileSafe(0, next.x, next.y);

			if (nextTile.Equals(Tiles.Air))
			{
				entity.RemainingMoves = 0;
				end = current;
				break;
			}
			else if (nextTile.Equals(Tiles.Stopper))
			{
				entity.RemainingMoves = 0;
				end = next;
				break;
			}
			else
				current = next;
		}
			

		entity.Wait = true;
		entity.StartCoroutine(entity.SlideTo(end, dir));
	}

	public override void SetUVs(Tile tile, MeshData data)
	{
		switch (tile.Data)
		{
		case 0:
			base.SetUVs(tile, data);
			break;

		case 1:
			data.AddUV(meshIndex, new Vector2(1.0f, 1.0f));
			data.AddUV(meshIndex, new Vector2(0.0f, 1.0f));
			data.AddUV(meshIndex, new Vector2(0.0f, 0.0f));
			data.AddUV(meshIndex, new Vector2(1.0f, 0.0f));
			break;

		case 2:
			data.AddUV(meshIndex, new Vector2(0.0f, 1.0f));
			data.AddUV(meshIndex, new Vector2(0.0f, 0.0f));
			data.AddUV(meshIndex, new Vector2(1.0f, 0.0f));
			data.AddUV(meshIndex, new Vector2(1.0f, 1.0f));
			break;

		case 3:
			data.AddUV(meshIndex, new Vector2(0.0f, 0.0f));
			data.AddUV(meshIndex, new Vector2(1.0f, 0.0f));
			data.AddUV(meshIndex, new Vector2(1.0f, 1.0f));
			data.AddUV(meshIndex, new Vector2(0.0f, 1.0f));
			break;
		}
	}
}
