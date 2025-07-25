using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	public static class UITools
	{
		public static GameObject InstantiateGUIObject<T>(GameObject prefab, Transform parent, string name) where T : Component
		{
			GameObject gameObject = InstantiateGUIObject_Pre<T>(prefab, parent, name);
			if (gameObject == null)
			{
				return null;
			}
			RectTransform component = gameObject.GetComponent<RectTransform>();
			if (component == null)
			{
				Debug.LogError(name + " prefab is missing RectTransform component!");
			}
			else
			{
				component.localScale = Vector3.one;
			}
			return gameObject;
		}

		public static GameObject InstantiateGUIObject<T>(GameObject prefab, Transform parent, string name, Vector2 pivot, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition) where T : Component
		{
			GameObject gameObject = InstantiateGUIObject_Pre<T>(prefab, parent, name);
			if (gameObject == null)
			{
				return null;
			}
			RectTransform component = gameObject.GetComponent<RectTransform>();
			if (component == null)
			{
				Debug.LogError(name + " prefab is missing RectTransform component!");
			}
			else
			{
				component.localScale = Vector3.one;
				component.pivot = pivot;
				component.anchorMin = anchorMin;
				component.anchorMax = anchorMax;
				component.anchoredPosition = anchoredPosition;
			}
			return gameObject;
		}

		private static GameObject InstantiateGUIObject_Pre<T>(GameObject prefab, Transform parent, string name) where T : Component
		{
			if (prefab == null)
			{
				Debug.LogError(name + " prefab is null!");
				return null;
			}
			GameObject gameObject = Object.Instantiate(prefab);
			if (!string.IsNullOrEmpty(name))
			{
				gameObject.name = name;
			}
			T component = gameObject.GetComponent<T>();
			if (component == null)
			{
				Debug.LogError(name + " prefab is missing the " + component.GetType().ToString() + " component!");
				return null;
			}
			if (parent != null)
			{
				gameObject.transform.SetParent(parent, worldPositionStays: false);
			}
			return gameObject;
		}

		public static Vector3 GetPointOnRectEdge(RectTransform rectTransform, Vector2 dir)
		{
			if (rectTransform == null)
			{
				return Vector3.zero;
			}
			if (dir != Vector2.zero)
			{
				dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
			}
			Rect rect = rectTransform.rect;
			dir = rect.center + Vector2.Scale(rect.size, dir * 0.5f);
			return dir;
		}

		public static Rect GetWorldSpaceRect(RectTransform rt)
		{
			if (rt == null)
			{
				return default(Rect);
			}
			Rect rect = rt.rect;
			Vector2 vector = rt.TransformPoint(new Vector2(rect.xMin, rect.yMin));
			Vector2 vector2 = rt.TransformPoint(new Vector2(rect.xMin, rect.yMax));
			Vector2 vector3 = rt.TransformPoint(new Vector2(rect.xMax, rect.yMin));
			return new Rect(vector.x, vector.y, vector3.x - vector.x, vector2.y - vector.y);
		}

		public static Rect TransformRectTo(Transform from, Transform to, Rect rect)
		{
			Vector3 position;
			Vector3 position2;
			Vector3 position3;
			if (from != null)
			{
				position = from.TransformPoint(new Vector2(rect.xMin, rect.yMin));
				position2 = from.TransformPoint(new Vector2(rect.xMin, rect.yMax));
				position3 = from.TransformPoint(new Vector2(rect.xMax, rect.yMin));
			}
			else
			{
				position = new Vector2(rect.xMin, rect.yMin);
				position2 = new Vector2(rect.xMin, rect.yMax);
				position3 = new Vector2(rect.xMax, rect.yMin);
			}
			if (to != null)
			{
				position = to.InverseTransformPoint(position);
				position2 = to.InverseTransformPoint(position2);
				position3 = to.InverseTransformPoint(position3);
			}
			return new Rect(position.x, position.y, position3.x - position.x, position.y - position2.y);
		}

		public static Rect InvertY(Rect rect)
		{
			return new Rect(rect.xMin, rect.yMin, rect.width, 0f - rect.height);
		}

		public static void SetInteractable(Selectable selectable, bool state, bool playTransition)
		{
			if (selectable == null)
			{
				return;
			}
			if (!playTransition)
			{
				if (selectable.transition == Selectable.Transition.ColorTint)
				{
					ColorBlock colors = selectable.colors;
					float fadeDuration = colors.fadeDuration;
					colors.fadeDuration = 0f;
					selectable.colors = colors;
					selectable.interactable = state;
					colors.fadeDuration = fadeDuration;
					selectable.colors = colors;
				}
				else
				{
					selectable.interactable = state;
				}
			}
			else
			{
				selectable.interactable = state;
			}
		}
	}
}
