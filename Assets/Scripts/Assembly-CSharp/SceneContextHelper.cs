using System;
using UnityEngine;
using Zenject;

public static class SceneContextHelper
{
	private static SceneContext _sceneContext;

	public static T ResolveZenjectBinding<T>()
	{
		GetSceneContextIfNeeded();
		return _sceneContext.Container.Resolve<T>();
	}

	public static void ResolveZenjectBindings(Action<DiContainer> method)
	{
		GetSceneContextIfNeeded();
		method(_sceneContext.Container);
	}

	private static void GetSceneContextIfNeeded()
	{
		if (_sceneContext == null)
		{
			_sceneContext = UnityEngine.Object.FindObjectOfType<SceneContext>();
		}
	}
}
