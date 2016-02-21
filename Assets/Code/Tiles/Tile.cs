//
//  Tile.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public class Tile  
{
	public const int SizeBits = 5;
	public const int Size = 1 << SizeBits;
	public const int HalfSize = Size / 2;

	protected static MeshBuilder meshBuilder = new MeshBuilder();

	protected string name = "Unassigned";
	protected ushort tileID = 0;
	protected byte meshIndex = 0;
	protected int posIndex = 0;

	public string Name { get { return name; } }
	public ushort ID { get { return tileID; } }
	public byte MeshIndex { get { return meshIndex; } }
	public int PosIndex { get { return posIndex; } }

	public virtual void Build(int tX, int tY, MeshData data, bool overlay)
	{
		meshBuilder.BuildSquare(meshIndex, tX, tY, data, overlay);
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

	public virtual void OnFunction()
	{
	}

	public virtual void OnEnter(int tX, int tY, Entity entity)
	{
	}
}
