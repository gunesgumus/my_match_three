using UnityEngine;

namespace GNMS.MatchThree
{
	public class Board : MonoBehaviour
	{
		BoardStateMachine boardStateMachine;

		private void Awake()
		{
			this.InitializeBoardContent();
			this.boardStateMachine = this.gameObject.AddComponent<BoardStateMachine>();
		}

		void InitializeBoardContent()
		{

		}
	}
}
