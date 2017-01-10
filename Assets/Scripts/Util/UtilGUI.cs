using UnityEngine;

public class UtilGUI : MonoBehaviour
{
	private static Texture2D whitetexture = new Texture2D(1, 1);

	// Just draws a solid color without the need for UI components and objects (uses OnGUI instead).
	public static void DrawRectangle(Rect rect, Color color, float alpha = 0.0f)
	{
		color.a = alpha;
		whitetexture.SetPixel(0, 0, color);		
		whitetexture.Apply();
		GUI.DrawTexture(rect, whitetexture);
	}
}
