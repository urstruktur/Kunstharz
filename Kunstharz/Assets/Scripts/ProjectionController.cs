using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionController : MonoBehaviour {

    Matrix4x4 orthographic;
    Matrix4x4 perspective;

    // make public for testing
    [Range(0.0f, 1.0f)]
    private float percentage = 1f;
    private float percentageOld = 1f;

    public bool lerp = false;

    private Camera cam;

    void Start () {
        cam = GetComponent<Camera>();

        // initialize projection matrices
        orthographic = Matrix4x4.Ortho(-cam.orthographicSize * cam.aspect, cam.orthographicSize * cam.aspect, 
            -cam.orthographicSize, cam.orthographicSize, -15, cam.farClipPlane);
        perspective = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, cam.nearClipPlane, cam.farClipPlane);
    }
	
	void Update () {
        if (lerp)
        {
            if (cam.orthographic)
            {
                // fly in
                LeanTween.value(gameObject, matrixLerpCallback, 0f, 1f, 3f).setEase(LeanTweenType.easeInExpo);
            }
            else
            {
                // fly out
                LeanTween.value(gameObject, matrixLerpCallback, 1f, 0f, 3f).setEase(LeanTweenType.easeOutExpo);
            }
            
            lerp = false;
        }

        if(percentage != percentageOld)
        {
            cam.projectionMatrix = MatrixLerp(orthographic, perspective, percentage);
            if(percentage == 0)
            {
                cam.orthographic = true;
            }
        }
        percentageOld = percentage;
	}

    public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float amount)
    {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(from[i], to[i], amount);
        return ret;
    }

    void matrixLerpCallback(float val)
    {
        cam.projectionMatrix = MatrixLerp(orthographic, perspective, val);
        if(val == 0)
        {
            cam.orthographic = true;
        }
        if(val == 1)
        {
            cam.orthographic = false;
        }
    }
}
