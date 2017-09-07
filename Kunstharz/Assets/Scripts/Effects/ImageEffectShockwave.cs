using UnityEngine;
using System;
using System.Collections;

[ExecuteInEditMode]
public class ImageEffectShockwave : MonoBehaviour
{
    [SerializeField]
    protected Shader currentShader;

    public Vector3 origin = Vector3.zero;
    public Vector3 origin_enemy = Vector3.zero;

    public float actionnessTransitionTime = 0.5f;

    public bool inverted = false;

    [Range(0.0f, 1.0f)]
    public float progress = 0f;

    [Range(0.0f, 1.0f)]
    public float progressEnemy = 0f;

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

    int tweenId;
    public void InvertImage()
    {
        inverted = true;
        LTDescr invertTween = LeanTween.value(0, 1, 0.5f).setOnUpdate(v => inverted = v < 0.1 ? false : true).setEaseInBounce();
        tweenId = invertTween.id;
        /*LeanTween.delayedCall(0.2f, () => inverted = false).setOnComplete(() => {
            LeanTween.delayedCall(0.2f, () => inverted = true);
        });*/
    }

    public void NormalizeImage()
    {
        LeanTween.cancel(tweenId);
        inverted = false;
    }

    public void Shock(Vector3 origin)
    {
        this.origin = origin;
        LeanTween.value(0, 1, 1f).setOnUpdate(value => progress = value).setEaseOutQuint().setOnComplete(() => progress = 0);
        /*
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Shockwave");
        }
        */
    }

    public void ShockEnemy(Vector3 origin)
    {
        this.origin_enemy = origin;
        LeanTween.value(0, 1, 1f).setOnUpdate(value => progressEnemy = value).setEaseOutQuint();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (currentShader != null)
        {
            material.SetVector("_Origin", origin);
            material.SetVector("_OriginEnemy", origin_enemy);
            material.SetFloat("_Progress", progress);
            material.SetFloat("_ProgressEnemy", progressEnemy);

            if (inverted)
                material.SetFloat("_Invert", 1);
            else
                material.SetFloat("_Invert", 0);

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