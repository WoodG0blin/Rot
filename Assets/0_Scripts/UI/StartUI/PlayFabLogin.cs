using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

namespace Rot
{
    public class PlayFabLogin : AwaitableView
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private Image _background;
        [SerializeField] private Button _clickArea;

        public bool LoggedIn { get; private set; }

        void Start()
        {
            LoggedIn = false;
            _background.color = Color.grey;
            _label.enabled = LoggedIn;

            _clickArea.onClick.AddListener(() => LogIn());

            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
                PlayFabSettings.staticSettings.TitleId = "DFF3F";
        }

        public async Task<bool> LogIn()
        {
            if (LoggedIn) return LoggedIn;

            _clickArea.enabled = false;

            var request = new LoginWithCustomIDRequest
            {
                CustomId = "WoodGoblin",
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
            await this;

            LoggedIn = currentAwaiter.GetResult();
            _background.color = LoggedIn ? Color.green : Color.grey;
            _label.enabled = LoggedIn;
            _clickArea.enabled = true;

            return LoggedIn;
        }

        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("Successfully logged in");
            currentAwaiter.Finish(true);
        }

        private void OnLoginFailure(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            Debug.LogError($"Login error: {errorMessage}");
            currentAwaiter.Finish(false);
        }
    }
}
