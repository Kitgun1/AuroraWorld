using DI;
using UnityEngine;

namespace AuroraWorld
{
    public class LobbyContext : MonoBehaviour
    {
        private DIContainer _lobbyContainer;

        public void Init(DIContainer projectContainer)
        {
            _lobbyContainer = new DIContainer(projectContainer);
        }
    }
}