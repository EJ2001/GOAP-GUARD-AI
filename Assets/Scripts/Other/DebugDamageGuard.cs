using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDamageGuard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;        

            if (Physics.Raycast(transform.position, transform.forward, out hit, 50))
            {
                Guard guard = hit.transform.GetComponentInParent<Guard>();
                if(guard) guard.currentHealth -= 30;
            }
        }
    }
}
