using System;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using UnityEngine;

public class WeaponValidator
{
	public struct FireHistory
	{
		public int ServerFrame;

		public WeaponProfile WeaponProfile;

		public FireHistory(int serverFrame, WeaponProfile weaponProfile)
		{
			ServerFrame = serverFrame;
			WeaponProfile = weaponProfile;
		}
	}

	private readonly BoltEntity _self;

	private readonly BoltHitboxBody _hitboxBody;

	private readonly ProfileManager _profileManager;

	private readonly Action<HitInfo> _onValidHit;

	private readonly Action<WeaponProfile, IPlayerHitCommandInput> _onShouldSpawnEntity;

	private readonly Dictionary<int, FireHistory> _fireHistory = new Dictionary<int, FireHistory>();

	private string _displayName;

	public WeaponValidator(BoltEntity self, BoltHitboxBody hitboxBody, ProfileManager profileManager, Action<HitInfo> onValidHit, Action<WeaponProfile, IPlayerHitCommandInput> onShouldSpawnEntity)
	{
		_self = self;
		_hitboxBody = hitboxBody;
		_profileManager = profileManager;
		_onValidHit = onValidHit;
		_onShouldSpawnEntity = onShouldSpawnEntity;
	}

	private string TryGetDisplayName()
	{
		if (string.IsNullOrEmpty(_displayName))
		{
			IPlayerState state = _self.GetState<IPlayerState>();
			if (state != null)
			{
				_displayName = state.DisplayName;
			}
		}
		return _displayName;
	}

	public void TrackFireHistory(PlayerInputCommand cmd, WeaponProfile weaponProfile)
	{
		int num = cmd.ServerFrame;
		int num2 = BoltNetwork.ServerFrame - 600;
		if (num < num2)
		{
			num = num2;
		}
		_fireHistory.Add(num, new FireHistory(num, weaponProfile));
	}

	public void ForgetFireHistory(int index)
	{
		if (_fireHistory.ContainsKey(index))
		{
			_fireHistory.Remove(index);
		}
	}

	public bool IsValidHitCommandInput(int serverFrame, IPlayerHitCommandInput input, WeaponStateAtFrame weaponStateAtFrame, int nextMeleeFrame, int framesUntilNextMelee)
	{
		WeaponProfile byId = _profileManager.GetById<WeaponProfile>(input.WeaponId);
		int num = BoltNetwork.ServerFrame - 600;
		if (input.LaunchServerFrame < num)
		{
			input.LaunchServerFrame = num;
		}
		if (input.IsMelee())
		{
			int num2 = nextMeleeFrame - framesUntilNextMelee;
			if (serverFrame < num2 || serverFrame > nextMeleeFrame)
			{
				ServerReporter.TrackValidationFailure(TryGetDisplayName(), byId.Id, ServerValidationFailureReason.MeleeWeaponNotSwinging);
				return false;
			}
		}
		else
		{
			if (weaponStateAtFrame.Profile != byId)
			{
				ServerReporter.TrackValidationFailure(TryGetDisplayName(), byId.Id, ServerValidationFailureReason.IncorrectWeaponActive);
				return false;
			}
			if (weaponStateAtFrame.HitType.IsProjectile() && input.Victim == null && !byId.ShouldSendGroundHitToServer(weaponStateAtFrame.HitType))
			{
				ServerReporter.TrackValidationFailure(TryGetDisplayName(), byId.Id, ServerValidationFailureReason.NoVictimOrGroundHit);
				return false;
			}
			if (!_fireHistory.TryGetValue(input.LaunchServerFrame, out var _))
			{
				ServerReporter.TrackValidationFailure(TryGetDisplayName(), byId.Id, ServerValidationFailureReason.NoFireHistory);
				return false;
			}
			if (input.Victim != null && PathIsBlockedByGround(input, byId))
			{
				ServerReporter.TrackValidationFailure(TryGetDisplayName(), byId.Id, ServerValidationFailureReason.PathBlockedByGround);
				return false;
			}
		}
		return true;
	}

