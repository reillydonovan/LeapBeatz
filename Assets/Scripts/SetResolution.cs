using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetResolution : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Screen.SetResolution(1920, 1080, false);
        Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
