using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley;
using Microsoft.Xna.Framework;

namespace TreeSizeFramework.Patches
{
    internal class GameLocationPatcher
    {

        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.performToolAction))]
        public static class PerformToolAction
        {
            public static bool Prefix(GameLocation __instance, Tool t, int tileX, int tileY)
            {
                if (t is Axe)
                {
                    Rectangle toolArea = new(tileX * 64, tileY * 64, 64, 64);
                    foreach (TerrainFeature feature in __instance.terrainFeatures.Values)
                    {
                        if (feature is Tree or FruitTree && feature.getBoundingBox().Intersects(toolArea))
                        {
                            feature.performToolAction(t, 1, feature.Tile);
                        }
                    }
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.isTerrainFeatureAt))]
        public static class IsTerrainFeatureAt
        {
            public static bool Prefix(GameLocation __instance, int x, int y, ref bool __result)
            {
                if (__instance.terrainFeatures != null)
                {
                    Rectangle tileRect = new(x * 64, y * 64, 64, 64);
                    foreach (TerrainFeature feature in __instance.terrainFeatures.Values)
                    {
                        if (feature is Tree or FruitTree && feature.getBoundingBox().Intersects(tileRect))
                        {
                            __result = true;
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.isCollidingPosition), new Type[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
        public static class IsCollidingPosition
        {
            public static bool Prefix(GameLocation __instance, Rectangle position, ref bool __result)
            {
                ;
                bool is_event_up = Game1.eventUp;
                if (is_event_up && Game1.CurrentEvent != null && !Game1.CurrentEvent.ignoreObjectCollisions)
                {
                    is_event_up = false;
                }
                foreach (TerrainFeature feature in __instance.terrainFeatures.Values)
                {
                    if (!is_event_up && feature is Tree tree)
                    {
                        if (tree.getBoundingBox().Intersects(position) && !tree.isPassable())
                        {
                            __result = true;
                            return false;
                        }
                    }
                    else if (!is_event_up && feature is FruitTree fTree)
                    {
                        if (fTree.getBoundingBox().Intersects(position) && !fTree.isPassable())
                        {
                            __result = true;
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.IsTileOccupiedBy), new Type[] { typeof(Vector2), typeof(CollisionMask), typeof(CollisionMask), typeof(bool) })]
        public static class IsTileOccupiedBy
        {
            public static bool Prefix(GameLocation __instance, Vector2 tile, ref bool __result)
            {
                var tData = Game1.content.Load<Dictionary<string, CWildTreeData>>($"{ModEntry.instance.ModManifest.UniqueID}/WildTreeData");
                var tData2 = Game1.content.Load<Dictionary<string, CFruitTreeData>>($"{ModEntry.instance.ModManifest.UniqueID}/FruitTreeData");
                foreach (TerrainFeature feature in __instance.terrainFeatures.Values)
                {
                    if (feature is Tree tree && tData.TryGetValue(tree.treeType.Value, out var treeData))
                    {
                        for (int i = 0; i < (treeData.BoundingBoxWidth + 2 - treeData.BoundingBoxWidth % 2) / 2; i++)
                        {
                            if (tile.Y == tree.Tile.Y)
                            {
                                if (tile.X == tree.Tile.X + i)
                                {
                                    __result = true;
                                    return false;
                                }
                                else if (tile.X == tree.Tile.X - i)
                                {
                                    __result = true;
                                    return false;
                                }
                            }
                        }
                    }
                    else if (feature is FruitTree ftree && tData2.TryGetValue(ftree.treeId.Value, out var fTreeData))
                    {
                        for (int i = 0; i < (fTreeData.BoundingBoxWidth + 2 - fTreeData.BoundingBoxWidth % 2) / 2; i++)
                        {
                            if (tile.Y == ftree.Tile.Y)
                            {
                                if (tile.X == ftree.Tile.X + i)
                                {
                                    __result = true;
                                    return false;
                                }
                                else if (tile.X == ftree.Tile.X - i)
                                {
                                    __result = true;
                                    return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.drawAboveFrontLayer))]
        public static class DrawAboveFrontLayer
        {
            public static bool Prefix(GameLocation __instance, SpriteBatch b)
            {
                Rectangle viewport = new(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height);
                foreach (TerrainFeature feat in __instance.terrainFeatures.Values)
                {
                    if (feat is Tree or FruitTree && feat.getRenderBounds().Intersects(viewport))
                    {
                        feat.draw(b);
                    }
                }
                return true;
            }
        }
    }
}
