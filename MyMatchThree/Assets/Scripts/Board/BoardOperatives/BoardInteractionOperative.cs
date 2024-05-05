namespace GNMS.MatchThree
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	public enum ItemSlideInput : byte
	{
		Right,
		Left,
		Up,
		Down,
	}

	public class BoardInteractionOperative : MonoBehaviour
	{
		public Action<ItemSlideInput> OnItemSlideInput;

		Board board;

		InteractiveSlidableItem interactedSlidableItem = null;
		Vector3 interactionStartPosition;

		private void Awake()
		{
			this.board = this.GetComponent<Board>();
		}

		private void Update()
		{
			this.HandleMouseDown();
			this.HandleMouseUp();
		}

		void HandleMouseDown()
		{
			if (Input.GetMouseButtonDown(0))
			{
				List<RaycastResult> eventSystemRaycastResults = new List<RaycastResult>();
				EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current), eventSystemRaycastResults);
				if (eventSystemRaycastResults.Count > 0)
				{
					return;
				}
				this.interactionStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				this.interactedSlidableItem = this.board.GetItemAtPosition(this.interactionStartPosition) as InteractiveSlidableItem;
				if (this.interactedSlidableItem != null)
				{
					Debug.Log($"obtained item: {this.interactedSlidableItem}, is null {this.interactedSlidableItem == null}", this.interactedSlidableItem.gameObject);
				}
				else
				{
					Debug.Log($"no item at position");
				}
			}
		}

		void HandleMouseUp()
		{
			if (this.interactedSlidableItem == null)
			{
				return;
			}

			if (Input.GetMouseButtonUp(0))
			{
				Vector3 interactionEndPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector3 interactionSlide = interactionEndPosition - this.interactionStartPosition;

				this.interactedSlidableItem = null;
			}
		}
	}
}