	private bool PathIsBlockedByGround(IPlayerHitCommandInput input, WeaponProfile weaponProfile)
	{
		if (weaponProfile.Projectile.MovementType == Projectile.MovementType.Arcing)
		{
			Vector3 origin = input.Origin;
			Vector3 vector = input.Forward * weaponProfile.Projectile.Speed;
			Vector3 vector2 = Vector3.ProjectOnPlane(vector, Vector3.up);
			float y = vector.y;
			float num = 0f;
			Vector3 start = origin;
			for (int i = input.LaunchServerFrame + 1; i < input.HitServerFrame; i++)
			{
				num += BoltNetwork.FrameDeltaTime;
				Vector3 vector3 = origin + vector2 * num;
				vector3.y = origin.y + y * num + 0.5f * Physics.gravity.y * weaponProfile.Projectile.ArcMultiplier * num * num;
				if (Physics.Linecast(start, vector3, LayerMaskConfig.GroundLayers))
				{
					return true;
				}
				start = vector3;
			}
			return false;
		}
		Vector3 end = input.Point + (input.Point - input.Origin).normalized * 0.2f;
		return Physics.Linecast(input.Origin, end, LayerMaskConfig.GroundLayers);
	}

	public void ProcessValidInput(int serverFrame, IPlayerHitCommandInput input, out BoltEntity hitVictim, WeaponStateAtFrame weaponStateAtFrame, int nextMeleeTime, int meleeCooldownFrames)
	{
		hitVictim = input.Victim;
		WeaponProfile byId = _profileManager.GetById<WeaponProfile>(input.WeaponId);
		if (byId != null && byId.Id == input.WeaponId && byId.Charge.CanCharge)
		{
			input.ChargeTime = Mathf.Clamp(input.ChargeTime, 0f, byId.Charge.Time);
		}
		if (input.IsRaycast() && !byId.Spread.IsAoE)
		{
			Ray ray = new Ray(input.Origin, input.Forward);
			if (TryGetHitBoxesInPath(ray, 500f, input.HitServerFrame, out var hitBoxes))
			{
				Debug.Log($"[WeaponValidator] Direct hit. Can Critical? {byId.Critical.CanCritical}");
				BoltHit boltHit = hitBoxes.First();
				IDamageable damageable = boltHit.Damageable;
				HitInfo obj = input.ToHitInfo();
				obj.collider = damageable.HurtCollider;
				obj.hitEntity = (hitVictim = damageable.entity);
				obj.isHeadshot = boltHit.IncludesHeadshot;
				obj.distance = boltHit.Distance;
				if (byId.Critical.CanCritical && obj.isHeadshot)
				{
					Debug.Log("[WeaponValidator] Critical hit confirmed");
					obj.hitBox = boltHit.HitBoxes.First((BoltHitbox hb) => hb.hitboxType == BoltHitboxType.Head);
				}
				else
				{
					obj.hitBox = boltHit.HitBoxes.First();
				}
				_onValidHit(obj);
			}
			else if (input.Victim != null && input.Victim != _self)
			{
				Debug.LogError("[WeaponValidator] No Raycast Hits in Path");
				ServerReporter.TrackValidationFailure(TryGetDisplayName(), byId.Id, ServerValidationFailureReason.NoRaycastHitsInPath);
			}
		}
		else
		{
			List<HitInfo> hits = GetHits(input, byId);
			if (hits.Count <= 0 && !byId.SpawnedEntity.SpawnsEntity)
			{
				if (input.Victim != null && input.Victim != _self)
				{
					Debug.LogError("[WeaponValidator] No Valid Hits Found");
					ServerReporter.TrackValidationFailure(TryGetDisplayName(), byId.Id, ServerValidationFailureReason.NoValidHitsFound);
				}
				return;
			}
			ProcessHits(input, hits, byId);
		}
		if (byId.SpawnedEntity.SpawnsEntity && (byId.SpawnedEntity.SpawnOnDirectHit || hitVictim == null))
		{
			TrySpawnEntity(hitVictim, byId, input);
		}
	}

	private void TrySpawnEntity(BoltEntity hitVictim, WeaponProfile weaponProfile, IPlayerHitCommandInput input)
	{
		if (hitVictim != null)
		{
			IPlayerState state = hitVictim.GetState<IPlayerState>();
			if (state != null && state.IsShielded)
			{
				return;
			}
		}
		_onShouldSpawnEntity(weaponProfile, input);
	}

