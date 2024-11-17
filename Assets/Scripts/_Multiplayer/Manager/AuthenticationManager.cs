using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Multiplayer.Manager
{
    public class AuthenticationManager : Singleton<AuthenticationManager>
    {
        private async void Start()
        {
            await UnityServices.InitializeAsync();

            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                AuthenticationService.Instance.SignedIn += OnSignedIn;
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if(AuthenticationService.Instance.IsSignedIn)
                {
                    string userName = PlayerPrefs.GetString("Username");
                    if(string.IsNullOrEmpty(userName))
                    {
                        userName = "Player";
                        PlayerPrefs.SetString("Username", userName);
                    }
                }
            }
        }

        private void OnSignedIn()
        {
            Debug.Log("PlayerId: " + AuthenticationService.Instance.PlayerId);
            Debug.Log("AccessToken: " + AuthenticationService.Instance.AccessToken);
        }
    }
}
