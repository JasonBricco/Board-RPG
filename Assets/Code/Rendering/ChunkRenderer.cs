//
//  ChunkRenderer.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public sealed class ChunkRenderer : IUpdatable
{
	private BoardManager boardManager;

	public ChunkRenderer(BoardManager boardManager)
	{
		this.boardManager = boardManager;
	}

	public void UpdateTick()
	{
		Vector3 middle = new Vector3(Screen.width >> 1, Screen.height >> 1);
		Vector3 point = Camera.main.ScreenToWorldPoint(middle);

		int cX = (int)point.x >> Chunk.SizeBits, cY = (int)point.y >> Chunk.SizeBits;

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
