using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Rot
{
    public class PhotonLobbyUI : MonoBehaviourPunCallbacks
    {
        private Action<bool> _onConnectToLobby;
        private Action<bool> _onConnectedToRoom;
        private Action _onDisconnectFromLobby;

        public void Initiate()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public void Connect(Action<bool> OnConnection)
        {
            if (PhotonNetwork.IsConnected) return;

            _onConnectToLobby = OnConnection;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = Application.version;
        }

        public void Disconnect(Action OnDisconnected)
        {
            if (PhotonNetwork.IsConnected)
            {
                _onDisconnectFromLobby = OnDisconnected;
                PhotonNetwork.Disconnect();
            }
        }

        public void ConnectToRoom(Action<bool> OnRoomJoined)
        {
            if (PhotonNetwork.IsConnected)
            {
                _onConnectedToRoom = OnRoomJoined;
                PhotonNetwork.JoinRandomOrCreateRoom();
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Photon master server");
            _onConnectToLobby?.Invoke(true);
            _onConnectToLobby = null;
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Connected to Photon room");
            _onConnectedToRoom?.Invoke(true);
            _onConnectedToRoom=null;
        }
        public override void OnErrorInfo(ErrorInfo errorInfo)
        {
            Debug.Log("Connection error");
            _onConnectToLobby?.Invoke(false);
            _onConnectedToRoom?.Invoke(false);
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Disconnected from Photon master server");
            _onDisconnectFromLobby?.Invoke();
            _onDisconnectFromLobby = null;
        }
    }
}
