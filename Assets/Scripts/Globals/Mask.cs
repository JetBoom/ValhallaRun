public static class Mask
{
	public const int
		WORLD = (1 << Layers.WORLD) | (1 << Layers.WORLD2),
		SOLID = (1 << Layers.WORLD) | (1 << Layers.WORLD2) | (1 << Layers.SOLID) | (1 << Layers.LIVING),
		PLAYERSOLID = (1 << Layers.WORLD) | (1 << Layers.WORLD2) | (1 << Layers.SOLID) | (1 << Layers.LIVING) | (1 << Layers.BLOCKLIVING),
		PROJECTILE = (1 << Layers.WORLD) | (1 << Layers.WORLD2) | (1 << Layers.SOLID) | (1 << Layers.LIVING) | (1 << Layers.BLOCKPROJECTILE),
		TEMP = (1 << Layers.TEMP);
}
