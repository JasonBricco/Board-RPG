using UnityEngine;

public struct PossibleTile
{
	public Vector2i pos;
	public int remaining;

	public PossibleTile(Vector2i pos, int remaining)
	{
		this.pos = pos;
		this.remaining = remaining;
	}
}