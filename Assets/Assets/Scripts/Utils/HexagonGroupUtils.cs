using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.World;
using AuroraWorld.Gameplay.World.Geometry;

namespace Assets.Scripts.Utils
{
    public static class HexagonGroupUtils
    {
        public static List<List<HexagonProxy>> GroupByConnected(IEnumerable<HexagonProxy> hexagons, Terrain terrain)
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

                        var neighbors = current.Position.Neighbors()
                            .Select(i => terrain.ContainsHexagon(i) ? terrain.AttachHexagon(i, out _) : null)
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