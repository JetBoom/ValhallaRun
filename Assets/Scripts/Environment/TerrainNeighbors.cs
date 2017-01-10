using UnityEngine;

public class TerrainNeighbors : MonoBehaviour
{
	public Terrain left;
	public Terrain top;
	public Terrain right;
	public Terrain bottom;

	void Awake()
	{
		Terrain thisterrain = GetComponent<Terrain>();
		if (thisterrain)
			thisterrain.SetNeighbors(left, top, right, bottom);
	}
}
