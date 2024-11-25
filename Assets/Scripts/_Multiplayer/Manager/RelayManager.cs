using System;
using System.Linq;
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
    #region OldCode

    //[SerializeField]
    //private string environment = "production";

    //[SerializeField]
    //private int maxNumberOfConnections = 10;

    //public bool IsRelayEnabled => Transport != null && Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;

    //public UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

    //public async Task SetupRelay()
    //{
    //    //Logger.Instance.LogInfo($"Relay Server Starting With Max Connections: {maxNumberOfConnections}");

    //    InitializationOptions options = new InitializationOptions()
    //        .SetEnvironmentName(environment);

    //    await UnityServices.InitializeAsync(options);

    //    //authentication to create access token
    //    if (!AuthenticationService.Instance.IsSignedIn)
    //    {
    //        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    //    }

    //    Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxNumberOfConnections);

    //    string JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
    //    Debug.LogError("Join code: " + JoinCode);


    //    Transport.SetRelayServerData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes,
    //            allocation.Key, allocation.ConnectionData);

    //    //Logger.Instance.LogInfo($"Relay Server Generated Join Code: {relayHostData.JoinCode}");
    //}
    //public async Task JoinRelay(string joinCode)
    //{
    //    //Logger.Instance.LogInfo($"Client Joining Game With Join Code: {joinCode}");

    //    InitializationOptions options = new InitializationOptions()
    //        .SetEnvironmentName(environment);

    //    await UnityServices.InitializeAsync(options);

    //    if (!AuthenticationService.Instance.IsSignedIn)
    //    {
    //        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    //    }

    //    JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

    //    Transport.SetRelayServerData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes,
    //            allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
    //    //Logger.Instance.LogInfo($"Client Joined Game With Join Code: {joinCode}");

    //}
    #endregion
    private bool _isHost = false;
    private string _joinCode;
    private string _ip;
    private int _port;
    private byte[] _connectionData;
    private byte[] _hostConnectionData;
    private byte[] _key;
    private System.Guid _allocationId;
    private byte[] _allocationIdBytes;

    public bool IsHost { get => _isHost; }

    public string GetAllocationId()
    {
        return _allocationId.ToString();
    }

    public string GetConnectionData()
    {
        return _connectionData.ToString();
    }

    public async Task<string> CreateRelay(int maxConnection)
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
        _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        RelayServerEndpoint dtlsEndPoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
        _ip = dtlsEndPoint.Host;
        _port = dtlsEndPoint.Port;

        _allocationId = allocation.AllocationId;
        _allocationIdBytes = allocation.AllocationIdBytes;
        _connectionData = allocation.ConnectionData;
        _key = allocation.Key;


        _isHost = true;
        return _joinCode;
    }

    public async Task<bool> JoinRelay(string joinCode)
    {
        _joinCode = joinCode;

        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        RelayServerEndpoint dtlsEndpoint = allocation.ServerEndpoints.First((conn => conn.ConnectionType == "dtls"));

        _ip = dtlsEndpoint.Host;
        _port = dtlsEndpoint.Port;

        _allocationId = allocation.AllocationId;
        _allocationIdBytes = allocation.AllocationIdBytes;
        _connectionData = allocation.ConnectionData;
        _hostConnectionData = allocation.HostConnectionData;
        _key = allocation.Key;

        return true;
    }

    public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, string _dtlsAddress, int _dtlsPort) GetHostConnectionInfo()
    {
        return (_allocationIdBytes, _key, _connectionData, _ip, _port);
    }

    public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, byte[] HostConnectionData, string _dtlsAddress, int _dtlsPort) GetClientConnectionInfo()
    {
        return (_allocationIdBytes, _key, _connectionData, _hostConnectionData, _ip, _port);
    }
}