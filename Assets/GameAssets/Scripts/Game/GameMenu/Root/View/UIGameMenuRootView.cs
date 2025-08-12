using AuroraWorld.GameMenu.View;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraWorld.GameMenu.Root.View
{
    public class UIGameMenuRootView : MonoBehaviour
    {
        [SerializeField] private Button _openCreateWorldButton;
        [SerializeField] private Button _openWorldButton;
        [SerializeField] private Button _openSettingsButton;
        [SerializeField] private Button _quitGameButton;

        [SerializeField] private UIGameMenuCreateWorldView _createWorldView;

        public Subject<(string worldName, int seedHash)> CreateWorldSignal => _createWorldView.CreateWorldSignal;
        public Subject<string> OpenWorldSignal { get; } = new();
        public Subject<Unit> QuitGameSignal { get; } = new();

        private void OnEnable()
        {
            ClearListeners();
            _openCreateWorldButton.onClick.AddListener(() =>
            {
                SetInteractable(true);
                _openCreateWorldButton.interactable = false;
            });
            _openWorldButton.onClick.AddListener(() =>
            {
                SetInteractable(true);
                _openWorldButton.interactable = false;
            });
            _openSettingsButton.onClick.AddListener(() =>
            {
                SetInteractable(true);
                _openSettingsButton.interactable = false;
            });
            _quitGameButton.onClick.AddListener(() =>
            {
                SetInteractable(true);
                _quitGameButton.interactable = false;
                QuitGameSignal.OnNext(Unit.Default);
                QuitGameSignal.Dispose(true);
            });
        }

        private void OnDisable()
        {
            ClearListeners();
        }

        private void ClearListeners()
        {
            _openCreateWorldButton.onClick.RemoveAllListeners();
            _openWorldButton.onClick.RemoveAllListeners();
            _openSettingsButton.onClick.RemoveAllListeners();
            _quitGameButton.onClick.RemoveAllListeners();
        }

        private void SetInteractable(bool value)
        {
            _openCreateWorldButton.interactable = value;
            _openWorldButton.interactable = value;
            _openSettingsButton.interactable = value;
            _quitGameButton.interactable = value;
        }
    }
}