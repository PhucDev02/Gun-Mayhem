using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BoosterSpawner : MonoBehaviour
{
    [SerializeField] 
    private GameObject boosterPrefab;
    [SerializeField]
    public float spawnInterval = 5f;
    private CancellationTokenSource cancellationTokenSource;

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
                    //GameObject boosterInstance = Instantiate(boosterPrefab, GetRandomSpawnPosition(), Quaternion.identity);
                    GameObject boosterInstance = ObjectPool.Instance.Spawn(PoolObject.Booster);
                    boosterInstance.transform.position = GetRandomSpawnPosition();

                    BoosterHandler boosterComponent = boosterInstance.GetComponent<BoosterHandler>();
                    boosterComponent.SetBooster(selectedBooster);

                }
            }
        }
        catch (TaskCanceledException)
        {
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-14f, 14f);
        float y = 8f;
        float z = Random.Range(-10f, 10f);
        return new Vector3(x, y, z);
    }

    void OnDestroy()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
