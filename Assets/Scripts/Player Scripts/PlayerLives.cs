using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLives : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private Slider livesSlider;

    [Header("UI & Effects")]
    [SerializeField] private GameObject invincibleIndicator;

    private int currentLives;
    private bool isInvincible;
    public int CurrentLives { get => currentLives; set => currentLives = value; }
    public bool IsInvincible { get => isInvincible; set => isInvincible = value; }

    [Header("Damage & Invincibility")]
    [Tooltip("Time duration for invincibility after taking damage")]
    [SerializeField] private float invincibilityDuration = 1f;
    private SpriteRenderer spriteRenderer;

    [Header("Death Effect")]
    [SerializeField] private GameObject explodeEffectPrefab;

    private CancellationTokenSource cancellationTokenSource = new();
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerController.Register(this);
    }

    void Start()
    {
        currentLives = ConstValue.maxLives;
        spriteRenderer = GetComponent<SpriteRenderer>();
        livesSlider = GetComponent<Slider>();
        livesSlider.maxValue = ConstValue.maxLives;

        HandleInvincibleForm(false);
        UpdateLives();
    }

    void Update()
    {
        HandleFallToDeath();
    }

    private void HandleFallToDeath()
    {
        if (transform.position.y <= -12.5f)
        {
            UpdateLives(-1);
            if (currentLives <= 0)
            {
                HandleDeath();
                return;
            }
            transform.position = new Vector3(Random.Range(ConstValue.environmentLimitX.x, ConstValue.environmentLimitX.y), 12f);
        }
    }

    private void UpdateLives(int amount = 0)
    {
        currentLives = (currentLives + amount <= 0) ? 0 :
               (currentLives + amount >= ConstValue.maxLives) ? ConstValue.maxLives :
               currentLives + amount;
        MessageSystem.TriggerEvent(MessageKey.UI.UpdatePlayerLives, playerController.EPlayer, currentLives);
    }

    private void HandleDeath()
    {
        ShowWinner();
        gameObject.SetActive(false);
    }

    public void IncreaseHp(int amount)
    {
        UpdateLives(amount);
    }

    public async void ActivateGodMode(int duration)
    {
        CallCancellationTokenSource();
        HandleInvincibleForm(true);
        await UniTask.Delay(duration * 1000);
        HandleInvincibleForm(false);
    }

    private void HandleInvincibleForm(bool state)
    {
        isInvincible = state;
        invincibleIndicator.SetActive(state);
    }

    private void CallCancellationTokenSource()
    {
        cancellationTokenSource.Cancel();
    }

    private void ShowWinner()
    {
        Debug.Log(GameController.Instance == null);
        GameController.Instance.SetupGameResult();
    }

    private void OnDestroy()
    {
        CallCancellationTokenSource();
    }
}
