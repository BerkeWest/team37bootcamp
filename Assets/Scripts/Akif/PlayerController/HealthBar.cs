using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image frontBar;
    [SerializeField] Image backBar;

    [SerializeField] float delay = 0.5f;
    [SerializeField] float smoothSpeed = 1f;

    public Transform target;

    float currentHealth;
    float maxHealth;
    float lerpTimer;

    private void Start()
    {
        target = Camera.main.transform;
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
        transform.LookAt(target.position);

        lerpTimer += Time.deltaTime;

        if (lerpTimer >= delay && backBar.fillAmount > frontBar.fillAmount)
        {
            backBar.fillAmount = Mathf.Lerp(backBar.fillAmount, frontBar.fillAmount, smoothSpeed);
        }
    }
}