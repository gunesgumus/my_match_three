namespace GNMS.MatchThree
{
	using System;

	public abstract class PowerUpItem : InteractiveSlidableItem
	{
		public abstract void Combine(PowerUpItem other);
	}
}