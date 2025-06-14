using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.World;
using AuroraWorld.Gameplay.World.Geometry;

namespace Assets.Scripts.Utils
{
    public static class HexagonGroupUtils
    {
        public static List<List<HexEntityProxy>> GroupConnectedHexagons(IEnumerable<HexEntityProxy> hexagons, WorldTerrain worldTerrain)
        {
            var allHexagons = new HashSet<HexEntityProxy>(hexagons);
            var visited = new HashSet<HexEntityProxy>();
            var groups = new List<List<HexEntityProxy>>();

            foreach (var hexagon in allHexagons)
            {
                if (!visited.Contains(hexagon))
                {
                    var group = new List<HexEntityProxy>();
                    var queue = new Queue<HexEntityProxy>();
                    queue.Enqueue(hexagon);
                    visited.Add(hexagon);

                    while (queue.Count > 0)
                    {
                        var current = queue.Dequeue();
                        group.Add(current);

                        var neighbors = current.Position.Neighbors()
                            .Select(i => worldTerrain.ContainsHexagon(i) ? worldTerrain.AttachHexagon(i, out _) : null)
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