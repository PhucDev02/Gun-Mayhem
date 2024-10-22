using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BoosterHandler : MonoBehaviour
{
    [SerializeField] private int lifeTime = 10;
    [SerializeField] private Booster booster;
    [SerializeField] private SpriteRenderer boosterSpr;
    private CancellationTokenSource cancellationTokenSource = new();


    public async void Init()
    {
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
        if(gameObject != null)
        {
            CallCancellationTokenSource();
            ObjectPool.Instance?.Recall(gameObject);
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
}
