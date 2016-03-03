using UnityEngine;
using System.Collections.Generic;

public class RandomArrowTile : OverlayTile 
{
	public RandomArrowTile(ushort ID, int mesh, BoardManager manager) : base(manager)
	{
		name = "Random Arrow";
		tileID = ID;
		meshIndex = mesh;
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

			if (!boardManager.IsPassable(next.x, next.y))
				dirs.RemoveAt(i);
			else
			{
				dir = dirs[i];
				break;
			}
		}

		if (dir.Equals(Vector2i.zero)) return;

		Vector2i end = boardManager.GetLineEnd(start, dir);

		entity.RemainingMoves = 0;

		if (end.Equals(start)) return;

		entity.Wait = true;
		entity.StartCoroutine(entity.SlideTo(end, dir));
	}
}
