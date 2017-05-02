using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ImageBlur : MonoBehaviour {

	private Material material;

	void Awake () {
		material = new Material(Shader.Find("Hidden/BlurShader"));
	}

	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		Graphics.Blit (source, destination, material);
	}
}