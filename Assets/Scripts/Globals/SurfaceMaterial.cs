using UnityEngine;
using System.Collections.Generic;

public enum SurfaceMaterialType
{
	None = 0,
	Hard,
	Soft
}

public static class SurfaceMaterial
{
	static Dictionary<PhysicMaterial, SurfaceMaterialType> matToSurfaceType = new Dictionary<PhysicMaterial, SurfaceMaterialType>();
	static Dictionary<int, SurfaceMaterialType> terrainTexIDToSurfaceType = new Dictionary<int, SurfaceMaterialType>()
	{
		{ 0, SurfaceMaterialType.Hard },
		{ 1, SurfaceMaterialType.Hard },
		{ 2, SurfaceMaterialType.Soft }
	};

	public static void add(PhysicMaterial physmat, SurfaceMaterialType mattype)
	{
		if (!matToSurfaceType.ContainsKey(physmat))
			matToSurfaceType.Add(physmat, mattype);
	}

	public static SurfaceMaterialType get(GameObject obj, Vector3 point)
	{
		Collider col = obj.GetComponent<Collider>();
		if (col != null)
			return get(col, point);

		return SurfaceMaterialType.Hard;
	}

	public static SurfaceMaterialType get(Collider col, Vector3 point)
	{
		if (col is TerrainCollider)
		{
			Terrain terrain = col.GetComponent<Terrain>();
			int id = terrain.GetMainTextureIndex(point);
			if (terrainTexIDToSurfaceType.ContainsKey(id))
				return terrainTexIDToSurfaceType[id];

			return SurfaceMaterialType.Hard;
		}

		PhysicMaterial mat = col.material;
		if (matToSurfaceType.ContainsKey(mat))
			return matToSurfaceType[mat];

		return SurfaceMaterialType.Hard;
	}
}
