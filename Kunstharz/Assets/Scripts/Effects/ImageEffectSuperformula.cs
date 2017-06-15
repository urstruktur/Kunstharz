using System;
using UnityEngine;

[ExecuteInEditMode]
public class ImageEffectSuperformula : MonoBehaviour
{
    [SerializeField]
    protected Shader currentShader;

    [Range(0.0f, 1.0f)]
    public float intensity = 1f;

    [Range(0.0f, 50.0f)]
    public float m = 3f;

    [Range(0.0f, 50.0f)]
    public float n1 = 5f;

    [Range(0.0f, 50.0f)]
    public float n2 = 18f;

    [Range(0.0f, 50.0f)]
    public float n3 = 18f;

    [Range(0.0f, 50.0f)]
    public float glitch = 0f;

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

    void Start()
    {
        if (!SystemInfo.supportsImageEffects)
            enabled = false;
        else if (!currentShader && !currentShader.isSupported)
            enabled = false;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (currentShader != null)
        {
            material.SetFloat("_Intensity", intensity);
            material.SetFloat("_M", m);
            material.SetFloat("_N1", n1);
            material.SetFloat("_N2", n2);
            material.SetFloat("_N3", n3);
            material.SetFloat("_Glitch", glitch);

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

    public void Shoot()
    {
        Animator animator = GetComponent<Animator>();
        glitch = 1.0f;
        if (animator != null)
        {
            //animator.StopPlayback();
            animator.Play("Lasershot");
            LeanTween.value(this.gameObject, 1f, 0f, 2f).setOnUpdate((float val) => {
                glitch = val;
            }).setEase(LeanTweenType.easeOutCubic);
        }
    }
}