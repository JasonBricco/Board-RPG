using UnityEngine;

public class TileType  
{
	protected Material material;

	protected static MeshBuilder meshBuilder = new MeshBuilder();

	protected string name = "Unassigned";
	protected ushort tileID = 0;
	protected int meshIndex = 0;
	protected int layer = 0;

	public Texture2D Texture { get { return (Texture2D)material.mainTexture; } }
	public string Name { get { return name; } }
	public ushort ID { get { return tileID; } }
	public int MeshIndex { get { return meshIndex; } }
	public int Layer { get { return layer; } }

	public virtual void Build(Tile tile, int tX, int tY, MeshData data)
	{
		data.SetMaterial(material, meshIndex);
		meshBuilder.BuildSquare(this, tile, tX, tY, data);
	}

	public virtual bool IsPassable(int x, int y)
	{
		return true;
	}

	public virtual bool CanAdd(Vector2i pos)
	{
		return true;
	}

	public virtual Tile Preprocess(Tile tile, Vector2i pos)
	{
		return tile;
	}

	public virtual void OnDeleted(Vector2i pos)
	{
	}

	public virtual void OnFunction(Vector2i pos)
	{
	}

	public virtual void OnEnter(int tX, int tY, Entity entity)
	{
		entity.wait = false;
	}

	public virtual void SetUVs(Tile tile, MeshData data)
	{
		data.AddUV(meshIndex, new Vector2(1.0f, 0.0f));
		data.AddUV(meshIndex, new Vector2(1.0f, 1.0f));
		data.AddUV(meshIndex, new Vector2(0.0f, 1.0f));
		data.AddUV(meshIndex, new Vector2(0.0f, 0.0f));
	}
}
