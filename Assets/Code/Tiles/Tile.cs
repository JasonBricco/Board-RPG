using UnityEngine;
using System;

public struct Tile : IEquatable<Tile>
{
	private ushort tileID;
	private ushort data;

	public Tile(ushort ID)
	{
		this.tileID = ID;
		this.data = 0;
	}

	public Tile(ushort ID, ushort data)
	{
		this.tileID = ID;
		this.data = data;
	}

	public ushort ID
	{
		get { return tileID; }
	}

	public ushort Data
	{
		get { return data; }
	}

	public bool Equals(Tile other)
	{
		return other.ID == this.tileID;
	}
}
