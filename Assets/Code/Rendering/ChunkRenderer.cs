using UnityEngine;

public sealed class ChunkRenderer : MonoBehaviour, IUpdatable
{
	private const int WToXShift = Chunk.SizeBits + Tile.SizeBits;

	[SerializeField] private BoardManager boardManager;

	private void Awake()
	{
		Engine.StartUpdating(this);
	}

	public void UpdateFrame()
	{
		Vector3 middle = new Vector3(Screen.width >> 1, Screen.height >> 1);
		Vector3 point = Camera.main.ScreenToWorldPoint(middle);

		int cX = (int)point.x >> WToXShift, cY = (int)point.y >> WToXShift;

		for (int x = cX - 1; x <= cX + 1; x++)
		{
			for (int y = cY - 1; y <= cY + 1; y++)
			{
				if (boardManager.InChunkBounds(x, y))
				{
					Chunk chunkToRender = boardManager.GetChunkDirect(x, y);

					if (chunkToRender != null)
						chunkToRender.Render();
				}
			}
		}
	}
}
