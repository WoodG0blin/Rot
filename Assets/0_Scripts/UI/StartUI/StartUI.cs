using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rot
{
    public class StartUI : MonoBehaviour
    {
        [SerializeField] private Button _singlePlayer;
        [SerializeField] private Button _multiPlayer;
        [SerializeField] private Button _settings;
        [SerializeField] private Button _exit;
        [SerializeField] private PlayFabLogin _playfabLogin;
        [SerializeField] private LobbyUI _lobby;

        void Start()
        {
            if (_singlePlayer != null) _singlePlayer.onClick.AddListener(StartSinglePlayerGame);
            if (_multiPlayer != null) _multiPlayer.onClick.AddListener(StartMultiplayerGame);
            if (_settings != null) _settings.onClick.AddListener(ShowSettings);
            if (_exit != null) _exit.onClick.AddListener(ExitApplication);
        }


        private void StartSinglePlayerGame()
        {
            Debug.Log("Loading in single player mode");
        }

        private async void StartMultiplayerGame()
        {
            _multiPlayer.enabled = false;

            if (await _playfabLogin.LogIn())
            {
                if(await _lobby.ConnectToGameServer())
                    Debug.Log("Loading in multiplayer mode");
                else Debug.Log("Photon connection not established");
            }
            else Debug.Log("Cannot enter PlayFab account");

            _multiPlayer.enabled = true;
        }

        private void ShowSettings()
        {
            Debug.Log("Settings menu");
        }

        private void ExitApplication()
        {
            Debug.Log("Exiting game");
            Application.Quit();
        }
    }
}
