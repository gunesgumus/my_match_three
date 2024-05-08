namespace GNMS.MatchThree
{
	using UnityEngine;

	public class Tnt : PowerUpItem
	{
		public override bool CheckBoardPlacement(Board board)
		{
			return false;
		}

		public override void Combine(PowerUpItem other)
		{
			if (other is LightBall lightball)
			{
				other.Combine(this);
			}
			else if (other is Propeller propeller)
			{
				other.Combine(this);
			}
			else if (other is Rocket rocket)
			{
				other.Combine(this);
			}
			else if (other is Tnt tnt)
			{
				this.CombineWithTnt(tnt);
			}
		}

		void CombineWithTnt(Tnt tnt)
		{

		}
	}
}