using UnityEngine;

public class TileType  
{
	public const int SizeBits = 5;
	public const int Size = 1 << SizeBits;
	public const int HalfSize = Size / 2;

	protected static MeshBuilder meshBuilder = new MeshBuilder();

	protected string name = "Unassigned";
	protected ushort tileID = 0;
	protected int meshIndex = 0;
	protected int layer = 0;

	public string Name { get { return name; } }
	public ushort ID { get { return tileID; } }
	public int MeshIndex { get { return meshIndex; } }
	public int Layer { get { return layer; } }

	public virtual void Build(Tile tile, int tX, int tY, MeshData data)
	{
		meshBuilder.BuildSquare(tile, tX, tY, data);
	}

	public virtual bool CanAdd(BoardData data, Vector2i pos)
	{
		return true;
	}

	public virtual void OnAdded(BoardData data, Vector2i pos)
	{
	}

	public virtual void OnDeleted(BoardData data, Vector2i pos)
	{
	}

	public virtual void OnFunction(Vector2i pos)
	{
	}

	public virtual void OnEnter(int tX, int tY, Entity entity)
	{
		entity.Wait = false;
	}

	public virtual void SetUVs(Tile tile, MeshData data)
	{
		data.AddUV(meshIndex, new Vector2(1.0f, 0.0f));
		data.AddUV(meshIndex, new Vector2(1.0f, 1.0f));
		data.AddUV(meshIndex, new Vector2(0.0f, 1.0f));
		data.AddUV(meshIndex, new Vector2(0.0f, 0.0f));
	}
}
