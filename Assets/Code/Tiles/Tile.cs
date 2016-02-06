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
	protected ushort tileID = 0;
	protected byte meshIndex = 0;
	protected bool isOverlay = false;

	public ushort ID
	{
		get { return tileID; }
	}

	public byte MeshIndex
	{
		get { return meshIndex; }
	}

	public bool IsOverlay
	{
		get { return isOverlay; }
	}

	public virtual void Build(int x, int z, MeshData data, bool overlay)
	{
		meshBuilder.BuildSquare(meshIndex, x, z, data, overlay);
	}
}
