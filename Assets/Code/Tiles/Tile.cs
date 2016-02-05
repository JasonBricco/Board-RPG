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
	protected static MeshBuilder meshBuilder = new MeshBuilder();
	protected ushort tileID;
	protected byte meshIndex;

	public ushort ID
	{
		get { return tileID; }
	}

	public byte MeshIndex
	{
		get { return meshIndex; }
	}

	public virtual void Build(int x, int z, MeshData data)
	{
		meshBuilder.BuildSquare(meshIndex, x, z, data);
	}
}
