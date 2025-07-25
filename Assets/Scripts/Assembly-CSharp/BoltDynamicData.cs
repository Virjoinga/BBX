using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BoltInternal;
using UnityEngine;

internal static class BoltDynamicData
{
	private static readonly string ASSEMBLY_CSHARP = "Assembly-CSharp";

	private static readonly string ASSEMBLY_CSHARP_FIRST = "Assembly-CSharp-firstpass";

	public static void Setup()
	{
		BoltNetworkInternal.DebugDrawer = new UnityDebugDrawer();
		BoltNetworkInternal.UsingUnityPro = true;
		BoltNetworkInternal.GetSceneName = GetSceneName;
		BoltNetworkInternal.GetSceneIndex = GetSceneIndex;
		BoltNetworkInternal.GetGlobalBehaviourTypes = GetGlobalBehaviourTypes;
		BoltNetworkInternal.EnvironmentSetup = BoltNetworkInternal_User.EnvironmentSetup;
		BoltNetworkInternal.EnvironmentReset = BoltNetworkInternal_User.EnvironmentReset;
	}

	private static int GetSceneIndex(string name)
	{
		return BoltScenes_Internal.GetSceneIndex(name);
	}

	private static string GetSceneName(int index)
	{
		return BoltScenes_Internal.GetSceneName(index);
	}

	public static List<STuple<BoltGlobalBehaviourAttribute, Type>> GetGlobalBehaviourTypes()
	{
		Assembly[] array = new Assembly[2]
		{
			GetAssemblyByName(ASSEMBLY_CSHARP),
			GetAssemblyByName(ASSEMBLY_CSHARP_FIRST)
		};
		List<STuple<BoltGlobalBehaviourAttribute, Type>> list = new List<STuple<BoltGlobalBehaviourAttribute, Type>>();
		try
		{
			Assembly[] array2 = array;
			foreach (Assembly assembly in array2)
			{
				if (assembly == null)
				{
					continue;
				}
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (typeof(MonoBehaviour).IsAssignableFrom(type))
					{
						BoltGlobalBehaviourAttribute[] array3 = (BoltGlobalBehaviourAttribute[])type.GetCustomAttributes(typeof(BoltGlobalBehaviourAttribute), inherit: false);
						if (array3.Length == 1)
						{
							list.Add(new STuple<BoltGlobalBehaviourAttribute, Type>(array3[0], type));
						}
					}
				}
			}
		}
		catch (Exception exception)
		{
			BoltLog.Exception(exception);
		}
		return list;
	}

	private static Assembly GetAssemblyByName(string name)
	{
		return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault((Assembly assembly) => assembly.GetName().Name == name);
	}
}
