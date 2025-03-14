using System;
using System.Linq;
using UnityEngine;

namespace HurnCheat
{
	// Token: 0x02000004 RID: 4
	internal class Utils
	{
		// Token: 0x06000011 RID: 17 RVA: 0x000040B8 File Offset: 0x000022B8
		public static Vector3 WorldToScreen(Vector3 In)
		{
			Vector3 result = Vector3.zero;
			Vector3 vector;
			vector..ctor(Camera.main.WorldToScreenPoint(In).x, Camera.main.WorldToScreenPoint(In).y, Camera.main.WorldToScreenPoint(In).z);
			if (vector.z > 0.01f)
			{
				result = vector;
			}
			return result;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00004114 File Offset: 0x00002314
		public static byte[] FromHex(string hex)
		{
			hex = hex.Replace("-", "");
			byte[] array = new byte[hex.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
			}
			return array;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00004164 File Offset: 0x00002364
		public static void DrawLabel(Vector3 point, string text, Color c, int fontsize = 15)
		{
			Vector3 vector = Utils.WorldToScreen(point);
			if (vector.z > 0.01f)
			{
				int length = text.Length;
				GUIStyle guistyle = new GUIStyle(GUI.skin.GetStyle("label"));
				guistyle.fontSize = fontsize;
				vector.x -= (float)(text.Length * 2);
				GUI.color = Color.black;
				GUI.Label(new Rect(vector.x - 1f, (float)Screen.height - vector.y, 650f, 450f / (float)(length / 2)), text, guistyle);
				GUI.Label(new Rect(vector.x + 1f, (float)Screen.height - vector.y, 650f, 450f / (float)(length / 2)), text, guistyle);
				GUI.Label(new Rect(vector.x, (float)Screen.height - vector.y + 1f, 650f, 450f / (float)(length / 2)), text, guistyle);
				GUI.Label(new Rect(vector.x, (float)Screen.height - vector.y - 1f, 650f, 450f / (float)(length / 2)), text, guistyle);
				GUI.color = c;
				GUI.Label(new Rect(vector.x, (float)Screen.height - vector.y, 650f, 450f / (float)(length / 2)), text, guistyle);
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000042DC File Offset: 0x000024DC
		public static void DrawLabelS(Rect rScreen, string text, Color c, int fontsize = 16)
		{
			Main main = new Main();
			GUIStyle guistyle = new GUIStyle(GUI.skin.GetStyle("label"));
			guistyle.fontSize = fontsize;
			GUI.color = Color.Lerp(Color.black, Color.white, 0.4f);
			GUI.Label(new Rect(rScreen.xMin - 1f, rScreen.yMin, rScreen.width, rScreen.height), text, guistyle);
			GUI.Label(new Rect(rScreen.xMin + 1f, rScreen.yMin, rScreen.width, rScreen.height), text, guistyle);
			GUI.Label(new Rect(rScreen.xMin, rScreen.yMin + 1f, rScreen.width, rScreen.height), text, guistyle);
			GUI.Label(new Rect(rScreen.xMin, rScreen.yMin - 1f, rScreen.width, rScreen.height), text, guistyle);
			GUI.color = c;
			GUI.Label(new Rect(rScreen.xMin, rScreen.yMin, rScreen.width, rScreen.height), text, guistyle);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000440C File Offset: 0x0000260C
		public static int DistanceToCross(Vector3 WorldPosition)
		{
			Vector3 vector = Utils.WorldToScreen(WorldPosition);
			int num = Screen.width / 2;
			int num2 = Screen.height / 2;
			float num3 = (vector.x > (float)num) ? (vector.x - (float)num) : ((float)num - vector.x);
			float num4 = (vector.y > (float)num) ? (vector.y - (float)num) : ((float)num2 - vector.y);
			return (int)Mathf.Sqrt(num3 * num3 + num4 * num4);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00004484 File Offset: 0x00002684
		public static Vector3 Vec3Set(float x, float y, float z)
		{
			Vector3 zero = Vector3.zero;
			zero.x = x;
			zero.y = y;
			zero.z = z;
			return zero;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000044D4 File Offset: 0x000026D4
		public static string RandomString(int length)
		{
			Random random = new Random();
			return new string((from s in Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
			select s[random.Next(s.Length)]).ToArray<char>());
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00004518 File Offset: 0x00002718
		public static string IntBar(float value)
		{
			return ((int)value).ToString();
		}

		// Token: 0x04000044 RID: 68
		public static byte[] cData = Utils.FromHex("48-75-72-6e-43-68-65-61-74-20-2d-20-34-6c-33-78-78-78");
	}
}
