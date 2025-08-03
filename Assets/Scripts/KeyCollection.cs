using System.Collections;
using UnityEngine;

public class KeyCollection : MonoBehaviour
{
    [SerializeField] private GameObject key;
    [SerializeField] private GameObject door;
    [SerializeField] private float openAngle = 90f;
    private bool hasKey = false;
    private bool doorOpened = false;
    private Vector3 initialDoorRotation;

    private void Start()
    {
        if (door != null)
        {
            initialDoorRotation = door.transform.eulerAngles;
        }
        GameObjectManager.Instance?.allKeys.Add(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == key)
        {
            hasKey = true;
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("DoorTrigger") && hasKey && !doorOpened)
        {
            Vector3 initialState = door.transform.eulerAngles;
            Vector3 currentRotation = door.transform.eulerAngles;
            door.transform.eulerAngles = new Vector3(currentRotation.x, openAngle, currentRotation.z);

            doorOpened = true;
            hasKey = false;
        }
    }

    public void ResetKeyAndDoor()
    {
        hasKey = false;
        key.gameObject.SetActive(true);
        door.transform.eulerAngles = initialDoorRotation;
        doorOpened = false;
    }
}

