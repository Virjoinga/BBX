using Bolt;
using UnityEngine;

[ExecuteInEditMode]
public class BoltAOI : EntityBehaviour
{
	[SerializeField]
	public float detectRadius = 32f;

	[SerializeField]
	public float releaseRadius = 64f;

	[SerializeField]
	public int updateRate = 30;

	private void Update()
	{
		Graphics.DrawMesh(BoltPOI.Mesh, Matrix4x4.TRS(base.transform.position, Quaternion.identity, new Vector3(detectRadius, detectRadius, detectRadius)), BoltPOI.MaterialAOIDetect, 0);
		Graphics.DrawMesh(BoltPOI.Mesh, Matrix4x4.TRS(base.transform.position, Quaternion.identity, new Vector3(releaseRadius, releaseRadius, releaseRadius)), BoltPOI.MaterialAOIRelease, 0);
	}

	public override void SimulateOwner()
	{
		if (BoltNetwork.Frame % 30 == 0 && BoltNetwork.ScopeMode == ScopeMode.Manual && base.enabled && base.entity.controller != null)
		{
			BoltPOI.UpdateScope(this, base.entity.controller);
		}
	}
}
