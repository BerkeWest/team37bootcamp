using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpen : MonoBehaviour
{
    [SerializeField] private Animator chestLidAnimator;
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int itemCount = 3;
    [SerializeField] private float throwDistance = 1.5f;      // How far forward items land
    [SerializeField] private float arcHeight = 1f;            // Max height of the throw
    [SerializeField] private float throwDuration = 0.6f;      // Time to land
    private bool animDone = false;
    private float groundY;

    private void Start()
    {
        groundY = transform.position.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword") && !animDone)
        {
            chestLidAnimator.Play("chestOpening");
            animDone = true;
            StartCoroutine(SpawnItemsAfterAnimation());
        }
    }

    private IEnumerator SpawnItemsAfterAnimation()
    {
        yield return new WaitForSeconds(1.5f); // Wait for animation to end

        for (int i = 0; i < itemCount; i++)
        {
            GameObject item = Instantiate(
                itemPrefabs[Random.Range(0, itemPrefabs.Count)],
                spawnPoint.position,
                Quaternion.identity
            );

            Vector3 targetOffset = transform.forward * Random.Range(throwDistance * 0.8f, throwDistance * 1.2f)
                                 + transform.right * Random.Range(-0.3f, 0.3f); // Add some sideways randomness
            Vector3 targetPosition = spawnPoint.position + targetOffset;

            StartCoroutine(AnimateItemArc(item.transform, spawnPoint.position, targetPosition, arcHeight, throwDuration));
        }
    }

    private IEnumerator AnimateItemArc(Transform item, Vector3 start, Vector3 end, float height, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Interpolate XZ
            Vector3 flatPosition = Vector3.Lerp(start, end, t);

            // Simulate arc using a parabola for Y
            float arcY = Mathf.Lerp(start.y, end.y, t) + height * Mathf.Sin(Mathf.PI * t);

            item.position = new Vector3(flatPosition.x, arcY, flatPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to final position (ground level)
        item.position = end;
    }
}
