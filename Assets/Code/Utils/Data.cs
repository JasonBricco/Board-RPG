using UnityEngine;

public sealed class Data
{
	public int num, secondNum;
	public GameState state;
	public string[] strings;
	public Entity entity, secondEntity;
	public Vector3 position;

	public Data(int num, int secondNum = 0, int thirdNum = 0, int fourthNum = 0)
	{
		this.num = num;
		this.secondNum = secondNum;
	}

	public Data(GameState state)
	{
		this.state = state;
	}

	public Data(Vector3 position)
	{
		this.position = position;
	}

	public Data(int num, string[] strings, Entity entity) : this(num)
	{
		this.strings = strings;
		this.entity = entity;
	}

	public Data(Entity entity, Entity secondEntity = null)
	{
		this.entity = entity;
		this.secondEntity = secondEntity;
	}
}
