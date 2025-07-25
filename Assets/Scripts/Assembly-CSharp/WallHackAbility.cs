using System.Collections.Generic;
using System.Linq;
using Constants;
using UnityEngine;

public class WallHackAbility : MonoBehaviour
{
	private bool _isActive;

	private List<PlayerController> _cachedPlayerControllers;

	private List<PlayerController> GetPlayers()
	{
		if (_cachedPlayerControllers == null)
		{
			_cachedPlayerControllers = Object.FindObjectsOfType<PlayerController>().ToList();
		}
		_cachedPlayerControllers.RemoveAll((PlayerController p) => p == null || p.IsLocal);
		return _cachedPlayerControllers;
	}

	public void Activate()
	{
		foreach (PlayerController player in GetPlayers())
		{
			player.LoadoutController.Outfit.gameObject.SetLayerRecursively(19);
			Renderer[] componentsInChildren = player.LoadoutController.Outfit.GetComponentsInChildren<Renderer>();
			Color value = (player.IsLocalPlayerTeammate ? Match.FriendlyTeamColor : Match.EnemyTeamColor);
			Renderer[] array = componentsInChildren;
			foreach (Renderer obj in array)
			{
				obj.material.SetColor("_TeamColor", value);
				obj.material.SetFloat("_EnableGlow", 1f);
				obj.material.SetFloat("_EnableBehindEffect", 1f);
			}
		}
		_isActive = true;
	}

	public void Deactivate()
	{
		foreach (PlayerController player in GetPlayers())
		{
			player.LoadoutController.Outfit.gameObject.SetLayerRecursively(9);
			Renderer[] componentsInChildren = player.LoadoutController.Outfit.GetComponentsInChildren<Renderer>();
			foreach (Renderer obj in componentsInChildren)
			{
				obj.material.SetFloat("_EnableGlow", 0f);
				obj.material.SetFloat("_EnableBehindEffect", 0f);
			}
		}
		_isActive = false;
	}
}
