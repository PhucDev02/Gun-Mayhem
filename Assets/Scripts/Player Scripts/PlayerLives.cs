using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLives : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private Slider livesSlider;

    [Header("UI & Effects")]
    [SerializeField] private GameObject godModeSymbol;

    private int currentLives;
    private float godModeTimer;
    public int CurrentLives { get => currentLives; set => currentLives = value; }

    [Header("Damage & Invincibility")]
    [Tooltip("Time duration for invincibility after taking damage")]
    [SerializeField] private float invincibilityDuration = 1f;
    private float invincibilityTimer;
    private SpriteRenderer spriteRenderer;

    [Header("Death Effect")]
    [SerializeField] private GameObject explodeEffectPrefab;


    void Start()
    {
        currentLives = ConstValue.maxLives;
        spriteRenderer = GetComponent<SpriteRenderer>();
        livesSlider = GetComponent<Slider>();
        livesSlider.maxValue = ConstValue.maxLives;

        godModeTimer = 0f;
        invincibilityTimer = 0f;
    }

    void Update()
    {
        livesSlider.value = Mathf.Lerp(livesSlider.value, currentLives, 10f * Time.deltaTime);
        currentLives = Mathf.Clamp(currentLives, 0, ConstValue.maxLives);

        HandleGodMode();
        HandleInvincibilityEffect();
        HandleFallToDeath();
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
            currentLives--;
            if(currentLives <= 0)
            {
                HandleDeath();
                return;
            }
            transform.position = new Vector3(Random.Range(ConstValue.environmentLimitX.x, ConstValue.environmentLimitX.y), 12f);
        }
    }

    private void HandleDeath()
    {
        Invoke(nameof(ShowWinner), 2f);
    }

    public void IncreaseHp(int amount)
    {
        int _lives = (currentLives == ConstValue.maxLives) ? ConstValue.maxLives : currentLives + amount;
        currentLives = Mathf.Clamp(_lives, 0, ConstValue.maxLives);
    }

    public void ActivateGodMode(int duration)
    {
        godModeTimer = duration;
    }

    public void TakeDamage(int damage)
    {
        if (invincibilityTimer <= 0 && godModeTimer <= 0)
        {
            currentLives -= damage;
            invincibilityTimer = invincibilityDuration;
        }
    }

    private void ShowWinner()
    {
        Debug.Log(GameController.Instance == null);
        GameController.Instance.SetupGameResult();
    }
}
