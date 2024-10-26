using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BoosterSpawner : MonoBehaviour
{
    [SerializeField] 
    private GameObject boosterPrefab;
    [SerializeField]
    public float spawnInterval = 5f;
    private CancellationTokenSource cancellationTokenSource = new();

    private void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();

        SpawnBoosterAsync(cancellationTokenSource.Token);
    }

    private async void SpawnBoosterAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay((int)(spawnInterval * 1000), cancellationToken);
                Booster selectedBooster = BoosterManager.Instance.GetRandomBoosterByRate();
                Debug.Log("Spawn booster: " + selectedBooster.effectType.ToString());

                if (selectedBooster != null)
                {
                    GameObject boosterInstance = ObjectPool.Instance.Spawn(PoolObjectTag.Booster);
                    boosterInstance.transform.position = GetRandomSpawnPosition();

                    BoosterHandler boosterComponent = boosterInstance.GetComponent<BoosterHandler>();
                    boosterComponent.SetBooster(selectedBooster);

                }
            }
        }
        catch (TaskCanceledException)
        {
            Debug.Log("Task was canceled");
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-14f, 14f);
        float y = 12f;
        return new Vector3(x, y);
    }

    void OnDestroy()
    {
        cancellationTokenSource.Cancel();
    }
}
