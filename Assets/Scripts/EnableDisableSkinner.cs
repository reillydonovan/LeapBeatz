using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableSkinner : MonoBehaviour {
    public GameObject SkinnerMeshTarget;
    public GameObject[] SkinnerMeshRenderer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (SkinnerMeshTarget.activeInHierarchy == true)
        {
            for (int i = 0; i < SkinnerMeshRenderer.Length; i++) { 
                SkinnerMeshRenderer[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < SkinnerMeshRenderer.Length; i++) {
                SkinnerMeshRenderer[i].SetActive(false);
            }
        }
		
	}
}