	private List<HitInfo> GetHits(IPlayerHitCommandInput input, WeaponProfile weaponProfile)
	{
		HitInfo hitInfo = input.ToHitInfo();
		hitInfo.weaponProfile = weaponProfile;
		if (weaponProfile.Explosion.Explodes)
		{
			List<HitInfo> list = AreaEffectCalculator.GetAffectableInRangeServer(weaponProfile.CalculateRangeAtTime(input.ChargeTime), hitInfo, input.HitServerFrame);
			if (weaponProfile.Explosion.IgnoreSelf && list.Count > 0)
			{
				list = list.Where(delegate(HitInfo areaHit)
				{
					IPlayerController componentInParent = areaHit.collider.GetComponentInParent<IPlayerController>();
					return componentInParent == null || componentInParent.entity != _self;
				}).ToList();
			}
			if (weaponProfile.Explosion.ClosestOnly && list.Count > 1)
			{
				return new List<HitInfo> { list.OrderBy((HitInfo areahit) => areahit.distance).First() };
			}
			return list;
		}
		if (weaponProfile.Spread.IsAoE)
		{
			hitInfo.point = hitInfo.origin;
			float num = weaponProfile.Range;
			if (weaponProfile.Charge.CanCharge && weaponProfile.Charge.RangeAtMax > 0f)
			{
				num = weaponProfile.CalculateRangeAtTime(hitInfo.chargeTime);
			}
			else if (weaponProfile.Spread.FalloffRange > num)
			{
				num = weaponProfile.Spread.FalloffRange;
			}
			return AreaEffectCalculator.GetAffectableInRangeServer(num, input.Forward, weaponProfile.Spread.Amount, hitInfo, input.HitServerFrame);
		}
		List<HitInfo> list2 = new List<HitInfo>();
		bool flag = PathIsClear(input.Origin, input.Point);
		if (input.Victim != null && input.Victim != _self && flag)
		{
			if (!input.IsRaycast() && !input.IsContinuous())
			{
				list2.Add(hitInfo);
				return list2;
			}
			bool flag2 = false;
			BoltHit boltHit;
			if (input.HitServerFrame == input.LaunchServerFrame || weaponProfile.Projectile.MovementType == Projectile.MovementType.Linear)
			{
				flag2 = TryGetFirstHitInPath(input.Origin, input.Forward, input.HitServerFrame, out boltHit);
			}
			else
			{
				Vector3 initDirection = input.Forward * weaponProfile.Projectile.Speed;
				flag2 = TryGetFirstHitInArcingPath(input.Origin, initDirection, weaponProfile.Projectile.ArcMultiplier, input.LaunchServerFrame, input.HitServerFrame, out boltHit);
			}
			if (flag2)
			{
				hitInfo.isHeadshot = boltHit.IncludesHeadshot;
				hitInfo.distance = boltHit.Distance;
				if (weaponProfile.Critical.CanCritical && hitInfo.isHeadshot)
				{
					Debug.Log("[WeaponValidator] Critical hit confirmed");
					hitInfo.hitBox = boltHit.HitBoxes.First((BoltHitbox hb) => hb.hitboxType == BoltHitboxType.Head);
				}
				else
				{
					hitInfo.hitBox = boltHit.HitBoxes.First();
				}
				list2.Add(hitInfo);
			}
		}
		return list2;
	}

	private bool TryGetFirstHitInPath(Vector3 start, Vector3 direction, int serverFrame, out BoltHit boltHit, float distanceLimit = 500f)
	{
		Ray ray = new Ray(start, direction);
		Debug.DrawRay(start, direction * 100f, Color.blue, 10f);
		if (TryGetHitBoxesInPath(ray, distanceLimit, serverFrame, out var hitBoxes))
		{
			boltHit = hitBoxes.First();
			return true;
		}
		boltHit = default(BoltHit);
		return false;
	}

	private bool TryGetFirstHitInArcingPath(Vector3 initStart, Vector3 initDirection, float arcMultiplier, int launchFrame, int hitFrame, out BoltHit boltHit)
	{
		Vector3 vector = Vector3.ProjectOnPlane(initDirection, Vector3.up);
		float y = initDirection.y;
		float num = 0f;
		Vector3 vector2 = initStart;
		for (int i = launchFrame + 1; i < hitFrame; i++)
		{
			num += BoltNetwork.FrameDeltaTime;
			Vector3 vector3 = initStart + vector * num;
			vector3.y = initStart.y + y * num + 0.5f * Physics.gravity.y * arcMultiplier * num * num;
			Vector3 normalized = (vector3 - vector2).normalized;
			Debug.DrawLine(vector2, vector3, Color.blue, 10f);
			float num2 = Vector3.Distance(vector3, vector2);
			if (TryGetFirstHitInPath(vector2, normalized, i, out boltHit, num2 + 0.1f))
			{
				return true;
			}
			vector2 = vector3;
		}
		boltHit = default(BoltHit);
		return false;
	}

