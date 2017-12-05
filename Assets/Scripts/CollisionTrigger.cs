using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour {

    // Use this for initialization
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("On Trigger Enter");
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("On Trigger Stay");
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("On Trigger Exit");
    }
}
