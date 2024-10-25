using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private Slider healthSlider;

    [Header("UI & Effects")]
    [SerializeField] private Image healthUI;
    [SerializeField] private GameObject godModeSymbol;

    private int currentHealth;
    private float godModeTimer;
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }

    [Header("Damage & Invincibility")]
    [Tooltip("Time duration for invincibility after taking damage")]
    [SerializeField] private float invincibilityDuration = 1f;
    private float invincibilityTimer;
    private SpriteRenderer spriteRenderer;

    [Header("Death Effect")]
    [SerializeField] private GameObject explodeEffectPrefab;
    private bool hasExploded;
    private Collider2D playerCollider;


    void Start()
    {
        // Initialize health and components
        currentHealth = ConstValue.maxHealth;
        //healthSlider.maxValue = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        healthSlider = GetComponent<Slider>();

        godModeTimer = 0f;
        invincibilityTimer = 0f;
        hasExploded = false;
    }

    void Update()
    {
        healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth, 10f * Time.deltaTime);
        currentHealth = Mathf.Clamp(currentHealth, 0, ConstValue.maxHealth);

        HandleGodMode();
        HandleInvincibilityEffect();
        HandleFallToDeath();

        if (currentHealth <= 0 && !hasExploded)
        {
            HandleDeath();
        }
    }

    private void HandleGodMode()
    {
        godModeTimer -= Time.deltaTime;

        if (godModeTimer <= 0)
        {
            godModeSymbol.SetActive(false);
        }
        else
        {
            godModeSymbol.SetActive(true);
        }
    }

    private void HandleInvincibilityEffect()
    {
        invincibilityTimer -= Time.deltaTime;

        spriteRenderer.color = invincibilityTimer > 0
            ? new Color(1, 1, 1, 0.5f)
            : new Color(1, 1, 1, 1f);
    }

    private void HandleFallToDeath()
    {
        if (transform.position.y <= -12.5f)
        {
            currentHealth = 0;
        }
    }

    private void HandleDeath()
    {
        hasExploded = true;

        // Play explosion effect
        Instantiate(explodeEffectPrefab, transform.position, Quaternion.identity);

        // Disable player's appearance and collider
        spriteRenderer.enabled = false;
        playerCollider.enabled = false;

        // Show winner and destroy player object after a delay
        Invoke(nameof(ShowWinner), 2f);
        //Destroy(gameObject, 2f);
    }

    public void IncreaseHp(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, ConstValue.maxHealth);
    }

    public void ActivateGodMode(int duration)
    {
        godModeTimer = duration;
    }

    public void TakeDamage(int damage)
    {
        if (invincibilityTimer <= 0 && godModeTimer <= 0)
        {
            currentHealth -= damage;
            invincibilityTimer = invincibilityDuration;
        }
    }

    private void ShowWinner()
    {
        Debug.Log(GameController.Instance == null);
        GameController.Instance.SetupGameResult();
    }
}
