﻿using UnityEngine;
using System;
using System.Collections;

[ExecuteInEditMode]
public class ImageEffectShockwave : MonoBehaviour
{
    [SerializeField]
    protected Shader currentShader;

    public Vector3 origin = Vector3.zero;

	public float actionnessTransitionTime = 0.5f;

    [Range(0.0f, 1.0f)]
    public float progress = 0f;

	private float actionness = 0.0f;

    private Camera cam;

    private Material currentMaterial;
    public Material material
    {
        get
        {
            if (currentMaterial == null)
            {
                currentMaterial = new Material(currentShader);
                currentMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return currentMaterial;
        }
    }

	public bool actionMode {
		get {
			return actionness > 0;
		}

		set {
			LeanTween.value (value ? 0.0f : 1.0f, value ? 1.0f : 0.0f, actionnessTransitionTime).setOnUpdate (f => actionness = f);
		}
	}

    void Start()
    {
        if (!SystemInfo.supportsImageEffects)
            enabled = false;
        else if (!currentShader && !currentShader.isSupported)
            enabled = false;

        cam = GetComponent<Camera>();
        if(cam == null)
        {
            Debug.LogError("No camera component found!");
        }
    }

    public void Shock(Vector3 origin)
    {
        this.origin = origin;
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Shockwave");
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (currentShader != null)
        {
            material.SetVector("_Origin", origin);
            material.SetFloat("_Progress", progress);
			material.SetFloat("_Actionness", actionness);
            //Matrix4x4 viewProjInverse = (cam.projectionMatrix * cam.worldToCameraMatrix).inverse;
            Matrix4x4 viewProjInverse = cam.cameraToWorldMatrix;
            material.SetMatrix("_ViewProjectInverse", viewProjInverse);
            Graphics.Blit(source, destination, material);
        }
        else
            Graphics.Blit(source, destination);
    }

    private void OnDisable()
    {
        if (currentMaterial)
            DestroyImmediate(currentMaterial);
    }
}