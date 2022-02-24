using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Utilities
{
	public static class TransformUtils
	{
		private static List<ActiveRoutine> CurrentActiveRoutines
		{
			get
			{
				List<ActiveRoutine> result;
				if ((result = currentActiveRoutines) == null)
				{
					result = (currentActiveRoutines = new List<ActiveRoutine>());
				}
				return result;
			}
		}

		private static void StopActiveCoroutines(Transform t, CoroutineType coroutineType)
		{
			CurrentActiveRoutines.RemoveAll((ActiveRoutine c) => c.Coroutine == null);
			Predicate<ActiveRoutine> match = (ActiveRoutine r) => (r.Transform == t && r.CoroutineType == coroutineType) || r.Coroutine == null;
			CurrentActiveRoutines.FindAll(match).ForEach(delegate(ActiveRoutine r)
			{
				CoroutineUtils.StopCoroutine(r.Coroutine);
			});
			CurrentActiveRoutines.RemoveAll(match);
		}

		private static Coroutine StartActiveCoroutine(Transform t, IEnumerator enumerator, CoroutineType coroutineType)
		{
			StopActiveCoroutines(t, coroutineType);
			Coroutine coroutine = CoroutineUtils.StartCoroutine(enumerator);
			CurrentActiveRoutines.Add(new ActiveRoutine
			{
				Coroutine = coroutine,
				Transform = t,
				CoroutineType = coroutineType
			});
			return coroutine;
		}

		public static void StopCoroutine([NotNull] this Transform t, Coroutine c)
		{
			ActiveRoutine activeRoutine = CurrentActiveRoutines.Find((ActiveRoutine a) => a.Coroutine == c && a.Transform == t);
			if (activeRoutine == null)
			{
				return;
			}
			CoroutineUtils.StopCoroutine(activeRoutine.Coroutine);
			CurrentActiveRoutines.Remove(activeRoutine);
		}

		public static void StopAllTransformCoroutines([NotNull] this Transform t)
		{
			List<ActiveRoutine> list = (from a in CurrentActiveRoutines
			where a.Transform == t
			select a).ToList<ActiveRoutine>();
			if (list.Count == 0)
			{
				return;
			}
			foreach (ActiveRoutine activeRoutine in list)
			{
				if (activeRoutine.Coroutine != null)
				{
					CoroutineUtils.StopCoroutine(activeRoutine.Coroutine);
				}
				CurrentActiveRoutines.Remove(activeRoutine);
			}
		}

		public static Coroutine MovePosition([NotNull] this Transform t, Vector3 newPos, float time)
		{
			return StartActiveCoroutine(t, t.MovePosition(false, newPos, time, null, LerpType.Linear, null, false, null), CoroutineType.Position);
		}

		public static Coroutine MovePosition([NotNull] this Transform t, Vector3 newPos, float time, Action callback)
		{
			return StartActiveCoroutine(t, t.MovePosition(false, newPos, time, null, LerpType.Linear, null, false, callback), CoroutineType.Position);
		}

		public static Coroutine MovePosition([NotNull] this Transform t, Vector3 newPos, float time, Action callback, Reference<float> multiplier)
		{
			return StartActiveCoroutine(t, t.MovePosition(false, newPos, time, multiplier, LerpType.Linear, null, false, callback), CoroutineType.Position);
		}

		public static Coroutine MovePosition([NotNull] this Transform t, Vector3 newPos, float time, AnimationCurve curve, Action callback)
		{
			return StartActiveCoroutine(t, t.MovePosition(false, newPos, time, null, LerpType.Linear, curve, false, callback), CoroutineType.Position);
		}

		public static Coroutine MovePosition([NotNull] this Transform t, Vector3 newPos, float time, AnimationCurve curve)
		{
			return StartActiveCoroutine(t, t.MovePosition(false, newPos, time, null, LerpType.Linear, curve, false, null), CoroutineType.Position);
		}

		public static Coroutine MovePositionX([NotNull] this Transform t, float newX, float time, AnimationCurve curve)
		{
			return StartActiveCoroutine(t, MovePositionElement(t, false, newX, 0, time, null, LerpType.Linear, curve, false, null), CoroutineType.PositionX);
		}

		public static Coroutine MovePositionX([NotNull] this Transform t, float newX, float time, AnimationCurve curve, Action callback)
		{
			return StartActiveCoroutine(t, MovePositionElement(t, false, newX, 0, time, null, LerpType.Linear, curve, false, callback), CoroutineType.PositionX);
		}

		public static Coroutine MovePositionX([NotNull] this Transform t, float newX, float time, Action callback)
		{
			return StartActiveCoroutine(t, MovePositionElement(t, false, newX, 0, time, null, LerpType.Linear, null, false, callback), CoroutineType.PositionX);
		}

		public static Coroutine MovePositionX([NotNull] this Transform t, float newX, float time)
		{
			return StartActiveCoroutine(t, MovePositionElement(t, false, newX, 0, time, null, LerpType.Linear, null, false, null), CoroutineType.PositionX);
		}

		public static Coroutine MovePositionY([NotNull] this Transform t, float newY, float time, AnimationCurve curve = null)
		{
			return StartActiveCoroutine(t, MovePositionElement(t, false, newY, 1, time, null, LerpType.Linear, curve, false, null), CoroutineType.PositionY);
		}

		public static Coroutine MovePositionZ([NotNull] this Transform t, float newZ, float time, AnimationCurve curve = null)
		{
			return StartActiveCoroutine(t, MovePositionElement(t, false, newZ, 2, time, null, LerpType.Linear, curve, false, null), CoroutineType.PositionZ);
		}

		public static Coroutine MovePositionZ([NotNull] this Transform t, float newZ, float time, Action callback)
		{
			return StartActiveCoroutine(t, MovePositionElement(t, false, newZ, 2, time, null, LerpType.Linear, null, false, callback), CoroutineType.PositionZ);
		}

		public static Coroutine MoveLocalPosition([NotNull] this Transform t, Vector3 newPos, float time)
		{
			return StartActiveCoroutine(t, t.MovePosition(true, newPos, time, null, LerpType.Linear, null, false, null), CoroutineType.Position);
		}

		public static Coroutine MoveLocalPosition([NotNull] this Transform t, Vector3 newPos, float time, AnimationCurve curve)
		{
			return StartActiveCoroutine(t, t.MovePosition(true, newPos, time, null, LerpType.Linear, curve, false, null), CoroutineType.Position);
		}

		public static Coroutine MoveLocalPositionX([NotNull] this Transform t, float newX, float time)
		{
			return StartActiveCoroutine(t, MovePositionElement(t, true, newX, 0, time, null, LerpType.Linear, null, false, null), CoroutineType.PositionX);
		}

		public static Coroutine MoveLocalPositionY([NotNull] this Transform t, float newY, float time)
		{
			return StartActiveCoroutine(t, MovePositionElement(t, true, newY, 1, time, null, LerpType.Linear, null, false, null), CoroutineType.PositionY);
		}

		public static Coroutine MoveLocalPositionY([NotNull] this Transform t, float newY, float time, AnimationCurve curve)
		{
			return StartActiveCoroutine(t, MovePositionElement(t, true, newY, 1, time, null, LerpType.Linear, curve, false, null), CoroutineType.PositionY);
		}

		public static Coroutine MoveLocalPositionZ([NotNull] this Transform t, float newZ, float time)
		{
			return StartActiveCoroutine(t, MovePositionElement(t, true, newZ, 2, time, null, LerpType.Linear, null, false, null), CoroutineType.PositionZ);
		}

		public static Coroutine Rotate([NotNull] Transform t, Quaternion newRot, float time)
		{
			return StartActiveCoroutine(t, t.LerpRotation(false, newRot, time, LerpType.Linear, null, null), CoroutineType.Rotation);
		}

		public static Coroutine RotateLocal([NotNull] Transform t, Quaternion newRot, float time)
		{
			return StartActiveCoroutine(t, t.LerpRotation(true, newRot, time, LerpType.Linear, null, null), CoroutineType.Rotation);
		}

		public static Coroutine RotateAround([NotNull] this Transform t, Vector3 pos, Vector3 axis, float angle, float time)
		{
			return StartActiveCoroutine(t, RotateAroundRoutine(t, pos, axis, angle, time), CoroutineType.Rotation);
		}

		public static Coroutine MoveScale([NotNull] this Transform t, Vector3 scale, float time, Action postExecute = null)
		{
			return StartActiveCoroutine(t, LerpScale(t, scale, time, null, postExecute), CoroutineType.Scale);
		}

		public static Coroutine MoveScale([NotNull] this Transform t, Vector3 scale, float time, AnimationCurve curve, Action postExecute = null)
		{
			return StartActiveCoroutine(t, LerpScale(t, scale, time, curve, postExecute), CoroutineType.Scale);
		}

		public static IEnumerator MovePositionIEnumerator([NotNull] this Transform objectTransform, Vector3 newPosition, float time, AnimationCurve curve)
		{
			return objectTransform.MovePosition(false, newPosition, time, null, LerpType.Linear, curve, false, null);
		}

		private static IEnumerator MovePosition([NotNull] this Transform objectTransform, bool local, Vector3 newPosition, float time, Reference<float> multiplier, LerpType lerpType, AnimationCurve curve, bool isFixedUpdate, Action postExecute)
		{
			if (time <= 0f)
			{
				if (local)
				{
					objectTransform.localPosition = newPosition;
				}
				else
				{
					objectTransform.position = newPosition;
				}
				if (postExecute != null)
				{
					postExecute();
				}
				yield break;
			}
			float t = 0f;
			Vector3 initialPosition = local ? objectTransform.localPosition : objectTransform.position;
			while (t < time)
			{
				t = Mathf.Min(time, t + Time.deltaTime * ((multiplier != null) ? multiplier.Value : 1f));
				Vector3 vector;
				if (lerpType != LerpType.Linear)
				{
					if (lerpType != LerpType.Spherical)
					{
						throw new ArgumentOutOfRangeException("lerpType", lerpType, null);
					}
					vector = Vector3.Slerp(initialPosition, newPosition, (curve != null) ? curve.Evaluate(t / time) : (t / time));
				}
				else
				{
					vector = Vector3.Lerp(initialPosition, newPosition, (curve != null) ? curve.Evaluate(t / time) : (t / time));
				}
				if (local)
				{
					objectTransform.localPosition = vector;
				}
				else
				{
					objectTransform.position = vector;
				}
				if (isFixedUpdate)
				{
					yield return new WaitForFixedUpdate();
				}
				else
				{
					yield return null;
				}
			}
			if (local)
			{
				objectTransform.localPosition = newPosition;
			}
			else
			{
				objectTransform.position = newPosition;
			}
			if (postExecute != null)
			{
				postExecute();
			}
			yield break;
		}

		private static IEnumerator MovePositionElement(Transform objectTransform, bool local, float newElement, int index, float time, Reference<float> multiplier, LerpType lerpType, AnimationCurve curve, bool isFixedUpdate, Action postExecute)
		{
			if (index < 0 || index > 2)
			{
				yield break;
			}
			if (time <= 0f)
			{
				if (local)
				{
					Vector3 localPosition = objectTransform.localPosition;
					localPosition[index] = newElement;
					objectTransform.localPosition = localPosition;
				}
				else
				{
					Vector3 position = objectTransform.position;
					position[index] = newElement;
					objectTransform.position = position;
				}
				if (postExecute != null)
				{
					postExecute();
				}
				yield break;
			}
			float t = 0f;
			float initialElement = local ? objectTransform.localPosition[index] : objectTransform.position[index];
			while (t < time)
			{
				t = Mathf.Min(time, t + Time.deltaTime * ((multiplier != null) ? multiplier.Value : 1f));
				Vector3 b;
				Vector3 a = b = (local ? objectTransform.localPosition : objectTransform.position);
				a[index] = initialElement;
				b[index] = newElement;
				Vector3 vector;
				if (lerpType != LerpType.Linear)
				{
					if (lerpType != LerpType.Spherical)
					{
						throw new ArgumentOutOfRangeException("lerpType", lerpType, null);
					}
					vector = Vector3.Slerp(a, b, (curve != null) ? curve.Evaluate(t / time) : (t / time));
				}
				else
				{
					vector = Vector3.Lerp(a, b, (curve != null) ? curve.Evaluate(t / time) : (t / time));
				}
				if (local)
				{
					objectTransform.localPosition = vector;
				}
				else
				{
					objectTransform.position = vector;
				}
				yield return isFixedUpdate ? new WaitForFixedUpdate() : null;
			}
			if (postExecute != null)
			{
				postExecute();
			}
			yield break;
		}

		public static IEnumerator LerpScale(Transform objectTransform, Vector3 b, float time, AnimationCurve curve = null, Action postExecute = null)
		{
			float t = 0f;
			Vector3 a = objectTransform.localScale;
			bool isCurve = curve != null;
			bool isReversedCurve = curve != null && ((curve.keys.First<Keyframe>().value > curve.keys.Last<Keyframe>().value && a.magnitude < b.magnitude) || (curve.keys.First<Keyframe>().value < curve.keys.Last<Keyframe>().value && a.magnitude > b.magnitude));
			while (t < time)
			{
				t = Mathf.Min(time, t + Time.deltaTime);
				float t2;
				if (isCurve)
				{
					float time2 = isReversedCurve ? (1f - t / time) : (t / time);
					t2 = (isReversedCurve ? (1f - curve.Evaluate(time2)) : curve.Evaluate(time2));
				}
				else
				{
					t2 = t / time;
				}
				Vector3 localScale = Vector3.LerpUnclamped(a, b, t2);
				if (!objectTransform)
				{
					yield break;
				}
				objectTransform.localScale = localScale;
				yield return null;
			}
			if (postExecute != null)
			{
				postExecute();
			}
			yield break;
		}

		public static IEnumerator LerpRotation([NotNull] this Transform objectTransform, bool local, Quaternion newRotation, float time, LerpType lerpType = LerpType.Linear, AnimationCurve curve = null, Reference<float> floatMultiplier = null)
		{
			if (time <= 0f)
			{
				if (local)
				{
					objectTransform.localRotation = newRotation;
				}
				else
				{
					objectTransform.rotation = newRotation;
				}
				yield break;
			}
			float t = 0f;
			Quaternion initialRotation = local ? objectTransform.localRotation : objectTransform.rotation;
			while (t < time)
			{
				t = Mathf.Min(time, t + Time.deltaTime * ((floatMultiplier != null) ? floatMultiplier.Value : 1f));
				Quaternion quaternion;
				if (lerpType != LerpType.Linear)
				{
					if (lerpType != LerpType.Spherical)
					{
						throw new ArgumentOutOfRangeException("lerpType", lerpType, null);
					}
					quaternion = Quaternion.Slerp(initialRotation, newRotation, (curve != null) ? curve.Evaluate(t / time) : (t / time));
				}
				else
				{
					quaternion = Quaternion.Lerp(initialRotation, newRotation, (curve != null) ? curve.Evaluate(t / time) : (t / time));
				}
				if (local)
				{
					objectTransform.localRotation = quaternion;
				}
				else
				{
					objectTransform.rotation = quaternion;
				}
				yield return null;
			}
			if (local)
			{
				objectTransform.localRotation = newRotation;
			}
			else
			{
				objectTransform.rotation = newRotation;
			}
			yield break;
		}

		private static IEnumerator RotateAroundRoutine(Transform tr, Vector3 pos, Vector3 axis, float angle, float time)
		{
			float currentAngle = 0f;
			float degPerSecond = angle / time;
			while (currentAngle < angle)
			{
				float num = degPerSecond * Time.deltaTime;
				currentAngle = Mathf.Min(angle, currentAngle + num);
				tr.RotateAround(pos, axis, num);
				yield return null;
			}
			yield break;
		}

		public static void SetScaleX([NotNull] this Transform t, float x)
		{
			t.localScale = new Vector3(x, t.localScale.y, t.localScale.z);
		}

		public static void SetScaleY([NotNull] this Transform t, float y)
		{
            t.localScale = new Vector3(t.localScale.x, y, t.localScale.z);
		}

		public static void SetScaleZ([NotNull] this Transform t, float z)
		{
            t.localScale = new Vector3(t.localScale.x, t.localScale.y, z);
		}

		public static void SetPositionX([NotNull] this Transform t, float x)
		{
            t.position = new Vector3(x, t.position.y, t.position.z);
		}

		public static void SetPositionY([NotNull] this Transform t, float y)
		{
            t.position = new Vector3(t.position.x, y, t.position.z);
		}

		public static void SetPositionZ([NotNull] this Transform t, float z)
		{
            t.position = new Vector3(t.position.x, t.position.y, z);
		}

		public static void SetLocalPositionX([NotNull] this Transform t, float x)
		{
            t.localPosition = new Vector3(x, t.localPosition.y, t.localPosition.z);
		}

		public static void SetLocalPositionY([NotNull] this Transform t, float y)
		{
            t.localPosition = new Vector3(t.localPosition.x, y, t.localPosition.z);
		}

		public static void SetLocalPositionZ([NotNull] this Transform t, float z)
		{
            t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, z);
		}

		private static List<ActiveRoutine> currentActiveRoutines;

		public enum LerpType
		{
			Linear,
			Spherical
		}

		private enum CoroutineType
		{
			Position,
			PositionX,
			PositionY,
			PositionZ,
			Rotation,
			Scale
		}

		private class ActiveRoutine
		{
			public Coroutine Coroutine;

			public Transform Transform;

			public CoroutineType CoroutineType;
		}
	}
}
