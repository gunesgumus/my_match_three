namespace GNMS.MatchThree
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	public enum ItemSlideDirection : byte
	{
		Left,
		Right,
		Up,
		Down,
	}

	public static class ItemSlideDirectionExtensions
	{
		public static Vector2Int ConvertToVector2Int(this ItemSlideDirection slideDirection)
		{
			switch (slideDirection)
			{
				case ItemSlideDirection.Left:
					return Vector2Int.left;
				case ItemSlideDirection.Right:
					return Vector2Int.right;
				case ItemSlideDirection.Up:
					return Vector2Int.up;
				case ItemSlideDirection.Down:
					return Vector2Int.down;
			}
			return Vector2Int.zero;
		}

		public static ItemSlideDirection ConvertToItemSlideDirection(this Vector3 slide)
		{
			return (Mathf.Abs(slide.x) >= Mathf.Abs(slide.y)) ?
				(slide.x > 0f ? ItemSlideDirection.Right : ItemSlideDirection.Left) :
				(slide.y > 0f ? ItemSlideDirection.Up : ItemSlideDirection.Down);
		}
	}

	public class BoardInteractionOperative : MonoBehaviour
	{
		public Action<InteractiveSlidableItem, ItemSlideDirection> OnItemSlideInput;

		Board board;

		float minimumSlideDistance = 0.5f;

		InteractiveSlidableItem interactedSlidableItem = null;
		Vector3 interactionStartPosition;

		public float MinimumSlideDistance
		{
			get => this.minimumSlideDistance;
			set => minimumSlideDistance = value;
		}

		private void Awake()
		{
			this.board = this.GetComponent<Board>();
		}

		private void Update()
		{
			this.HandleMouseDown();
			this.HandleMouseSlide();
			this.HandleMouseUp();
		}

		void HandleMouseDown()
		{
			if (!Input.GetMouseButtonDown(0) || this.MouseInputIsObstructedByUI())
			{
				return;
			}

			this.StartInteraction();
		}

		bool MouseInputIsObstructedByUI()
		{
			List<RaycastResult> uiRaycastResults = new List<RaycastResult>();
			EventSystem.current.RaycastAll(
				new PointerEventData(EventSystem.current) { position = Input.mousePosition },
				uiRaycastResults);
			return uiRaycastResults.Count > 0;
		}

		void StartInteraction()
		{
			this.interactionStartPosition = this.GetMouseWorldPosition();
			this.interactedSlidableItem = this.board.GetItemAtPosition(this.interactionStartPosition) as InteractiveSlidableItem;
			if (this.interactedSlidableItem == null)
			{
				return;
			}
		}

		void HandleMouseSlide()
		{
			if (!Input.GetMouseButton(0) || this.interactedSlidableItem == null)
			{
				return;
			}

			Vector3 mouseWorldPosition = this.GetMouseWorldPosition();
			Vector3 slide = mouseWorldPosition - this.interactionStartPosition;
			if (Mathf.Abs(slide.x) <= this.minimumSlideDistance && Mathf.Abs(slide.y) <= this.minimumSlideDistance)
			{
				return;
			}

			ItemSlideDirection slideDirection = slide.ConvertToItemSlideDirection();

			this.OnItemSlideInput?.Invoke(
				this.interactedSlidableItem,
				slideDirection);
			this.interactedSlidableItem = null;
		}

		void HandleMouseUp()
		{
			if (!Input.GetMouseButtonUp(0) || this.interactedSlidableItem == null)
			{
				return;
			}

			Vector3 interactionEndPosition = this.GetMouseWorldPosition();
			Vector3 interactionSlide = interactionEndPosition - this.interactionStartPosition;

			// TODO: if not enough distance for a slide, perform a click on the item if it allows
			// Implement this after adding power-ups

			this.interactedSlidableItem = null;
		}

		Vector3 GetMouseWorldPosition()
		{
			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouseWorldPosition.z = 0f;
			return mouseWorldPosition;
		}
	}
}