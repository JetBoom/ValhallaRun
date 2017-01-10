using UnityEngine;

public static class UtilRandom
{
	// Takes a random object out of an array of variable length.
	public static object RandomFromArray(object[] arr)
	{
		return arr[Random.Range(0, arr.Length)];
	}
}
