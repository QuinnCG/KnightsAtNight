namespace Quinn
{
	public enum StatusEffectType
	{
		// Take damage every interval.
		Burning,
		// Deal less damage.
		Weakened,
		// Move slower.
		Slowed,
		// Can't move or attack.
		Shocked,
		// Like Burning, but meant to last longer.
		GhostFireBurning,
		// Take less damage.
		Resistance,
		// Deal more damage.
		Strength,
		// Move faster.
		Haste,
		// Same as shocked but with different VFX.
		Frozen
	}
}
