using UnityEngine;

public class SwapFunction : Function 
{
	public SwapFunction(BoardManager manager) : base(manager) {}

	public override void Compute(string[] args, Entity entity)
	{
		Entity other = boardManager.GetEntity((entity.EntityID + 1) & 1);

		Vector3 otherPos = other.Position;
		other.SetTo(entity.Position);
		entity.SetTo(otherPos);
	}
}
