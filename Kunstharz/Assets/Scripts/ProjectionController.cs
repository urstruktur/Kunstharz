using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionController : MonoBehaviour {

    Matrix4x4 orthographic;
    Matrix4x4 perspective;

    [Range(0.0f, 1.0f)]
    public float percentage = 0f;
    private float percentageOld = 1f;

    private Camera cam;

    void Start () {
        cam = GetComponent<Camera>();

        // initialize projection matrices
        orthographic = Matrix4x4.Ortho(-cam.orthographicSize * cam.aspect, cam.orthographicSize * cam.aspect, 
            -cam.orthographicSize, cam.orthographicSize, -15, cam.farClipPlane);
        perspective = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, 0, cam.farClipPlane);
    }
	
	void Update () {
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
}
