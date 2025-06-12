using System.Collections.Generic;
using AuroraWorld.Gameplay.World.Geometry;

namespace Assets.Scripts.Utils
{
    public static class HexagonGroupUtils
    {
        public static List<List<HexEntityProxy>> GroupConnectedHexagons(IEnumerable<HexEntityProxy> hexagons)
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

                    while (queue.Count>0)
                    {
                        var current = queue.Dequeue();
                        group.Add(current);

                        foreach (var neighbor in current.GetNeighbors())
                        {
                            if(hexagon == null) continue;
                            if (allHexagons.Contains(neighbor) && !visited.Contains(neighbor))
                            {
                                visited.Add(neighbor);
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