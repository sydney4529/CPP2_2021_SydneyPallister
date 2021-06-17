using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachtoParent : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(other.gameObject.GetComponent<ThirdPersonMovement>().isGrounded == true)
            {
                other.gameObject.GetComponent<ThirdPersonMovement>().gravity = 0;
                if (other.gameObject.GetComponent<ThirdPersonMovement>().gravity == 0)
                    other.transform.parent = transform;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.parent = null;
            other.gameObject.GetComponent<ThirdPersonMovement>().gravity = -39.24f;
        }

    }
}
