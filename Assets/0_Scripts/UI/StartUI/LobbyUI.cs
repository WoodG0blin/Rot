using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;

namespace Rot
{
    public class LobbyUI : AwaitableView
    {
        [SerializeField] private GameObject _lobbyPanel;
        [SerializeField] private Button _return;
        [SerializeField] private Button _play;
        [SerializeField] private PhotonLobbyUI _photonLobby;

        private bool _connectionSuccessfull;

        void Start()
        {
            _lobbyPanel.SetActive(false);
            _return.onClick.AddListener(LeaveLobby);
            _play.onClick.AddListener(StartPlay);

            _photonLobby.Initiate();
        }

        public async Task<bool> ConnectToGameServer()
        {
            _photonLobby.Connect(OnConnection);
            Debug.Log("Started connect to master server");
            await this;
            _lobbyPanel.SetActive(false);
            return currentAwaiter.GetResult();
        }

        private void OnConnection(bool success)
        {
            Debug.Log("Started connect to game server");
            if (success)
            {
                _lobbyPanel.SetActive(true);
                Debug.Log("Setting list of rooms");
                _photonLobby.ConnectToRoom(OnJoinRoom);
                _play.enabled = false;
            }
        }
        private void OnJoinRoom(bool success)
        {
            _play.enabled = success;
            if (!success) currentAwaiter?.Finish(false);
        }

        private void StartPlay()
        {
            currentAwaiter?.Finish(true);
        }

        private void LeaveLobby()
        {
            _photonLobby.Disconnect(OnLeaveLobby);
        }

        private void OnLeaveLobby()
        {
            currentAwaiter?.Finish(false);
        }
    }
}
