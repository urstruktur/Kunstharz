using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UpdateShaderVector : MonoBehaviour {

    public GameObject target;

    private Renderer r;
	
    void Start()
    {
        if(target == null)
        {
            Debug.Log("No target for shader vector.");
        }else
        {
            r = this.GetComponent<Renderer>();
        }
    }

    
    void Update () {
		if(target != null)
        {
           for(int i = 0; i < r.sharedMaterials.Length; i++)
            {
                // sharedMaterials to materials and remove [ExecuteInEditMode] if it doesnt work
                r.sharedMaterials[i].SetVector("_Origin", target.transform.position);
            }
        }
	}

    void OnGui()
    {

    }
}
