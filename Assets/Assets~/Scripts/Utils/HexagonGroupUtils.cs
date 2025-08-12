using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.World.Geometry;
using AuroraWorld.Gameplay.World.Proxy;
using AuroraWorld.Gameplay.World.Terrain.Proxy;

namespace AuroraWorld.Utils
{
    public static class HexagonGroupUtils
    {
        public static List<List<HexagonProxy>> GroupByConnected(IEnumerable<HexagonProxy> hexagons, TerrainStateProxy terrain)
        {
            var allHexagons = new HashSet<HexagonProxy>(hexagons);
            var visited = new HashSet<HexagonProxy>();
            var groups = new List<List<HexagonProxy>>();

            foreach (var hexagon in allHexagons)
            {
                if (!visited.Contains(hexagon))
                {
                    var group = new List<HexagonProxy>();
                    var queue = new Queue<HexagonProxy>();
                    queue.Enqueue(hexagon);
                    visited.Add(hexagon);

                    while (queue.Count > 0)
                    {
                        var current = queue.Dequeue();
                        group.Add(current);

                        var neighbors = current.CubePosition.Neighbors()
                            .Select(terrain.LoadHexagon)
                            .Where(i => i != null)
                            .ToArray();
                        foreach (var neighbor in neighbors)
                        {
                            if (allHexagons.Contains(neighbor) && visited.Add(neighbor))
                            {
                                queue.Enqueue(neighbor);
                            }
                        }
                    }

                    groups.Add(group);
                }
            }

            return groups;
        }
    }
}