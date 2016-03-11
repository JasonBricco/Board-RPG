using UnityEngine;

public class ArrowTile : OverlayTile 
{
	private ushort orientation = 0;

	public ArrowTile(ushort ID)
	{
		name = "Arrow";
		tileID = ID;

		material = Resources.Load<Material>("TileMaterials/Arrow");
		meshIndex = material.GetInt("_ID");
	}

	public override void OnFunction(Vector2i pos)
	{
		Tile tile = Map.GetTile(1, pos.x, pos.y);

		orientation = (ushort)((tile.Data + 1) & 3);
		Map.SetTileFast(pos, new Tile(tileID, orientation));

		Map.RebuildChunks();
	}

	public override Tile Preprocess(Tile tile, Vector2i pos)
	{
		tile.Data = orientation;
		return tile;
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		ushort data = Map.GetTile(1, tX, tY).Data;
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

		Vector2i start = new Vector2i(tX, tY);
		Vector2i end = Utils.GetLineEnd(start, dir);

		if (end.Equals(start))
			return;

		entity.wait = true;
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
