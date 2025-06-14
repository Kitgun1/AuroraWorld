using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;
using Assets.Utils.Coroutine;
using AuroraWorld.App.GameResources;
using AuroraWorld.Gameplay.Player.Proxy;
using AuroraWorld.Gameplay.World.Geometry;
using UnityEngine;
using Terrain = AuroraWorld.Gameplay.World.Terrain;

namespace AuroraWorld.Gameplay.Player
{
    public class HexagonMapSelections
    {
        private readonly Dictionary<string, Selection> _selections = new();
        private readonly Resource<Material> _selectionMaterialsResource = new();

        public void AttachSelection(string tag, SelectionSettingsProxy settings, Terrain terrain, params HexagonProxy[] hexagons)
        {
            var selection = SelectionMeshLoadOrCreate(tag);
            _selections.TryAdd(tag, selection);

            var selected = new List<HexagonProxy>(selection.ThickLineMesh.SelectedHexagons);
            if (settings.RemoveMode.Value)
            {
                if (selected.Count == 0)
                {
                    RemoveSelection(tag);
                    return;
                }

                var intersections = CubeMath.IntersectingRanges(
                    hexagons.Select(h => h.Position).ToArray(),
                    selected.Select(h => h.Position).ToArray()
                );
                var resultSelected = selected.Where(s => intersections.Any(i => i == s.Position)).ToArray();
                if (settings.OnlyNeighbor.Value)
                {
                    var groupCount = HexagonGroupUtils.GroupByConnected(resultSelected, terrain).Count;
                    if (groupCount == 0)
                    {
                        RemoveSelection(tag);
                        return;
                    }

                    if (groupCount == 1) selection.ThickLineMesh.AttachMesh(resultSelected);
                }
                else
                {
                    selection.ThickLineMesh.AttachMesh(resultSelected);
                }
            }
            else
            {
                if (settings.OnlyNeighbor.Value && selected.Count > 0)
                {
                    var connectedGroups = HexagonGroupUtils.GroupByConnected(hexagons, terrain);

                    foreach (var group in connectedGroups)
                    {
                        if (group.Any(h => h.Position.Neighbors()
                                .Select(i => terrain.ContainsHexagon(i) ? terrain.AttachHexagon(i, out _) : null)
                                .Any(h1 => selected.Any(s => s.Position == h1?.Position))))
                        {
                            selected.AddRange(group);
                        }
                    }
                }
                else
                {
                    selected.AddRange(hexagons);
                }

                selection.ThickLineMesh.AttachMesh(selected.ToArray());
            }

            selection.Filter.mesh = selection.ThickLineMesh.Mesh;
        }

        public void RemoveSelection(string tag)
        {
            if (!_selections.TryGetValue(tag, out var selection)) return;

            Object.Destroy(selection.Object);
            _selections.Remove(tag);
        }

        private Selection SelectionMeshLoadOrCreate(string tag)
        {
            if (_selections.TryGetValue(tag, out var selection))
            {
                return selection;
            }

            var selectionObject = new GameObject("outline");
            var filter = selectionObject.AddComponent<MeshFilter>();
            var renderer = selectionObject.AddComponent<MeshRenderer>();
            renderer.material = _selectionMaterialsResource.Load("Selection Material");
            var thickLineMesh = new ThickLineMesh();
            return new Selection(filter, renderer, thickLineMesh);
        }

        private readonly struct Selection
        {
            public GameObject Object => Filter.gameObject;
            public MeshFilter Filter { get; }
            public MeshRenderer Renderer { get; }
            public ThickLineMesh ThickLineMesh { get; }

            public Selection(MeshFilter filter, MeshRenderer renderer, ThickLineMesh thickLineMesh)
            {
                Filter = filter;
                Renderer = renderer;
                ThickLineMesh = thickLineMesh;
            }
        }
    }
}