using System;

namespace GNMS.MatchThree
{
	public abstract class PowerUpItem : InteractiveSlidableItem
	{
		public abstract void Combine(PowerUpItem other);
	}
}