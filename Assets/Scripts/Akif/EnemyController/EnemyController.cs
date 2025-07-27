using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public LayerMask obstacleMask;
    public Transform player;

    [Header("Attack Settings")]
    public GameObject attackCollider; // Hasar verecek trigger
    public Animator animator;

    private NavMeshAgent agent;
    private bool isChasing = false;
    private bool isAttacking = false;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private float currentHealth;
    private bool isDead = false;
    private bool isHit = false;

    private float lastSeenTime = 0f;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        DisableAttackCollider();
        currentHealth = maxHealth;

    }

    private void Update()
    {
        if (isDead || isHit) return;

        if (player == null) return;

        if (isAttacking) return;

        if (!isChasing)
        {
            TryDetectPlayer();
        }
        else
        {
            if (Time.time - lastSeenTime > 3f)
            {
                StopChasing();
                return;
            }
            MoveToPlayer();
            TryStartAttack();
        }
    }

    // -----------------------------------------------
    private void TryDetectPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Raycast ile arada engel var mı kontrol et
        if (!HasLineOfSightToPlayer()) return;

        float distance = Vector3.Distance(transform.position, player.position);
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // 1. Durum: Yüzü dönükse ve görüş hattı varsa → direkt algılar
        if (angle < 60f) // görüş açısı (opsiyonel)
        {
            StartChasing();
        }
        // 2. Durum: Yüzü dönük değil ama oyuncu çok yaklaşmışsa
        else if (distance <= detectionRange)
        {
            RotateTowards(player.position);
            StartChasing();
        }
    }

    private void StartChasing()
    {
        isChasing = true;
        animator.SetBool("isWalking", true);
    }

    private void StopChasing()
    {
        isChasing = false;
        agent.ResetPath();
        animator.SetBool("isWalking", false);
    }

    private void MoveToPlayer()
    {
        agent.SetDestination(player.position);
    }

    private void TryStartAttack()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private System.Collections.IEnumerator HandleHit()
    {
        isHit = true;
        agent.isStopped = true;
        animator.SetTrigger("getHit");

        yield return new WaitForSeconds(0.4f); // "getHit" animasyonu süresi kadar

        agent.isStopped = false;

        yield return new WaitForSeconds(0.6f);
        isHit = false;
    }


    private System.Collections.IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("attack");
        agent.isStopped = true;

        yield return new WaitForSeconds(0.3f); // animasyon süresine göre ayarla
        EnableAttackCollider();

        yield return new WaitForSeconds(0.2f); // hasar anı
        DisableAttackCollider();

        agent.isStopped = false;

        yield return new WaitForSeconds(1f); // animasyon sonrası gecikme
        isAttacking = false;
    }

    // -----------------------------------------------
    private bool HasLineOfSightToPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        Vector3 origin = transform.position + Vector3.up * 1.5f; // göz hizası
        Vector3 direction = (player.position - origin).normalized;
        float distance = Vector3.Distance(origin, player.position);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            bool seen = hit.transform == player;
            if (seen)
            {
                lastSeenTime = Time.time;
            }
            return seen;
        }
        return false;
    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;
        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 7.5f * Time.deltaTime);
    }

    private void EnableAttackCollider() => attackCollider.SetActive(true);
    private void DisableAttackCollider() => attackCollider.SetActive(false);

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HandleHit());
        }
    }

    private void Die()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetTrigger("die");
        GetComponent<Collider>().enabled = false;
        this.enabled = false; // Opsiyonel: script'i durdurur
    }


    // -----------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && attackCollider.activeSelf)
        {
            Debug.Log("Player'a hasar verildi!");

        }
        if (other.CompareTag("PlayerSword"))
        {
            Debug.Log("Enemy hasar aldı!");

            TakeDamage(other.GetComponentInParent<PlayerControllerS>().attackDmg);
        }
    }
}
