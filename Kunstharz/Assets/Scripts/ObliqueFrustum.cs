using UnityEngine;
using System.Collections;

public class ObliqueFrustum : MonoBehaviour
{
    public float horizontal;
    public float vertical;

    public bool resetMatrix = false;
    public bool applyMatrix = false;
    public Matrix4x4 projection;

    private Matrix4x4 defaultMatrix;
    private bool first = true;
 
    void SetObliqueness(float horizObl, float vertObl)
    {
        Matrix4x4 mat = Camera.main.projectionMatrix;
        mat[0, 2] = horizObl;
        mat[1, 2] = vertObl;
        Camera.main.projectionMatrix = mat;
    }

    void OnValidate()
    {
        if (first)
        {
            defaultMatrix = Camera.main.projectionMatrix;
            first = false;
        }
        if (resetMatrix)
        {
            projection = defaultMatrix;
            resetMatrix = false;
        }
        if (applyMatrix)
        {
            Camera.main.projectionMatrix = projection;
        }
        //Camera.main.projectionMatrix = projection;
        //SetObliqueness(horizontal, vertical);
    }
}