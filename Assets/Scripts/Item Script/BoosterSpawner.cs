using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;


[System.Serializable]
public class BoosterDataPackage : INetworkSerializable
{
    public Vector3 spawnPos;
    public Booster booster;

    public BoosterDataPackage(Vector3 spawnPos, Booster booster)
    {
        this.spawnPos = spawnPos;
        this.booster = booster;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        throw new System.NotImplementedException();
    }
}


public class BoosterSpawner : NetworkBehaviour
{
    [SerializeField]
    public float spawnInterval = 5f;
    private bool canSpawn = false;
    private NetworkVariable<BoosterDataPackage> boosterDataPackage = new NetworkVariable<BoosterDataPackage>();
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
                //Debug.Log("Spawn booster: " + selectedBooster.effectType.ToString());

                if (selectedBooster != null)
                {
                    var spawnPos = GetRandomSpawnPosition();
                    SyncBoosterSpawnServerRpc(new BoosterDataPackage(spawnPos, selectedBooster));
                    
                }
            }
        }
        catch (TaskCanceledException)
        {
            Debug.Log("Task was canceled");
        }
    }

    private void Update()
    {
        if(canSpawn)
        {
            canSpawn = false;
            GameObject boosterInstance = ObjectPool.Instance.Spawn(PoolObjectTag.Booster);
            boosterInstance.transform.position = boosterDataPackage.Value.spawnPos;

            BoosterHandler boosterComponent = boosterInstance.GetComponent<BoosterHandler>();
            boosterComponent.SetBooster(boosterDataPackage.Value.booster);
        }
    }

    [ServerRpc]
    public void SyncBoosterSpawnServerRpc(BoosterDataPackage boosterDataPackage)
    {
        this.boosterDataPackage.Value = boosterDataPackage;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(ConstValue.environmentLimitX.x, ConstValue.environmentLimitX.y);
        float y = 12f;
        return new Vector3(x, y);
    }

    void OnDestroy()
    {
        cancellationTokenSource.Cancel();
    }
}
