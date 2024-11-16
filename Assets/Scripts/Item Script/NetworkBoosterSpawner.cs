using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Unity.Netcode;
using Sirenix.Reflection.Editor;
using Unity.Collections.LowLevel.Unsafe;
using Cysharp.Threading.Tasks;


public struct NetworkBoosterPackage : INetworkSerializable
{
    public int boosterId;
    public Vector3 boosterPosition;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref boosterId);
        serializer.SerializeValue(ref boosterPosition);
    }

    public NetworkBoosterPackage(int boosterId, Vector3 boosterPosition)
    {
        this.boosterId = boosterId;
        this.boosterPosition = boosterPosition;
    }
}

public class NetworkBoosterSpawner : NetworkBehaviour
{
    [SerializeField]
    public float spawnInterval = 5f;
    private int selectedBooster;
    private Vector3 boosterPosition;
    private CancellationTokenSource cancellationTokenSource = new();
    private NetworkVariable<NetworkBoosterPackage> networkBoosterPackage = 
        new NetworkVariable<NetworkBoosterPackage>(writePerm: NetworkVariableWritePermission.Server);

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
                await UniTask.WaitUntil(() => GameController.Instance.GetTotalPlayer() == 2);
                await Task.Delay((int)(spawnInterval * 1000), cancellationToken);
                selectedBooster = (int)BoosterManager.Instance.GetRandomBoosterByRate().effectType;
                boosterPosition = GetRandomSpawnPosition();
                //Debug.Log("Spawn booster: " + selectedBooster.effectType.ToString());
                SyncBoosterPackageClientRpc(selectedBooster, boosterPosition);

                GameObject boosterInstance = ObjectPool.Instance.Spawn(PoolObjectTag.Booster);
                boosterInstance.transform.position = networkBoosterPackage.Value.boosterPosition;

                BoosterHandler boosterComponent = boosterInstance.GetComponent<BoosterHandler>();
                boosterComponent.SetBooster(networkBoosterPackage.Value.boosterId);

            }
        }
        catch (TaskCanceledException)
        {
            Debug.Log("Task was canceled");
        }
    }

    [ClientRpc]
    public void SyncBoosterPackageClientRpc(int boosterId, Vector3 boosterPos)
    { 
        var boosterPack = new NetworkBoosterPackage(boosterId, boosterPos);
        networkBoosterPackage.Value = boosterPack;
        Debug.Log("Network booster: " + networkBoosterPackage.Value.boosterId + " " +
            networkBoosterPackage.Value.boosterPosition);
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
