/*
 * TerrainToMeshCollider
 * williammoodhe@gmail.com
 * http://above.average.website
 * 
 * ExportTerrain ripped apart from TerrainObjExporter <http://wiki.unity3d.com/index.php?title=TerrainObjExporter>
 */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TerrainToMeshCollider : MonoBehaviour
{
	public bool saveToAsset;

	public static void ExportTerrain(Terrain terrain, int qualityloss, out int[] triangles, out Vector3[] vertices, bool reverseCoordinateSystem = false)
	{
		TerrainData terraindata = terrain.terrainData;
		int w = terraindata.heightmapWidth;
		int h = terraindata.heightmapHeight;
		Vector3 meshScale = terraindata.size;
		int tRes = (int)Mathf.Pow(2, qualityloss);
		meshScale = new Vector3(meshScale.x / (w - 1) * tRes, meshScale.y, meshScale.z / (h - 1) * tRes);
		float[,] tData = terraindata.GetHeights(0, 0, w, h);

		w = (w - 1) / tRes + 1;
		h = (h - 1) / tRes + 1;
		Vector3[] tVertices = new Vector3[w * h];

		int[] tPolys;

		tPolys = new int[(w - 1) * (h - 1) * 6];

		// Build vertices and UVs
		for (int y = 0; y < h; y++)
		{
			for (int x = 0; x < w; x++)
			{
				Vector3 vertPos = Vector3.Scale(meshScale, new Vector3(-y, tData[x * tRes, y * tRes], x));
				vertPos = Quaternion.AngleAxis(90f, Vector3.up) * vertPos;
				float vx = vertPos.x;
				vertPos.x = vertPos.z;
				vertPos.z = vx;
				tVertices[y * w + x] = vertPos;
			}
		}

		int index = 0;
		int a, b;
		// Build triangle indices: 3 indices into vertex array for each triangle
		for (int y = 0; y < h - 1; y++)
		{
			for (int x = 0; x < w - 1; x++)
			{
				// For each grid cell output two triangles
				a = (y * w) + x;
				b = ((y + 1) * w) + x + 1;

				tPolys[index++] = a;
				tPolys[index++] = b - 1;
				tPolys[index++] = b;

				tPolys[index++] = b;
				tPolys[index++] = a + 1;
				tPolys[index++] = a;
				
			}
		}
#if UNITY_EDITOR
		Debug.Log("terrain tris: " + tPolys.Length);
		Debug.Log("terrain verts: " + tVertices.Length);
#endif
		if (reverseCoordinateSystem)
		{
			System.Array.Reverse(tPolys);
			System.Array.Reverse(tVertices);
		}

		triangles = tPolys;
		vertices = tVertices;
	}

	void Start()
	{
		TerrainCollider tcol = GetComponent<TerrainCollider>();
		if (GetComponent<MeshCollider>() != null)
		{
			if (tcol != null)
				Destroy(tcol);
			Destroy(this);

			return;
		}

		Terrain terrain = GetComponent<Terrain>();
		if (terrain == null)
			return;

		int[] tris;
		Vector3[] verts;
		ExportTerrain(terrain, 0, out tris, out verts, true);

		Mesh mesh = new Mesh();
		mesh.name = "MeshForTerrain" + name;
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.Optimize();

#if UNITY_EDITOR
		if (saveToAsset)
			AssetDatabase.CreateAsset(mesh, "Assets/" + mesh.name + ".asset");
#endif
		
		MeshCollider col = gameObject.AddComponent<MeshCollider>();
		col.sharedMesh = mesh;

		if (tcol != null)
			Destroy(tcol);
		Destroy(this);
	}
}
