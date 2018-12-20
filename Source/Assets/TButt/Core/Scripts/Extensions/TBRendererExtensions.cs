using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TBRendererExtensions
{
	public static void SetInstancedProperty (this Renderer r, int propertyID, Color color, ref MaterialPropertyBlock block)
	{
		r.GetPropertyBlock (block);
		block.SetColor (propertyID, color);
		r.SetPropertyBlock (block);
	}

	public static void SetInstancedProperty (this Renderer r, int propertyID, float value, ref MaterialPropertyBlock block)
	{
		r.GetPropertyBlock (block);
		block.SetFloat (propertyID, value);
		r.SetPropertyBlock (block);
	}

	public static void SetInstancedProperty (this Renderer[] renderers, int propertyID, Color color, ref MaterialPropertyBlock block)
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			if (renderers[i] == null)
				continue;

			renderers[i].SetInstancedProperty (propertyID, color, ref block);
		}
	}

	public static void SetInstancedProperty (this Renderer[] renderers, int propertyID, float value, ref MaterialPropertyBlock block)
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			if (renderers[i] == null)
				continue;

			renderers[i].SetInstancedProperty (propertyID, value, ref block);
		}
	}
}
