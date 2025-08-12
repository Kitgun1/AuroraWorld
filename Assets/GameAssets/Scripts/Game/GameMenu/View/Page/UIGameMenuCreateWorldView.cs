using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraWorld.GameMenu.View
{
    public class UIGameMenuCreateWorldView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _worldNameInput;
        [SerializeField] private TMP_InputField _worldSeedInput;
        [SerializeField] private Button _createWorldButton;

        public readonly Subject<(string worldName, int seedHash)> CreateWorldSignal = new();

        private void OnEnable()
        {
            _createWorldButton.onClick.AddListener(() =>
            {
                if (_worldNameInput.text == "")
                {
                    Debug.LogWarning("Ошибка, имя не может быть пустым!");

                    // TODO: Отобразить ошибку в игровом процессе.
                    return;
                }
                var seed = _worldSeedInput.text == ""
                    ? Guid.NewGuid().ToString()
                    : _worldSeedInput.text;

                var seedHash = seed.GetHashCode();
                CreateWorldSignal.OnNext((_worldNameInput.text, seedHash));
            });
        }

        private void OnDisable()
        {
            _createWorldButton.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            CreateWorldSignal.Dispose(true);
        }
    }
}