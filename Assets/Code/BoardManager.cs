using UnityEngine;
using System.Collections.Generic;

public sealed class BoardManager : MonoBehaviour
{
	public const int Size = 256;
	public const int WidthInChunks = Size / Chunk.Size;

	[SerializeField] private Material[] materials;
	private GameObject mainButtons;

	private Chunk[,] chunks = new Chunk[WidthInChunks, WidthInChunks];

	private List<Chunk> chunksToRebuild = new List<Chunk>();

	private void Awake()
	{
		Serializer.ListenForSave(SaveTiles);
		Serializer.ListenForLoad(LoadTiles);
	}

	private void Start()
	{
		mainButtons = UIStore.GetGraphic("MainButtons");
		ShowMainButtons();

		EventManager.StartListening("ClearPressed", ClearBoard);
		EventManager.StartListening("StateChanged", StateChangedHandler);
	}

	private void StateChangedHandler(int state)
	{
		switch ((GameState)state)
		{
		case GameState.Editing:
			ShowMainButtons();
			break;

		default:
			mainButtons.SetActive(false);
			break;
		}
	}

	private void ShowMainButtons()
	{
		if (!mainButtons.activeSelf)
			mainButtons.SetActive(true);
	}

	public Material GetMaterial(int index)
	{
		return materials[index];
	}

	public List<Vector2i> GetStartPositions()
	{
		List<Vector2i> positions = new List<Vector2i>();

		for (int x = 0; x < chunks.GetLength(0); x++)
		{
			for (int y = 0; y < chunks.GetLength(0); y++)
			{
				Chunk chunk = chunks[x, y];

				if (chunk != null)
				{
					for (int cX = 0; cX < Chunk.Size; cX++)
					{
						for (int cY = 0; cY < Chunk.Size; cY++)
						{
							Tile tile = chunk.GetTile(1, cX, cY);

							if (tile.Equals(Tiles.Start))
								positions.Add(new Vector2i((x * Chunk.Size) + cX, (y * Chunk.Size) + cY));
						}
					}
				}
			}
		}

		return positions;
	}

	public Tile GetTileSafe(int layer, int tX, int tY)
	{
		if (!InTileBounds(tX, tY)) return new Tile(0);
		return GetTile(layer, tX, tY);
	}

	public Tile GetTile(int layer, int tX, int tY)
	{
		Chunk chunk = GetChunk(tX, tY);
		return chunk == null ? Tiles.Air : chunk.GetTile(layer, tX & Chunk.Size - 1, tY & Chunk.Size - 1);
	}

	public void SetTile(Vector2i tPos, Tile tile)
	{
		tile.Type.OnAdded(tPos);
		GetChunkSafe(tPos.x, tPos.y).SetTile(tPos.x & Chunk.Size - 1, tPos.y & Chunk.Size - 1, tile);
	}

	public void DeleteTile(Vector2i tPos)
	{
		int localX = tPos.x & Chunk.Size - 1, localY = tPos.y & Chunk.Size - 1;
		GetChunkSafe(tPos.x, tPos.y).DeleteTile(localX, localY, tPos.x, tPos.y);
	}

	private Chunk GetChunk(int tX, int tY)
	{
		return chunks[tX >> Chunk.SizeBits, tY >> Chunk.SizeBits];
	}

	private Chunk GetChunkSafe(int tX, int tY)
	{
		Vector2i pos = new Vector2i(tX >> Chunk.SizeBits, tY >> Chunk.SizeBits);
		Chunk chunk = chunks[pos.x, pos.y];

		if (chunk == null)
		{
			chunk = new Chunk(pos.x, pos.y);
			chunks[pos.x, pos.y] = chunk;
		}

		return chunk;
	}

	public Chunk GetChunkDirect(int chunkX, int chunkY)
	{
		return chunks[chunkX, chunkY];
	}

	public void FlagChunkForRebuild(Vector2i tPos)
	{
		Chunk chunk = GetChunk(tPos.x, tPos.y);

		if (!chunk.FlaggedForUpdate)
		{
			chunk.FlaggedForUpdate = true;
			chunksToRebuild.Add(chunk);
		}
	}

	public void RebuildChunks()
	{
		for (int i = 0; i < chunksToRebuild.Count; i++)
			chunksToRebuild[i].BuildMesh();

		chunksToRebuild.Clear();
	}

	public bool InTileBounds(int x, int y)
	{
		return x >= 0 && y >= 0 && x < Size && y < Size;
	}

	public bool InChunkBounds(int x, int y)
	{
		return x >= 0 && y >= 0 && x < WidthInChunks && y < WidthInChunks; 
	}

	private void ClearBoard(int data)
	{
		for (int x = 0; x < chunks.GetLength(0); x++)
		{
			for (int y = 0; y < chunks.GetLength(0); y++)
			{
				Chunk chunk = chunks[x, y];

				if (chunk != null)
					chunk.Destroy();

				chunks[x, y] = null;
			}
		}

		Engine.CommandProcessor.ClearAll();
	}

	private void SaveTiles(BoardData data)
	{
		for (int x = 0; x < chunks.GetLength(0); x++)
		{
			for (int y = 0; y < chunks.GetLength(1); y++)
			{
				Chunk chunk = chunks[x, y];

				if (chunk != null)
					chunk.Save(data);
			}
		}
	}

	private void LoadTiles(BoardData data)
	{
		for (int i = 0; i < data.savedChunks.Count; i++)
		{
			int pos = data.savedChunks[i];
			int cX = pos & (BoardManager.WidthInChunks - 1);
			int cY = (pos >> 4) & (BoardManager.WidthInChunks - 1);

			Chunk chunk = new Chunk(cX, cY);
			chunks[cX, cY] = chunk;

			chunk.Load(data.chunkData[i]);
		}
	}
}
