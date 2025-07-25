using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	public static class MovementUtility
	{
		private static Dictionary<GameObject, Dictionary<Type, Component>> gameObjectComponentMap = new Dictionary<GameObject, Dictionary<Type, Component>>();

		private static Dictionary<GameObject, Dictionary<Type, Component[]>> gameObjectComponentsMap = new Dictionary<GameObject, Dictionary<Type, Component[]>>();

		public static GameObject WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, LayerMask objectLayerMask, Vector3 targetOffset, LayerMask ignoreLayerMask, bool useTargetBone, HumanBodyBones targetBone)
		{
			GameObject result = null;
			Collider[] array = Physics.OverlapSphere(transform.position, viewDistance, objectLayerMask);
			if (array != null)
			{
				float num = float.PositiveInfinity;
				for (int i = 0; i < array.Length; i++)
				{
					GameObject gameObject;
					if ((gameObject = WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, array[i].gameObject, targetOffset, usePhysics2D: false, 0f, out var angle, ignoreLayerMask, useTargetBone, targetBone)) != null && angle < num)
					{
						num = angle;
						result = gameObject;
					}
				}
			}
			return result;
		}

		public static GameObject WithinSight2D(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, LayerMask objectLayerMask, Vector3 targetOffset, float angleOffset2D, LayerMask ignoreLayerMask)
		{
			GameObject result = null;
			Collider2D[] array = Physics2D.OverlapCircleAll(transform.position, viewDistance, objectLayerMask);
			if (array != null)
			{
				float num = float.PositiveInfinity;
				for (int i = 0; i < array.Length; i++)
				{
					GameObject gameObject;
					if ((gameObject = WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, array[i].gameObject, targetOffset, usePhysics2D: true, angleOffset2D, out var angle, ignoreLayerMask, useTargetBone: false, HumanBodyBones.Hips)) != null && angle < num)
					{
						num = angle;
						result = gameObject;
					}
				}
			}
			return result;
		}

		public static GameObject WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject, Vector3 targetOffset, LayerMask ignoreLayerMask, bool useTargetBone, HumanBodyBones targetBone)
		{
			float angle;
			return WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, targetObject, targetOffset, usePhysics2D: false, 0f, out angle, ignoreLayerMask, useTargetBone, targetBone);
		}

		public static GameObject WithinSight2D(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject, Vector3 targetOffset, float angleOffset2D, LayerMask ignoreLayerMask, bool useTargetBone, HumanBodyBones targetBone)
		{
			float angle;
			return WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, targetObject, targetOffset, usePhysics2D: true, angleOffset2D, out angle, ignoreLayerMask, useTargetBone, targetBone);
		}

		public static GameObject WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject, Vector3 targetOffset, bool usePhysics2D, float angleOffset2D, out float angle, int ignoreLayerMask, bool useTargetBone, HumanBodyBones targetBone)
		{
			if (targetObject == null)
			{
				angle = 0f;
				return null;
			}
			Animator componentForType;
			if (useTargetBone && (componentForType = GetComponentForType<Animator>(targetObject)) != null)
			{
				Transform boneTransform = componentForType.GetBoneTransform(targetBone);
				if (boneTransform != null)
				{
					targetObject = boneTransform.gameObject;
				}
			}
			Vector3 vector = targetObject.transform.position - transform.TransformPoint(positionOffset);
			if (usePhysics2D)
			{
				Vector3 eulerAngles = transform.eulerAngles;
				eulerAngles.z -= angleOffset2D;
				angle = Vector3.Angle(vector, Quaternion.Euler(eulerAngles) * Vector3.up);
				vector.z = 0f;
			}
			else
			{
				angle = Vector3.Angle(vector, transform.forward);
				vector.y = 0f;
			}
			if (vector.magnitude < viewDistance && angle < fieldOfViewAngle * 0.5f)
			{
				if (LineOfSight(transform, positionOffset, targetObject, targetOffset, usePhysics2D, ignoreLayerMask) != null)
				{
					return targetObject;
				}
				if (GetComponentForType<Collider>(targetObject) == null && GetComponentForType<Collider2D>(targetObject) == null && targetObject.gameObject.activeSelf)
				{
					return targetObject;
				}
			}
			return null;
		}

		public static GameObject LineOfSight(Transform transform, Vector3 positionOffset, GameObject targetObject, Vector3 targetOffset, bool usePhysics2D, int ignoreLayerMask)
		{
			RaycastHit hitInfo;
			if (usePhysics2D)
			{
				RaycastHit2D raycastHit2D;
				if ((bool)(raycastHit2D = Physics2D.Linecast(transform.TransformPoint(positionOffset), targetObject.transform.TransformPoint(targetOffset), ~ignoreLayerMask)) && (raycastHit2D.transform.IsChildOf(targetObject.transform) || targetObject.transform.IsChildOf(raycastHit2D.transform)))
				{
					return targetObject;
				}
			}
			else if (Physics.Linecast(transform.TransformPoint(positionOffset), targetObject.transform.TransformPoint(targetOffset), out hitInfo, ~ignoreLayerMask) && (hitInfo.transform.IsChildOf(targetObject.transform) || targetObject.transform.IsChildOf(hitInfo.transform)))
			{
				return targetObject;
			}
			return null;
		}

		public static GameObject WithinHearingRange(Transform transform, Vector3 positionOffset, float audibilityThreshold, float hearingRadius, LayerMask objectLayerMask)
		{
			GameObject result = null;
			Collider[] array = Physics.OverlapSphere(transform.TransformPoint(positionOffset), hearingRadius, objectLayerMask);
			if (array != null)
			{
				float num = 0f;
				for (int i = 0; i < array.Length; i++)
				{
					float audibility = 0f;
					GameObject gameObject;
					if ((gameObject = WithinHearingRange(transform, positionOffset, audibilityThreshold, array[i].gameObject, ref audibility)) != null && audibility > num)
					{
						num = audibility;
						result = gameObject;
					}
				}
			}
			return result;
		}

		public static GameObject WithinHearingRange2D(Transform transform, Vector3 positionOffset, float audibilityThreshold, float hearingRadius, LayerMask objectLayerMask)
		{
			GameObject result = null;
			Collider2D[] array = Physics2D.OverlapCircleAll(transform.TransformPoint(positionOffset), hearingRadius, objectLayerMask);
			if (array != null)
			{
				float num = 0f;
				for (int i = 0; i < array.Length; i++)
				{
					float audibility = 0f;
					GameObject gameObject;
					if ((gameObject = WithinHearingRange(transform, positionOffset, audibilityThreshold, array[i].gameObject, ref audibility)) != null && audibility > num)
					{
						num = audibility;
						result = gameObject;
					}
				}
			}
			return result;
		}

		public static GameObject WithinHearingRange(Transform transform, Vector3 positionOffset, float audibilityThreshold, GameObject targetObject)
		{
			float audibility = 0f;
			return WithinHearingRange(transform, positionOffset, audibilityThreshold, targetObject, ref audibility);
		}

		public static GameObject WithinHearingRange(Transform transform, Vector3 positionOffset, float audibilityThreshold, GameObject targetObject, ref float audibility)
		{
			AudioSource[] componentsForType;
			if ((componentsForType = GetComponentsForType<AudioSource>(targetObject)) != null)
			{
				for (int i = 0; i < componentsForType.Length; i++)
				{
					if (componentsForType[i].isPlaying)
					{
						float num = Vector3.Distance(transform.position, targetObject.transform.position);
						if (componentsForType[i].rolloffMode == AudioRolloffMode.Logarithmic)
						{
							audibility = componentsForType[i].volume / Mathf.Max(componentsForType[i].minDistance, num - componentsForType[i].minDistance);
						}
						else
						{
							audibility = componentsForType[i].volume * Mathf.Clamp01((num - componentsForType[i].minDistance) / (componentsForType[i].maxDistance - componentsForType[i].minDistance));
						}
						if (audibility > audibilityThreshold)
						{
							return targetObject;
						}
					}
				}
			}
			return null;
		}

		public static void DrawLineOfSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float angleOffset, float viewDistance, bool usePhysics2D)
		{
		}

		public static T GetComponentForType<T>(GameObject target) where T : Component
		{
			Component value2;
			if (gameObjectComponentMap.TryGetValue(target, out var value))
			{
				if (value.TryGetValue(typeof(T), out value2))
				{
					return value2 as T;
				}
			}
			else
			{
				value = new Dictionary<Type, Component>();
				gameObjectComponentMap.Add(target, value);
			}
			value2 = target.GetComponent<T>();
			value.Add(typeof(T), value2);
			return value2 as T;
		}

		public static T[] GetComponentsForType<T>(GameObject target) where T : Component
		{
			Component[] value2;
			if (gameObjectComponentsMap.TryGetValue(target, out var value))
			{
				if (value.TryGetValue(typeof(T), out value2))
				{
					return value2 as T[];
				}
			}
			else
			{
				value = new Dictionary<Type, Component[]>();
				gameObjectComponentsMap.Add(target, value);
			}
			Component[] components = target.GetComponents<T>();
			value2 = components;
			value.Add(typeof(T), value2);
			return value2 as T[];
		}

		public static void ClearCache()
		{
			gameObjectComponentMap.Clear();
			gameObjectComponentsMap.Clear();
		}
	}
}
