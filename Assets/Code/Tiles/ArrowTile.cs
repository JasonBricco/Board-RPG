using UnityEngine;

public class ArrowTile : TileType 
{
	public ArrowTile(ushort ID, int mesh, BoardManager manager) : base(manager)
	{
		name = "Arrow";
		tileID = ID;
		meshIndex = 7;
		layer = 1;
	}

	public override void OnFunction(Vector2i pos)
	{
		Tile tile = boardManager.GetTile(1, pos.x, pos.y);
		boardManager.SetTileFast(pos, new Tile(tile.ID, (ushort)((tile.Data + 1) & 3)));

		boardManager.FlagChunkForRebuild(pos);
		boardManager.RebuildChunks();
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		ushort data = boardManager.GetTile(1, tX, tY).Data;
		Vector2i dir = Vector2i.zero;

		switch (data)
		{
		case 1:
			dir = Vector2i.right;
			break;

		case 2:
			dir = Vector2i.down;
			break;

		case 3: 
			dir = Vector2i.left;
			break;

		default:
			dir = Vector2i.up;
			break;
		}

		Vector2i current = new Vector2i(tX, tY);
		Vector2i end = Vector2i.zero;

		int distance = 0;

		while (true)
		{
			Vector2i next = current + dir;
			Tile nextTile = boardManager.GetTileSafe(0, next.x, next.y);
			Tile nextOverlay = boardManager.GetTileSafe(1, next.x, next.y);

			if (nextTile.Equals(Tiles.Air))
			{
				entity.RemainingMoves = 0;
				end = current;

				if (distance == 0)
					return;

				break;
			}
			else if (nextTile.Equals(Tiles.Stopper) || nextOverlay.Equals(Tiles.Arrow))
			{
				entity.RemainingMoves = 0;
				end = next;
				break;
			}
			else
			{
				current = next;
				distance++;
			}
		}
			

		entity.Wait = true;
		entity.StartCoroutine(entity.SlideTo(end, dir));
	}

	public override void SetUVs(Tile tile, MeshData data)
	{
		switch (tile.Data)
		{
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

		default:
			base.SetUVs(tile, data);
			break;
		}
	}
}
