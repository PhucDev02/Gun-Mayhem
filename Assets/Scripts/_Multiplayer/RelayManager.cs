using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : Singleton<RelayManager>
{
    [SerializeField]
    private string environment = "production";

    [SerializeField]
    private int maxNumberOfConnections = 10;

    public bool IsRelayEnabled => Transport != null && Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;

    public UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

    public async Task SetupRelay()
    {
        //Logger.Instance.LogInfo($"Relay Server Starting With Max Connections: {maxNumberOfConnections}");

        InitializationOptions options = new InitializationOptions()
            .SetEnvironmentName(environment);

        await UnityServices.InitializeAsync(options);

        //authentication to create access token
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxNumberOfConnections);

        string JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.LogError("Join code: " + JoinCode);


        Transport.SetRelayServerData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes,
                allocation.Key, allocation.ConnectionData);

        //Logger.Instance.LogInfo($"Relay Server Generated Join Code: {relayHostData.JoinCode}");
    }

    public async Task JoinRelay(string joinCode)
    {
        //Logger.Instance.LogInfo($"Client Joining Game With Join Code: {joinCode}");

        InitializationOptions options = new InitializationOptions()
            .SetEnvironmentName(environment);

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

        Transport.SetRelayServerData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes,
                allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
        //Logger.Instance.LogInfo($"Client Joined Game With Join Code: {joinCode}");

    }
}