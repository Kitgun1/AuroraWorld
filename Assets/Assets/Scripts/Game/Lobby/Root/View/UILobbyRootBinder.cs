using R3;
using UnityEngine;

namespace AuroraWorld.Game.Lobby.Root.View
{
    public class UILobbyRootBinder : MonoBehaviour
    {
        private Subject<Unit> _exitSceneSignalSubject;

        public void HandleGoToGameplayButtonClick()
        {
            _exitSceneSignalSubject?.OnNext(Unit.Default);
        }

        public void Bind(Subject<Unit> exitSceneSignalSubject)
        {
            _exitSceneSignalSubject = exitSceneSignalSubject;
        }
    }
}