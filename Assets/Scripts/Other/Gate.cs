using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public bool isOpened = false;
    private Animator animator;
    public List<Guard> guards = new List<Guard>();

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Guard guard = other.gameObject.GetComponent<Guard>();
        if(guard && !guards.Contains(guard))
        {
            guards.Add(guard);
        }
        else guards.Remove(guard);
    }

    public void Open()
    {
        animator.SetBool("Closing", false);
        animator.SetBool("Opening", true);
        isOpened = true;
    }

    public void Close()
    {
        animator.SetBool("Opening", false);
        animator.SetBool("Closing", true);
        isOpened = false;
    }
}
