using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TButt
{
	public static class TBDebug
	{
		public static void DrawBounds (Bounds b)
		{
			DrawBounds (b, Color.white, Time.deltaTime);
		}

		public static void DrawBounds (Bounds b, Color c)
		{
			DrawBounds (b, c, Time.deltaTime);
		}

		public static void DrawBounds (Bounds b, float duration)
		{
			DrawBounds (b, Color.white, duration);
		}

		public static void DrawBounds (Bounds bounds, Color color, float duration)
		{
			Vector3 v3FrontTopLeft;
			Vector3 v3FrontTopRight;
			Vector3 v3FrontBottomLeft;
			Vector3 v3FrontBottomRight;
			Vector3 v3BackTopLeft;
			Vector3 v3BackTopRight;
			Vector3 v3BackBottomLeft;
			Vector3 v3BackBottomRight;

			Vector3 v3Center = bounds.center;
			Vector3 v3Extents = bounds.extents;

			v3FrontTopLeft = new Vector3 (v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
			v3FrontTopRight = new Vector3 (v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
			v3FrontBottomLeft = new Vector3 (v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
			v3FrontBottomRight = new Vector3 (v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner
			v3BackTopLeft = new Vector3 (v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
			v3BackTopRight = new Vector3 (v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner
			v3BackBottomLeft = new Vector3 (v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner
			v3BackBottomRight = new Vector3 (v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner

			Debug.DrawLine (v3FrontTopLeft, v3FrontTopRight, color, duration);
			Debug.DrawLine (v3FrontTopRight, v3FrontBottomRight, color, duration);
			Debug.DrawLine (v3FrontBottomRight, v3FrontBottomLeft, color, duration);
			Debug.DrawLine (v3FrontBottomLeft, v3FrontTopLeft, color, duration);

			Debug.DrawLine (v3BackTopLeft, v3BackTopRight, color, duration);
			Debug.DrawLine (v3BackTopRight, v3BackBottomRight, color, duration);
			Debug.DrawLine (v3BackBottomRight, v3BackBottomLeft, color, duration);
			Debug.DrawLine (v3BackBottomLeft, v3BackTopLeft, color, duration);

			Debug.DrawLine (v3FrontTopLeft, v3BackTopLeft, color, duration);
			Debug.DrawLine (v3FrontTopRight, v3BackTopRight, color, duration);
			Debug.DrawLine (v3FrontBottomRight, v3BackBottomRight, color, duration);
			Debug.DrawLine (v3FrontBottomLeft, v3BackBottomLeft, color, duration);
			//}
		}

	}
}