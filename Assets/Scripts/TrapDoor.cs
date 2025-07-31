using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    [SerializeField] private Animator trapDoorAnimator;

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPlayed)
        {
            trapDoorAnimator.Play("wallDisappear");
            hasPlayed = true;
        }
    }
}

