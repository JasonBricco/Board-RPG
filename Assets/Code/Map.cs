using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public sealed class Map : MonoBehaviour, IUpdatable
{
	public const int MaxMeshes = 14;
	public const int Size = 256;
	public const int WidthInChunks = Size / Chunk.Size;

	private static Dictionary<string, TileType> tilesByName = new Dictionary<string, TileType>();
	private static List<TileType> tilesList = new List<TileType>();

	private static Chunk[,] chunks = new Chunk[WidthInChunks, WidthInChunks];
	private static List<Chunk> chunksToRebuild = new List<Chunk>();

	private MapRenderer mapRenderer = new MapRenderer();

	private void Awake()
	{
		Serializer.ListenForSave(SaveTiles);
		Serializer.ListenForLoad(LoadTiles);

		EventManager.StartListening("ClearMap", Clear);

		CreateTiles();

		Engine.StartUpdating(this);
	}

	public void UpdateFrame()
	{
		mapRenderer.RenderChunks();
	}

	public static void SetTile(Vector2i tPos, Tile tile)
	{
		SetTileAdvanced(tPos, tile);
		RebuildChunks();
	}

	public static void SetMultipleTiles(List<Vector2i> tPositions, Tile tile)
	{
		for (int i = 0; i < tPositions.Count; i++)
			SetTileAdvanced(tPositions[i], tile);

		RebuildChunks();
	}

	private static void SetTileAdvanced(Vector2i tPos, Tile tile)
	{
		bool deleting = tile.ID == 0;

		if (InTileBounds(tPos.x, tPos.y))
		{
			if (deleting) 
			{
				DeleteTile(tPos);
				FlagChunkForRebuild(tPos);
			}
			else 
			{
				TileType type = GetTileType(tile);
				GetTileType(type.Layer, tPos.x, tPos.y).OnDeleted(tPos);

				if (type.CanAdd(tPos))
				{
					SetTileFast(tPos, tile);
					FlagChunkForRebuild(tPos);
				}
			}
		}
	}

	public static Tile GetTileSafe(int layer, int tX, int tY)
	{
		if (!InTileBounds(tX, tY)) return Tiles.Air;
		return GetTile(layer, tX, tY);
	}

	public static Tile GetTile(int layer, int tX, int tY)
	{
		Chunk chunk = GetChunk(tX, tY);
		return chunk == null ? Tiles.Air : chunk.GetTile(layer, tX & Chunk.Size - 1, tY & Chunk.Size - 1);
	}

	public static void SetTileFast(Vector2i tPos, Tile tile)
	{
		tile = GetTileType(tile).Preprocess(tile, tPos);
		GetChunkSafe(tPos.x, tPos.y).SetTile(tPos.x & Chunk.Size - 1, tPos.y & Chunk.Size - 1, tile);
	}

	public static void DeleteTile(Vector2i tPos)
	{
		int localX = tPos.x & Chunk.Size - 1, localY = tPos.y & Chunk.Size - 1;
		GetChunkSafe(tPos.x, tPos.y).DeleteTile(localX, localY, tPos.x, tPos.y);
	}

	public static Chunk GetChunk(int tX, int tY)
	{
		return chunks[tX >> Chunk.SizeBits, tY >> Chunk.SizeBits];
	}

	public static Chunk GetChunkSafe(int tX, int tY)
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

	public static Chunk GetChunkDirect(int chunkX, int chunkY)
	{
		return chunks[chunkX, chunkY];
	}

	public static void FlagChunkForRebuild(Vector2i tPos)
	{
		Chunk chunk = GetChunk(tPos.x, tPos.y);

		if (!chunk.flaggedForUpdate)
		{
			chunk.flaggedForUpdate = true;
			chunksToRebuild.Add(chunk);
		}
	}

	public static void RebuildChunks()
	{
		for (int i = 0; i < chunksToRebuild.Count; i++)
			chunksToRebuild[i].BuildMesh();

		chunksToRebuild.Clear();
	}

	public static bool InTileBounds(int tX, int tY)
	{
		return tX >= 0 && tY >= 0 && tX < Size && tY < Size;
	}

	public static bool InChunkBounds(int cX, int cY)
	{
		return cX >= 0 && cY >= 0 && cX < WidthInChunks && cY < WidthInChunks; 
	}

	public static void Clear(Data data)
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

		EventManager.Notify("MapCleared", null);
	}

	public static TileType GetTileType(string name)
	{
		TileType tile;
		bool success = tilesByName.TryGetValue(name, out tile);
		return success ? tile : null;
	}

	public static TileType GetTileType(Tile tile)
	{
		return tilesList[tile.ID];
	}

	public static TileType GetTileType(int layer, int x, int y)
	{
		return tilesList[GetTile(layer, x, y).ID];
	}

	public static TileType GetTileTypeSafe(int layer, int x, int y)
	{
		return tilesList[GetTileSafe(layer, x, y).ID];
	}

	private void SaveTiles(MapData data)
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

	private void LoadTiles(MapData data)
	{
		for (int i = 0; i < data.savedChunks.Count; i++)
		{
			int pos = data.savedChunks[i];
			int cX = pos & (Map.WidthInChunks - 1);
			int cY = (pos >> 4) & (Map.WidthInChunks - 1);

			Chunk chunk = new Chunk(cX, cY);
			chunks[cX, cY] = chunk;

			chunk.Load(data.chunkData[i]);
		}
	}

	private void CreateTiles()
	{
		tilesList.Add(new AirTile(Tiles.Air.ID));
		tilesList.Add(new GrassTile(Tiles.Grass.ID));
		tilesList.Add(new StartTile(Tiles.Start.ID));
		tilesList.Add(new StoneTile(Tiles.Stone.ID));
		tilesList.Add(new SandTile(Tiles.Sand.ID));
		tilesList.Add(new TriggerTile(Tiles.Trigger.ID));
		tilesList.Add(new LandTriggerTile(Tiles.LandTrigger.ID));
		tilesList.Add(new CardTile(Tiles.Card.ID));
		tilesList.Add(new ArrowTile(Tiles.Arrow.ID));
		tilesList.Add(new StopperTile(Tiles.Stopper.ID));
		tilesList.Add(new RandomArrowTile(Tiles.RandomArrow.ID));
		tilesList.Add(new WaterTile(Tiles.Water.ID));
		tilesList.Add(new BorderTile(Tiles.Border.ID));

		for (int i = 0; i < tilesList.Count; i++)
			tilesByName.Add(tilesList[i].Name, tilesList[i]);
	}
}
