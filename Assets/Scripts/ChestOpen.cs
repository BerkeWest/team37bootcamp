using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpen : MonoBehaviour
{
    [SerializeField] private Animator chestLidAnimator; 
    private bool animDone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword") && !animDone)
        {
            chestLidAnimator.Play("chestOpening");
            animDone = true;
        }
    }
}
