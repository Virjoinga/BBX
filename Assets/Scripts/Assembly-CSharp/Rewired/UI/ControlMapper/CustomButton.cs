using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class CustomButton : Button, ICustomSelectable, ICancelHandler, IEventSystemHandler
	{
		[SerializeField]
		private Sprite _disabledHighlightedSprite;

		[SerializeField]
		private Color _disabledHighlightedColor;

		[SerializeField]
		private string _disabledHighlightedTrigger;

		[SerializeField]
		private bool _autoNavUp = true;

		[SerializeField]
		private bool _autoNavDown = true;

		[SerializeField]
		private bool _autoNavLeft = true;

		[SerializeField]
		private bool _autoNavRight = true;

		private bool isHighlightDisabled;

		public Sprite disabledHighlightedSprite
		{
			get
			{
				return _disabledHighlightedSprite;
			}
			set
			{
				_disabledHighlightedSprite = value;
			}
		}

		public Color disabledHighlightedColor
		{
			get
			{
				return _disabledHighlightedColor;
			}
			set
			{
				_disabledHighlightedColor = value;
			}
		}

		public string disabledHighlightedTrigger
		{
			get
			{
				return _disabledHighlightedTrigger;
			}
			set
			{
				_disabledHighlightedTrigger = value;
			}
		}

		public bool autoNavUp
		{
			get
			{
				return _autoNavUp;
			}
			set
			{
				_autoNavUp = value;
			}
		}

		public bool autoNavDown
		{
			get
			{
				return _autoNavDown;
			}
			set
			{
				_autoNavDown = value;
			}
		}

		public bool autoNavLeft
		{
			get
			{
				return _autoNavLeft;
			}
			set
			{
				_autoNavLeft = value;
			}
		}

		public bool autoNavRight
		{
			get
			{
				return _autoNavRight;
			}
			set
			{
				_autoNavRight = value;
			}
		}

		private bool isDisabled => !IsInteractable();

		private event UnityAction _CancelEvent;

		public event UnityAction CancelEvent
		{
			add
			{
				_CancelEvent += value;
			}
			remove
			{
				_CancelEvent -= value;
			}
		}

		public override Selectable FindSelectableOnLeft()
		{
			if ((base.navigation.mode & Navigation.Mode.Horizontal) != Navigation.Mode.None || _autoNavLeft)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.left);
			}
			return base.FindSelectableOnLeft();
		}

		public override Selectable FindSelectableOnRight()
		{
			if ((base.navigation.mode & Navigation.Mode.Horizontal) != Navigation.Mode.None || _autoNavRight)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.right);
			}
			return base.FindSelectableOnRight();
		}

		public override Selectable FindSelectableOnUp()
		{
			if ((base.navigation.mode & Navigation.Mode.Vertical) != Navigation.Mode.None || _autoNavUp)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.up);
			}
			return base.FindSelectableOnUp();
		}

		public override Selectable FindSelectableOnDown()
		{
			if ((base.navigation.mode & Navigation.Mode.Vertical) != Navigation.Mode.None || _autoNavDown)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.down);
			}
			return base.FindSelectableOnDown();
		}

		protected override void OnCanvasGroupChanged()
		{
			base.OnCanvasGroupChanged();
			if (!(EventSystem.current == null))
			{
				EvaluateHightlightDisabled(EventSystem.current.currentSelectedGameObject == base.gameObject);
			}
		}

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			if (isHighlightDisabled)
			{
				Color color = _disabledHighlightedColor;
				Sprite newSprite = _disabledHighlightedSprite;
				string triggername = _disabledHighlightedTrigger;
				if (base.gameObject.activeInHierarchy)
				{
					switch (base.transition)
					{
					case Transition.ColorTint:
						StartColorTween(color * base.colors.colorMultiplier, instant);
						break;
					case Transition.SpriteSwap:
						DoSpriteSwap(newSprite);
						break;
					case Transition.Animation:
						TriggerAnimation(triggername);
						break;
					}
				}
			}
			else
			{
				base.DoStateTransition(state, instant);
			}
		}

		private void StartColorTween(Color targetColor, bool instant)
		{
			if (!(base.targetGraphic == null))
			{
				base.targetGraphic.CrossFadeColor(targetColor, instant ? 0f : base.colors.fadeDuration, ignoreTimeScale: true, useAlpha: true);
			}
		}

		private void DoSpriteSwap(Sprite newSprite)
		{
			if (!(base.image == null))
			{
				base.image.overrideSprite = newSprite;
			}
		}

		private void TriggerAnimation(string triggername)
		{
			if (!(base.animator == null) && base.animator.enabled && base.animator.isActiveAndEnabled && !(base.animator.runtimeAnimatorController == null) && !string.IsNullOrEmpty(triggername))
			{
				base.animator.ResetTrigger(_disabledHighlightedTrigger);
				base.animator.SetTrigger(triggername);
			}
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			EvaluateHightlightDisabled(isSelected: true);
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			EvaluateHightlightDisabled(isSelected: false);
		}

		private void Press()
		{
			if (IsActive() && IsInteractable())
			{
				base.onClick.Invoke();
			}
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			if (IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left)
			{
				Press();
				if (!IsActive() || !IsInteractable())
				{
					isHighlightDisabled = true;
					DoStateTransition(SelectionState.Disabled, instant: false);
				}
			}
		}

		public override void OnSubmit(BaseEventData eventData)
		{
			Press();
			if (!IsActive() || !IsInteractable())
			{
				isHighlightDisabled = true;
				DoStateTransition(SelectionState.Disabled, instant: false);
			}
			else
			{
				DoStateTransition(SelectionState.Pressed, instant: false);
				StartCoroutine(OnFinishSubmit());
			}
		}

		private IEnumerator OnFinishSubmit()
		{
			float fadeTime = base.colors.fadeDuration;
			float elapsedTime = 0f;
			while (elapsedTime < fadeTime)
			{
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}
			DoStateTransition(base.currentSelectionState, instant: false);
		}

		private void EvaluateHightlightDisabled(bool isSelected)
		{
			if (!isSelected)
			{
				if (isHighlightDisabled)
				{
					isHighlightDisabled = false;
					SelectionState state = (isDisabled ? SelectionState.Disabled : base.currentSelectionState);
					DoStateTransition(state, instant: false);
				}
			}
			else if (isDisabled)
			{
				isHighlightDisabled = true;
				DoStateTransition(SelectionState.Disabled, instant: false);
			}
		}

		public void OnCancel(BaseEventData eventData)
		{
			if (this._CancelEvent != null)
			{
				this._CancelEvent();
			}
		}
	}
}
