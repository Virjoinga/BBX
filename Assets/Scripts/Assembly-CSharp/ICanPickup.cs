public interface ICanPickup
{
	bool TryPickup(PickupData pickup, BoltEntity entity);

	bool CanPickup(PickupData pickup);
}
