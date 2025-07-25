using Bolt;

public abstract class SpawnedEntity<TState> : BaseEntityBehaviour<TState>, ISpawnedEntityController where TState : IState
{
	protected WeaponProfile _weaponProfile { get; private set; }

	protected BoltEntity _ownerEntity { get; private set; }

	protected IPlayerState _ownerPlayerState { get; private set; }

	protected bool _ownerIsPlayer { get; private set; }

	public void Setup(WeaponProfile weaponProfile, BoltEntity ownerEntity)
	{
		_weaponProfile = weaponProfile;
		_ownerEntity = ownerEntity;
		if (_ownerEntity.TryFindState<IPlayerState>(out var ownerPlayerState))
		{
			_ownerIsPlayer = true;
			_ownerPlayerState = ownerPlayerState;
		}
	}
}
