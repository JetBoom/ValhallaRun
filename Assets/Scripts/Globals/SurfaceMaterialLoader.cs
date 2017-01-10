using UnityEngine;
using System.Collections.Generic;

public class SurfaceMaterialLoader : MonoBehaviour
{
	public PhysicMaterial[] physicsMaterials;
	public SurfaceMaterialType[] surfaceMaterials;

	void Awake()
	{
		for (int i = 0; i < physicsMaterials.Length; i++)
			SurfaceMaterial.add(physicsMaterials[i], surfaceMaterials[i]);
	}
}
