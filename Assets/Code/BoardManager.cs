using UnityEngine;
using System.Collections.Generic;
using System.IO;

public sealed class BoardManager : MonoBehaviour
{
	public const int Size = 512;
	public const int WidthInChunks = Size / Chunk.Size;

	[SerializeField] private Material[] materials;
	private GameObject mainButtons;

	private Chunk[,] chunks = new Chunk[WidthInChunks, WidthInChunks];

	private List<Chunk> chunksToRebuild = new List<Chunk>();

	private BoardData boardData = new BoardData();

	private void Start()
	{
		mainButtons = UIStore.GetGraphic("MainButtons");

 		EventManager.StartListening("Quit", SaveBoard);
		EventManager.StartListening("ClearPressed", ClearBoard);
		EventManager.StartListening("StateChanged", StateChangedHandler);

		LoadBoard();
	}

	private void StateChangedHandler(int state)
	{
		switch ((GameState)state)
		{
		case GameState.Editing:
			mainButtons.SetActive(true);
			break;

		default:
			mainButtons.SetActive(false);
			break;
		}
	}

	public Material GetMaterial(int index)
	{
		return materials[index];
	}

	public BoardData GetData()
	{
		return boardData;
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
		tile.Type.OnAdded(boardData, tPos);
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
			chunk = new Chunk(pos.x, pos.y, this);
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

		boardData.startTiles.Clear();
		boardData.triggerData.Clear();
	}

	private void SaveBoard(int data)
	{
		for (int x = 0; x < chunks.GetLength(0); x++)
		{
			for (int y = 0; y < chunks.GetLength(1); y++)
			{
				Chunk chunk = chunks[x, y];

				if (chunk != null)
					chunk.Save(boardData);
			}
		}

		FileStream stream = new FileStream(Application.persistentDataPath + "/Data.txt", FileMode.Create);
		StreamWriter dataWriter = new StreamWriter(stream);

		string json = JsonUtility.ToJson(boardData);
		dataWriter.Write(json);
		dataWriter.Close();
	}

	private void LoadBoard()
	{
		string path = Application.persistentDataPath + "/Data.txt";

		if (File.Exists(path))
		{
			StreamReader reader = new StreamReader(path);
			string json = reader.ReadToEnd();
			boardData = JsonUtility.FromJson<BoardData>(json);
			reader.Close();

			for (int i = 0; i < boardData.savedChunks.Count; i++)
			{
				int pos = boardData.savedChunks[i];
				int cX = pos & (BoardManager.WidthInChunks - 1);
				int cY = (pos >> TileType.SizeBits) & (BoardManager.WidthInChunks - 1);

				Chunk chunk = new Chunk(cX, cY, this);
				chunks[cX, cY] = chunk;

				chunk.Load(boardData.chunkData[i]);
			}

			boardData.ClearChunkData();
		}
	}
}
