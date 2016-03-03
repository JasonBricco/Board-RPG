using UnityEngine;
using System;

public struct Tile : IEquatable<Tile>
{
	private ushort tileID;
	public ushort Data { get; set; }

	public Tile(ushort ID)
	{
		this.tileID = ID;
		this.Data = 0;
	}

	public Tile(ushort ID, ushort data)
	{
		this.tileID = ID;
		this.Data = data;
	}

	public ushort ID
	{
		get { return tileID; }
	}

	public bool Equals(Tile other)
	{
		return other.ID == this.tileID;
	}
}