	private void ProcessHits(IPlayerHitCommandInput input, List<HitInfo> hits, WeaponProfile weaponProfile)
	{
		for (int i = 0; i < hits.Count; i++)
		{
			HitInfo obj = hits[i];
			if (!(obj.collider == null) && (!weaponProfile.SpawnedEntity.SpawnsEntity || ((weaponProfile.Damage != 0f || weaponProfile.Explosion.Explodes) && weaponProfile.SpawnedEntity.DoesDamageOnHit)))
			{
				new Ray(obj.origin, obj.forward);
				_onValidHit(obj);
			}
		}
	}

	private static bool PathIsClear(Vector3 point, Vector3 origin)
	{
		return !Physics.Linecast(origin, point, LayerMaskConfig.GroundLayers);
	}

	private bool TryGetClosestVictimHitBoxInPath(BoltEntity victim, Ray ray, float distanceLimit, int serverFrame, out BoltHit closestHitBox)
	{
		return TryGetClosestHitBoxInPath(ray, distanceLimit, serverFrame, out closestHitBox, conditional);
		bool conditional(BoltHitboxBody body, BoltHitbox hitBox)
		{
			IDamageable component = body.GetComponent<IDamageable>();
			if (component != null)
			{
				return component.entity == victim;
			}
			return false;
		}
	}

	private bool TryGetClosestHitBoxInPath(Ray ray, float distanceLimit, int serverFrame, out BoltHit hitBox, Func<BoltHitboxBody, BoltHitbox, bool> conditional = null)
	{
		if (TryGetHitBoxesInPath(ray, distanceLimit, serverFrame, out var hitBoxes, conditional))
		{
			hitBox = hitBoxes.FirstOrDefault();
			return true;
		}
		hitBox = default(BoltHit);
		return false;
	}

	private bool TryGetVictimHitBoxesInPath(BoltEntity victim, Ray ray, float distanceLimit, int serverFrame, out List<BoltHit> hitBoxes)
	{
		return TryGetHitBoxesInPath(ray, distanceLimit, serverFrame, out hitBoxes, conditional);
		bool conditional(BoltHitboxBody body, BoltHitbox hitBox)
		{
			IDamageable component = body.GetComponent<IDamageable>();
			if (component != null)
			{
				return component.entity == victim;
			}
			return false;
		}
	}

	private bool TryGetHitBoxesInPath(Ray ray, float distanceLimit, int serverFrame, out List<BoltHit> hitBoxes, Func<BoltHitboxBody, BoltHitbox, bool> conditional = null)
	{
		Dictionary<BoltHitboxBody, BoltHit> dictionary = new Dictionary<BoltHitboxBody, BoltHit>();
		DebugExtension.DebugPoint(ray.origin, Color.magenta, 0.25f, 10f);
		Debug.DrawLine(ray.origin, ray.GetPoint(distanceLimit), Color.green, 10f);
		using (BoltPhysicsHits boltPhysicsHits = BoltNetwork.RaycastAll(ray, serverFrame))
		{
			Debug.Log($"[WeaponValidator] Found {boltPhysicsHits.count} bolt hitboxes within {distanceLimit} at server frame {serverFrame}");
			for (int i = 0; i < boltPhysicsHits.count; i++)
			{
				BoltPhysicsHit hit = boltPhysicsHits[i];
				DebugExtension.DebugPoint(ray.GetPoint(hit.distance), Color.green, 0.25f, 10f);
				Debug.DrawLine(ray.origin, ray.GetPoint(hit.distance), Color.green, 10f);
				if (hit.hitbox.hitboxType == BoltHitboxType.Proximity || hit.distance > distanceLimit || hit.body == _hitboxBody)
				{
					Debug.Log($"[WeaponValidator] Skipping bolt hitbox: type: {hit.hitbox.hitboxType}, distance: {hit.distance} /{distanceLimit}, is self: {hit.body == _hitboxBody}");
				}
				else if (conditional == null || conditional(hit.body, hit.hitbox))
				{
					if (!dictionary.TryGetValue(hit.body, out var value))
					{
						value = new BoltHit(hit);
						dictionary.Add(hit.body, value);
					}
					else
					{
						value.Add(hit);
					}
				}
				else
				{
					Debug.Log($"[WeaponValidator] {hit.body.name} failed conditional on server frame {serverFrame}");
				}
			}
		}
		hitBoxes = dictionary.Values.OrderBy((BoltHit bh) => bh.Distance).ToList();
		Debug.Log($"[WeaponValidator] Found {hitBoxes.Count} validated hitbox bodies at server frame {serverFrame}");
		return hitBoxes.Count > 0;
	}
}
