using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image frontBar;
    [SerializeField] Image backBar;

    [SerializeField] float delay = 0.5f;
    [SerializeField] float smoothSpeed = 1f;

    public bool lookAtCamera = true;
    public Transform target;

    float currentHealth;
    float maxHealth;
    float lerpTimer;

    private void Start()
    {
        if (lookAtCamera)
        {
            target = Camera.main.transform;

        }
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = value;
    }

    public void SetHealth(float current)
    {
        currentHealth = current;
        lerpTimer = 0f;

        float fillAmount = currentHealth / maxHealth;
        frontBar.fillAmount = fillAmount;
    }

    void Update()
    {
        if (lookAtCamera)
        {
            transform.LookAt(target.position);

        }

        lerpTimer += Time.deltaTime;

        if (lerpTimer >= delay)
        {
            if (backBar.fillAmount > frontBar.fillAmount)
            {
                backBar.fillAmount = Mathf.Lerp(backBar.fillAmount, frontBar.fillAmount, smoothSpeed);
            }
            else if (backBar.fillAmount < frontBar.fillAmount)
            {
                backBar.fillAmount = frontBar.fillAmount;
            }
        }
    }
}