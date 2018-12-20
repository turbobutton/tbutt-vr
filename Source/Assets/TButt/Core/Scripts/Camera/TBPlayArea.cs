using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent (typeof (MeshRenderer)), RequireComponent (typeof (MeshFilter))]
public class TBPlayArea : MonoBehaviour
{
	public Size size;
	public Color color = new Color (1f, 0.78f, 0.3f);

	public float heightOffset = 0f;
	public float edgeHeight = 0.5f;
	public bool drawInGame = true;

	[HideInInspector]
	public Vector3[] vertices;
	[HideInInspector]
	public Vector3[] edgeVertices;

	[HideInInspector]
	public float width;
	[HideInInspector]
	public float depth;

	public void OnEnable ()
	{
		if (Application.isPlaying)
		{
			GetComponent<MeshRenderer> ().enabled = drawInGame;

			// No need to remain enabled at runtime.
			// Anyone that wants to change properties at runtime
			// should call BuildMesh themselves.
			enabled = false;
		}
	}

#if UNITY_EDITOR
	Hashtable values;
	void Update ()
	{
		if (!Application.isPlaying)
		{
			var fields = GetType ().GetFields (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

			bool rebuild = false;

			if (values == null || (GetComponent<MeshFilter> ().sharedMesh == null))
			{
				rebuild = true;
			}
			else
			{
				foreach (var f in fields)
				{
					if (!values.Contains (f) || !f.GetValue (this).Equals (values[f]))
					{
						rebuild = true;
						break;
					}
				}
			}

			if (rebuild)
			{
				BuildMesh ();

				values = new Hashtable ();
				foreach (var f in fields)
					values[f] = f.GetValue (this);
			}
		}
	}
#endif

	void BuildMesh ()
	{
		Mesh ground = BuildGround ();
		Mesh edges = BuildEdges ();

		CombineInstance[] combine = new CombineInstance[2];
		combine[0].mesh = ground;
		combine[0].transform = Matrix4x4.identity;
		combine[1].mesh = edges;
		combine[1].transform = Matrix4x4.identity;

		MeshFilter mf = GetComponent<MeshFilter> ();
		mf.mesh = new Mesh ();
		mf.sharedMesh.CombineMeshes (combine, false);

		Renderer renderer = GetComponent<MeshRenderer> ();
		Material groundMat = new Material (Shader.Find ("Hidden/TB Play Area Ground"));
		groundMat.SetTexture ("_MainTex", Resources.Load ("TBGrid") as Texture);
		groundMat.SetTexture ("_MaskTex", Resources.Load ("TBGrid_Mask") as Texture);
		//renderer.sharedMaterials = new Material[2];
		//renderer.sharedMaterials[0] = groundMat;
		Material edgeMat = new Material (Shader.Find ("Hidden/TB Play Area Edges"));
		//renderer.sharedMaterials[1] = edgeMat;
		renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		renderer.receiveShadows = false;
		renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;

		Material[] mats = new Material[] { groundMat, edgeMat };
		renderer.materials = mats;
	}

	Mesh BuildGround ()
	{
		Quad quad = new Quad ();

		if (!GetBounds (size, ref quad))
			return null;

		vertices = new Vector3[] { quad.corner0, quad.corner1, quad.corner2, quad.corner3 };

		int[] triangles = new int[]
		{
			2, 1, 0,
			1, 2, 3
		};

		Vector2[] uv = new Vector2[]
		{
			new Vector2 (0f, 0f),
			new Vector2 (width, 0f),
			new Vector2 (0f, depth),
			new Vector2 (width, depth)
		};

		var colors = new Color[]
		{
			color,
			color,
			color,
			color
		};

		Mesh mesh = new Mesh ();
		//GetComponent<MeshFilter> ().mesh = mesh;
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.triangles = triangles;

		return mesh;
	}

	Mesh BuildEdges ()
	{
		if (Mathf.Approximately (edgeHeight, 0f))
			return null;

		Quad quad = new Quad ();

		if (!GetBounds (size, ref quad))
			return null;

		//var corners = new HmdVector3_t[] { rect.vCorners0, rect.vCorners1, rect.vCorners2, rect.vCorners3 };
		//Vector3[] corners = new Vector3[] { quad.corner0, quad.corner1, quad.corner2, quad.corner3 };
		Vector3[] corners = new Vector3[] { quad.corner0, quad.corner1, quad.corner2, quad.corner3 };

		edgeVertices = new Vector3[corners.Length * 2];
		for (int i = 0; i < corners.Length; i++)
		{
			var c = corners[i];
			edgeVertices[i] = new Vector3 (c.x, heightOffset, c.z);
			edgeVertices[i + 4] = new Vector3 (c.x, heightOffset + edgeHeight, c.z);
		}

		var triangles = new int[]
		{
			1, 4, 0,
			5, 4, 1,
			3, 5, 1,
			3, 7, 5,
			6, 7, 3,
			3, 2, 6,
			4, 2, 0,
			2, 4, 6
		};

		var uv = new Vector2[]
		{
			new Vector2(0.0f, 0.0f),
			new Vector2(1.0f, 0.0f),
			new Vector2(0.0f, 0.0f),
			new Vector2(1.0f, 0.0f),
			new Vector2(0.0f, 1.0f),
			new Vector2(1.0f, 1.0f),
			new Vector2(0.0f, 1.0f),
			new Vector2(1.0f, 1.0f)
		};

		var colors = new Color[]
		{
			color,
			color,
			color,
			color,
			new Color(color.r, color.g, color.b, 0.0f),
			new Color(color.r, color.g, color.b, 0.0f),
			new Color(color.r, color.g, color.b, 0.0f),
			new Color(color.r, color.g, color.b, 0.0f)
		};

		Mesh mesh = new Mesh ();
		//GetComponent<MeshFilter> ().mesh = mesh;
		mesh.vertices = edgeVertices;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.triangles = triangles;

		return mesh;
	}

	bool GetBounds (Size size, ref Quad quad)
	{
		try
		{
			var str = size.ToString ().Substring (1);
			var arr = str.Split (new char[] { 'x' }, 2);

			// convert to half size in meters (from cm)
			var x = float.Parse (arr[0]) / 200;
			var z = float.Parse (arr[1]) / 200;

			width = x * 2f;
			depth = z * 2f;

			quad.corner0 = new Vector3 (-x, heightOffset, -z);
			quad.corner1 = new Vector3 (x, heightOffset, -z);
			quad.corner2 = new Vector3 (-x, heightOffset, z);
			quad.corner3 = new Vector3 (x, heightOffset, z);

			return true;
		}
		catch { }

		return false;
	}

	public struct Quad
	{
		public Vector3 corner0;
		public Vector3 corner1;
		public Vector3 corner2;
		public Vector3 corner3;
	}

	public enum Size
	{
		_200x150,
		_300x225,
		_400x300
	}
}
