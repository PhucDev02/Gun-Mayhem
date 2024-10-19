using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BoosterHandler : MonoBehaviour
{
    [SerializeField] private int lifeTime = 10;
    [SerializeField] private Booster booster;
    [SerializeField] private SpriteRenderer boosterSpr;

    public async void Init()
    {
        Debug.Log("Booster appeared");
        await Task.Delay(lifeTime * 1000);
        Debug.Log("Booster disappeared");
        DeactiveBooster();
    }

    private void DeactiveBooster()
    {
        ObjectPool.Instance?.Recall(gameObject);
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
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            BoosterManager.Instance.ActivateBooster(booster, playerHealth);
            ObjectPool.Instance?.Recall(gameObject);

        }
    }
}
