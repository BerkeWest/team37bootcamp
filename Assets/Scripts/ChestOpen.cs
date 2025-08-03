using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpen : MonoBehaviour
{
    [SerializeField] private Animator chestLidAnimator;
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int itemCount = 3;
    [SerializeField] private float throwDistance = 1.5f;     // How far forward items land
    [SerializeField] private float arcHeight = 1f;           // Max height of the throw
    [SerializeField] private float throwDuration = 0.6f;     // Time to land
    private bool animDone = false;
    private float groundY;

    private void Start()
    {
        groundY = transform.position.y;
        // Eðer GameObjectManager kullanýyorsanýz ve doðru kurulduysa bu satýr aktif kalabilir.
        // Aksi takdirde, null referans hatalarýný önlemek için yorum satýrý yapmanýz önerilir.
        GameObjectManager.Instance?.allChests.Add(this);

    }

    private void OnTriggerEnter(Collider other)
    {
        // "PlayerSword" tag'inin doðru atandýðýndan emin olun.
        if (other.CompareTag("PlayerSword") && !animDone)
        {
            chestLidAnimator.Play("chestOpening");
            animDone = true;
            StartCoroutine(SpawnItemsAfterAnimation());
        }
    }

    private IEnumerator SpawnItemsAfterAnimation()
    {
        yield return new WaitForSeconds(1.5f); // Animasyonun bitmesini bekle

        for (int i = 0; i < itemCount; i++)
        {
            if (itemPrefabs.Count == 0 || spawnPoint == null) // ItemPrefab listesi boþsa veya spawnPoint atanmamýþsa hata vermemesi için kontrol
            {
                Debug.LogWarning("ChestOpen: ItemPrefabs listesi boþ veya Spawn Point atanmamýþ. Item fýrlatýlamýyor.");
                yield break;
            }

            GameObject item = Instantiate(
                itemPrefabs[Random.Range(0, itemPrefabs.Count)],
                spawnPoint.position,
                Quaternion.identity
            );

            // Yeni oluþturulan item'a rigidbody ekli deðilse ve düþmesini istiyorsak, burada ekleyebiliriz.
            // Ancak genellikle item prefab'inin kendisinde Rigidbody olmasý tercih edilir.
            // Örnek:
            // if (item.GetComponent<Rigidbody>() == null)
            // {
            //     Rigidbody rb = item.AddComponent<Rigidbody>();
            //     rb.useGravity = true; // Yerçekimini kullan
            //     rb.isKinematic = false; // Kinematik olmasýn ki fizik etkilensin
            // }


            Vector3 targetOffset = transform.forward * Random.Range(throwDistance * 0.8f, throwDistance * 1.2f)
                                 + transform.right * Random.Range(-0.3f, 0.3f); // Yanlara doðru rastgelelik ekle
            Vector3 targetPosition = spawnPoint.position + targetOffset;

            StartCoroutine(AnimateItemArc(item.transform, spawnPoint.position, targetPosition, arcHeight, throwDuration));
        }
    }

    private IEnumerator AnimateItemArc(Transform item, Vector3 start, Vector3 end, float height, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // YENÝ KONTROL: Eðer item yok edildiyse, bu Coroutine'i güvenli bir þekilde durdur.
            if (item == null)
            {
                //Debug.LogWarning("AnimateItemArc: Item yok edildiði için animasyon durduruldu.");
                yield break; // Coroutine'i sonlandýr
            }

            float t = elapsed / duration;

            // XZ düzleminde interpolasyon yap
            Vector3 flatPosition = Vector3.Lerp(start, end, t);

            // Y ekseni için parabolik bir yay simüle et
            float arcY = Mathf.Lerp(start.y, end.y, t) + height * Mathf.Sin(Mathf.PI * t);

            item.position = new Vector3(flatPosition.x, arcY, flatPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Coroutine tamamlandýðýnda item'ýn hala var olup olmadýðýný kontrol et
        if (item != null)
        {
            // Son pozisyona (yer seviyesine) sabitle
            item.position = end;

            // Animasyon bittikten sonra item'ýn Rigidbody'sini etkinleþtirebiliriz
            // Eðer prefab'te Rigidbody varsa ve isKinematic True ise burada false yapabiliriz.
            Rigidbody itemRb = item.GetComponent<Rigidbody>();
            if (itemRb != null && itemRb.isKinematic)
            {
                itemRb.isKinematic = false; // Yerçekiminin devreye girmesi için
            }
        }
    }

    public void ResetChest()
    {
        if (chestLidAnimator != null)
        {
            chestLidAnimator.Play("Idle", 0, 0f);
        }
        animDone = false;
    }
}