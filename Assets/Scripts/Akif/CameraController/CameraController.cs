using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed = 0.125f;
    void Start()
    {
        
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothSpeed);
        transform.LookAt(target);
    }
}
