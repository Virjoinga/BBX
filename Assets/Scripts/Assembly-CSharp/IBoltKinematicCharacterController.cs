using KinematicCharacterController;

public interface IBoltKinematicCharacterController : ICharacterController
{
	void AfterPositionRotationUpdate(float deltaTime);
}
