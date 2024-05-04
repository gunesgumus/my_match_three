namespace GNMS.MatchThree
{
	public class Propeller : PowerUpItem
	{
		public override void Combine(PowerUpItem other)
		{
			if (other is LightBall lightball)
			{
				other.Combine(this);
			}
			else if (other is Propeller propeller)
			{
				this.CombineWithPropeller(propeller);
			}
			else if (other is Rocket rocket)
			{
				this.CombineWithRocket(rocket);
			}
			else if (other is Tnt tnt)
			{
				this.CombineWithTnt(tnt);
			}
		}

		void CombineWithPropeller(Propeller propeller)
		{

		}

		void CombineWithRocket(Rocket rocket)
		{

		}

		void CombineWithTnt(Tnt tnt)
		{

		}
	}
}