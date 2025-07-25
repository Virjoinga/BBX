using Zenject;

namespace BSCore
{
	public class DataStoreInstaller : Installer<DataStoreInstaller>
	{
		public override void InstallBindings()
		{
			DataStoreFloat instance = new DataStoreFloat(DataStoreKeys.MouseSensitivity.ToString(), 1f);
			base.Container.BindInstance(instance).WithId(DataStoreKeys.MouseSensitivity).AsCached();
			DataStoreFloat instance2 = new DataStoreFloat(DataStoreKeys.ZoomSensitivity.ToString(), 0.5f);
			base.Container.BindInstance(instance2).WithId(DataStoreKeys.ZoomSensitivity).AsCached();
			DataStoreFloat instance3 = new DataStoreFloat(DataStoreKeys.CameraOffsetX.ToString(), 0.5f);
			base.Container.BindInstance(instance3).WithId(DataStoreKeys.CameraOffsetX).AsCached();
			DataStoreFloat instance4 = new DataStoreFloat(DataStoreKeys.CameraOffsetZ.ToString(), 5f);
			base.Container.BindInstance(instance4).WithId(DataStoreKeys.CameraOffsetZ).AsCached();
			DataStoreFloat instance5 = new DataStoreFloat(DataStoreKeys.FieldOfView.ToString(), 40f);
			base.Container.BindInstance(instance5).WithId(DataStoreKeys.FieldOfView).AsCached();
			DataStoreBool dataStoreBool = new DataStoreBool(DataStoreKeys.YInversion.ToString(), defaultValue: false);
			base.Container.BindInstance(dataStoreBool).WithId(DataStoreKeys.YInversion).AsCached();
			BSCoreInput.RegisterAxisInversion(Option.CameraVertical, dataStoreBool);
			base.Container.BindInstance(new DataStoreFloat(DataStoreKeys.MasterVolume.ToString(), 1f)).WithId(DataStoreKeys.MasterVolume).AsCached();
			base.Container.BindInstance(new DataStoreFloat(DataStoreKeys.SFXVolume.ToString(), 1f)).WithId(DataStoreKeys.SFXVolume).AsCached();
			base.Container.BindInstance(new DataStoreFloat(DataStoreKeys.MusicVolume.ToString(), 0.7f)).WithId(DataStoreKeys.MusicVolume).AsCached();
			base.Container.BindInstance(new DataStoreFloat(DataStoreKeys.UIVolume.ToString(), 0.7f)).WithId(DataStoreKeys.UIVolume).AsCached();
			base.Container.BindInstance(new DataStoreFloat(DataStoreKeys.VoiceVolume.ToString(), 1f)).WithId(DataStoreKeys.VoiceVolume).AsCached();
			base.Container.BindInstance(new DataStoreInt(DataStoreKeys.Vsync.ToString(), 0)).WithId(DataStoreKeys.Vsync).AsCached();
			base.Container.BindInstance(new DataStoreInt(DataStoreKeys.Antialiasing.ToString(), 2)).WithId(DataStoreKeys.Antialiasing).AsCached();
			base.Container.BindInstance(new DataStoreInt(DataStoreKeys.Textures.ToString(), 2)).WithId(DataStoreKeys.Textures).AsCached();
			base.Container.BindInstance(new DataStoreInt(DataStoreKeys.Shadows.ToString(), 3)).WithId(DataStoreKeys.Shadows).AsCached();
			base.Container.BindInstance(new DataStoreInt(DataStoreKeys.Anisotropic.ToString(), 0)).WithId(DataStoreKeys.Anisotropic).AsCached();
			base.Container.BindInstance(new DataStoreFloat(DataStoreKeys.FPSCap.ToString(), 60f)).WithId(DataStoreKeys.FPSCap).AsCached();
			base.Container.BindInstance(new DataStoreString(DataStoreKeys.chatChannel.ToString(), "global")).WithId(DataStoreKeys.FPSCap).AsCached();
			base.Container.BindInstance(new DataStoreBool(DataStoreKeys.SpawnPickerOpen.ToString(), defaultValue: true)).WithId(DataStoreKeys.SpawnPickerOpen).AsCached();
			base.Container.BindInstance(new DataStoreString(DataStoreKeys.HUDCustomization.ToString(), "{}")).WithId(DataStoreKeys.HUDCustomization).AsCached();
			base.Container.Bind<DataStoreManager>().AsSingle();
		}
	}
}
