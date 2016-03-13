using UnityEngine;
using System;

public struct Tile : IEquatable<Tile>
{
	public const int SizeBits = 5;
	public const int Size = 1 << SizeBits;
	public const int HalfSize = Size / 2;

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
