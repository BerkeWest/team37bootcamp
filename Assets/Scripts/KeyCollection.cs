using System.Collections;
using UnityEngine;

public class KeyCollection : MonoBehaviour
{
    [SerializeField] private GameObject key;
    [SerializeField] private GameObject door;
    [SerializeField] private float openAngle = 90f;
    private bool hasKey = false;
    private bool doorOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == key)
        {
            hasKey = true;
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("DoorTrigger") && hasKey && !doorOpened)
        {
            Vector3 currentRotation = door.transform.eulerAngles;
            door.transform.eulerAngles = new Vector3(currentRotation.x, openAngle, currentRotation.z);

            doorOpened = true;
            hasKey = false;
        }
    }
}

