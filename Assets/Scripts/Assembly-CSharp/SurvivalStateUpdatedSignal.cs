public struct SurvivalStateUpdatedSignal
{
	public int enemiesToKill;

	public int wave;

	public SurvivalStateUpdatedSignal(int enemiesToKill, int wave)
	{
		this.enemiesToKill = enemiesToKill;
		this.wave = wave;
	}
}
