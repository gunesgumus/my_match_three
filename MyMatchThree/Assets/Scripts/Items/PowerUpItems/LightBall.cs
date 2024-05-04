namespace GNMS.MatchThree
{
	public class LightBall : PowerUpItem
	{
		public override void Combine(PowerUpItem other)
		{
			if (other is LightBall lightball)
			{
				this.CombineWithLightBall(lightball);
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

		void CombineWithLightBall(LightBall lightBall)
		{

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