using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BoosterHandler : MonoBehaviour
{
    [SerializeField] private int lifeTime = 10;
    [SerializeField] private Booster booster;
    [SerializeField] private SpriteRenderer boosterSpr;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D boxCollider;
    private CancellationTokenSource cancellationTokenSource = new();

    private void OnValidate()
    {
        boosterSpr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }


    public async void Init()
    {
        boosterSpr.color = Color.white;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;
        boxCollider.enabled = true;
        try
        {
            cancellationTokenSource = new();
            await Task.Delay(lifeTime * 1000, cancellationTokenSource.Token);
            DeactiveBooster();
        }
        catch (TaskCanceledException)
        {
            Debug.Log("Task was canceled");
        }
    }

    private void DeactiveBooster()
    {
        if (gameObject != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
            boxCollider.enabled = false;
            boosterSpr.DOFade(0, 0.1f).SetLoops(5, LoopType.Yoyo).OnComplete(() =>
            {
                ObjectPool.Instance?.Recall(gameObject);
                CallCancellationTokenSource();
            });
        }
    }

    public void SetBooster(Booster booster)
    {
        this.booster = booster;
        if (boosterSpr != null && booster.boosterSpr != null)
        {
            boosterSpr.sprite = booster.boosterSpr;
        }
        Init();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            PlayerSystem playerSystem = collision.gameObject.GetComponent<PlayerSystem>();
            BoosterManager.Instance.ActivateBooster(booster, playerSystem);
            DeactiveBooster();
        }
    }

    private void CallCancellationTokenSource()
    {
        cancellationTokenSource.Cancel();
    }

    void OnDestroy()
    {
        CallCancellationTokenSource();
    }

    private void OnDisable()
    {
        boosterSpr?.DOKill();
    }
}
