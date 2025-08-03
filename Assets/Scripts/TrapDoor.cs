using System.Collections;
using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    [SerializeField] private GameObject wallToMove; 
    [SerializeField] private float moveDistance = 4.15f; 
    [SerializeField] private float moveDuration = 1f; 
    private bool hasMoved = false;
    private Vector3 wallInitialState;
    
    private void Start()
    {
        if (wallToMove != null)
        {
            wallInitialState = wallToMove.transform.position;
        }
        GameObjectManager.Instance?.allTraps.Add(this);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasMoved)
        {
            hasMoved = true;
            StartCoroutine(MoveWallDown());
        }
    }

    private IEnumerator MoveWallDown()
    {
        Vector3 startPos = wallToMove.transform.position;
        Vector3 endPos = startPos + Vector3.down * moveDistance;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            wallToMove.transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        wallToMove.transform.position = endPos;
    }

    public void TrapDoorReset()
    {
        hasMoved = false;
        wallToMove.transform.position = wallInitialState;
    }
    
}