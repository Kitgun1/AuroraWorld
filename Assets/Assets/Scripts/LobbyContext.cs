using DI;
using UnityEngine;

namespace AuroraWorld
{
    public class LobbyContext : MonoBehaviour
    {
        public void Init(DIContainer projectContainer)
        {
            var lobbyContainer = new DIContainer(projectContainer);
            
        }
    }
}