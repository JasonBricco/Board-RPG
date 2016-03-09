using UnityEngine;
using System.Collections.Generic;

public class RandomArrowTile : OverlayTile 
{
	public RandomArrowTile(ushort ID)
	{
		name = "Random Arrow";
		tileID = ID;

		material = Resources.Load<Material>("TileMaterials/RandomArrow");
		meshIndex = material.GetInt("_ID");
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		Vector2i start = new Vector2i(tX, tY);

		List<Vector2i> dirs = new List<Vector2i>(4);

		dirs.Add(Vector2i.left);
		dirs.Add(Vector2i.right);
		dirs.Add(Vector2i.up);
		dirs.Add(Vector2i.down);

		Utils.ShuffleList<Vector2i>(dirs);

		Vector2i dir = Vector2i.zero;

		for (int i = dirs.Count - 1; i >= 0; i--)
		{
			Vector2i next = start + dirs[i];

			if (!Map.GetTileTypeSafe(1, next.x, next.y).IsPassable(next.x, next.y))
				dirs.RemoveAt(i);
			else
			{
				dir = dirs[i];
				break;
			}
		}

		if (dir.Equals(Vector2i.zero)) return;

		Vector2i end = Utils.GetLineEnd(start, dir);

		entity.remainingMP = 0;

		if (end.Equals(start)) return;

		entity.wait = true;
		entity.StartCoroutine(entity.SlideTo(end, dir));
	}
}
