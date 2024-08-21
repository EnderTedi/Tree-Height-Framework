using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using StardewValley.ItemTypeDefinitions;
using TreeHeightFramework;
using StardewValley.Projectiles;
using StardewValley.Tools;

namespace BiggerTrees
{
    internal class ModEntry : Mod
    {
#nullable disable
        public static ModEntry instance;
#nullable enable

        public override void Entry(IModHelper helper)
        {
            instance = this;
            helper.Events.Content.AssetRequested += OnAssetRequested;

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll(typeof(ModEntry).Assembly);

        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.Name == $"{ModManifest.UniqueID}/WildTreeData")
            {
                e.LoadFrom(() => new Dictionary<string, CWildTreeData>(), AssetLoadPriority.Exclusive);
            }
            if (e.NameWithoutLocale.Name == $"{ModManifest.UniqueID}/FruitTreeData")
            {
                e.LoadFrom(() => new Dictionary<string, CFruitTreeData>(), AssetLoadPriority.Exclusive);
            }
        }

        public static Texture2D? TreeTexture(CWildTreeData? wildTreeData = null, CFruitTreeData? fruitTreeData = null)
        {
            if (wildTreeData != null)
            {
                if (wildTreeData.Textures is null)
                {
                    return null;
                }
                foreach (CWildTreeData.TextureData data1 in wildTreeData.Textures)
                {
                    if (data1.Texture is null) { continue; }
                    if ((data1.Condition == null || GameStateQuery.CheckConditions(data1.Condition)) && (Game1.GetSeasonKeyForLocation(Game1.player.currentLocation).Equals(data1.Season, StringComparison.OrdinalIgnoreCase) || data1.Season == null))
                    {
                        if (Game1.content.DoesAssetExist<Texture2D>(data1.Texture))
                            return Game1.content.Load<Texture2D>(data1.Texture);
                        else continue;
                    }
                }
            }
            else if (fruitTreeData != null)
            {
                if (fruitTreeData.Texture is null) return null;
                else if (Game1.content.DoesAssetExist<Texture2D>(fruitTreeData.Texture))
                    return Game1.content.Load<Texture2D>(fruitTreeData.Texture);
            }
            return null;
        }
    }

    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.performToolAction))]
    public static class performToolAction
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
            Vector2 tile = new(x, y);
            if (__instance.terrainFeatures != null)
            {
                Rectangle tileRect = new Rectangle(x * 64, y * 64, 64, 64);
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
        {;
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

    [HarmonyPatch(typeof(Tree), nameof(Tree.getBoundingBox))]
    public static class TreeGetBoundingBox
    {
        public static bool Prefix(Tree __instance, ref Rectangle __result)
        {
            var tData = Game1.content.Load<Dictionary<string, CWildTreeData>>($"{ModEntry.instance.ModManifest.UniqueID}/WildTreeData");
            if (tData.ContainsKey(__instance.treeType.Value))
            {
                var treeData = tData[__instance.treeType.Value];
                if (treeData.BoundingBoxWidth != 1 && treeData.BoundingBoxWidth > 0)
                {
                    Vector2 tileLocation = __instance.Tile;
                    Rectangle boundingBox = new((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);
                    boundingBox.Inflate((treeData.BoundingBoxWidth - 1) * 32, 0);
                    __result = boundingBox;
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(FruitTree), nameof(FruitTree.getBoundingBox))]
    public static class FruitTreeTreeGetBoundingBox
    {
        public static bool Prefix(FruitTree __instance, ref Rectangle __result)
        {
            var tData = Game1.content.Load<Dictionary<string, CFruitTreeData>>($"{ModEntry.instance.ModManifest.UniqueID}/FruitTreeData");
            if (tData.ContainsKey(__instance.treeId.Value))
            {
                var treeData = tData[__instance.treeId.Value];
                if (treeData.BoundingBoxWidth != 1 && treeData.BoundingBoxWidth > 0)
                {
                    Vector2 tileLocation = __instance.Tile;
                    Rectangle boundingBox = new((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);
                    boundingBox.Inflate((treeData.BoundingBoxWidth - 1) * 32, 0);
                    __result = boundingBox;
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Tree), "draw", new Type[] { typeof(SpriteBatch) })]
    public static class TreeDraw
    {
        public static bool Prefix(Tree __instance, SpriteBatch spriteBatch, List<Leaf> ___leaves)
        {

            var tData = Game1.content.Load<Dictionary<string, CWildTreeData>>($"{ModEntry.instance.ModManifest.UniqueID}/WildTreeData");
            if (!tData.ContainsKey(__instance.treeType.Value))
            {
                return true;
            }
            var treeData = tData[__instance.treeType.Value];
            Texture2D? texture = ModEntry.TreeTexture(wildTreeData: treeData);
            Vector2 tileLocation = __instance.Tile;
            float baseSortPosition = __instance.getBoundingBox().Bottom;
            if (texture == null || __instance.isTemporarilyInvisible || __instance.texture.Value == null || !Tree.TryGetData(__instance.treeType.Value, out var data) || __instance.growthStage.Value < 5)
            {
                return true;
            }
            else
            {
                if (!__instance.stump.Value || __instance.falling.Value)
                {
                    if (__instance.IsLeafy())
                        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - 51f, tileLocation.Y * 64f - 16f)), Tree.shadowSourceRect, Color.White * ((float)Math.PI / 2f - Math.Abs(__instance.shakeRotation)), 0f, Vector2.Zero, 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-06f);
                    else
                        spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - 51f, tileLocation.Y * 64f - 16f)), new Rectangle(469, 298, 42, 31), Color.White * ((float)Math.PI / 2f - Math.Abs(__instance.shakeRotation)), 0f, Vector2.Zero, 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-06f);
                    
                    Rectangle source_rect = new(0, 0, treeData.TreeWidth * 16, treeData.TreeHeight * 16);
                    if ((data.UseAlternateSpriteWhenSeedReady && __instance.hasSeed.Value) || (data.UseAlternateSpriteWhenNotShaken && !__instance.wasShakenToday.Value))
                        source_rect.X = treeData.TreeWidth * 16;
                    else
                        source_rect.X = 0;
                    if (__instance.hasMoss.Value)
                        source_rect.X = treeData.TreeWidth * 32;

                    spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f - (treeData.TreeWidth - 3) / 2  * 64f, tileLocation.Y * 64f + 64f - ((treeData.TreeHeight - 6) * 64f))), source_rect, Color.White * __instance.alpha, __instance.shakeRotation, new Vector2(24f, 96f), 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (baseSortPosition + 2f) / 10000f - tileLocation.X / 1000000f);
                }
                Rectangle stumpSource = new(treeData.TreeWidth * 16, treeData.TreeHeight * 16 - 32, treeData.TreeWidth * 16, 32);
                if (__instance.hasMoss.Value)
                    stumpSource.X += 96;
                if (__instance.health.Value >= 1f || (!__instance.falling.Value && __instance.health.Value > -99f))
                    spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - (treeData.TreeWidth - 1) / 2 * 64f + ((__instance.shakeTimer > 0f) ? ((float)Math.Sin(Math.PI * 2.0 / (double)__instance.shakeTimer) * 3f) : 0f), tileLocation.Y * 64f - 64f)), stumpSource, Color.White * __instance.alpha, 0f, Vector2.Zero, 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, baseSortPosition / 10000f);
                if (__instance.stump.Value && __instance.health.Value < 4f && __instance.health.Value > -99f)
                    spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - (treeData.TreeWidth - 1) / 2 * 64f + ((__instance.shakeTimer > 0f) ? ((float)Math.Sin(Math.PI * 2.0 / (double)__instance.shakeTimer) * 3f) : 0f), tileLocation.Y * 64f)), new Rectangle(Math.Min(2, (int)(3f - __instance.health.Value)) * 16, 144, 16, 16), Color.White * __instance.alpha, 0f, Vector2.Zero, 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (baseSortPosition + 1f) / 10000f);
            }

            foreach (Leaf i in ___leaves)
                spriteBatch.Draw(__instance.texture.Value, Game1.GlobalToLocal(Game1.viewport, i.position), new Rectangle(16 + i.type % 2 * 8, 112 + i.type / 2 * 8, 8, 8), Color.White, i.rotation, Vector2.Zero, 4f, SpriteEffects.None, baseSortPosition / 10000f + 0.01f);

            return false;
        }
    }

    [HarmonyPatch(typeof(FruitTree), "draw", new Type[] { typeof(SpriteBatch) })]
    public static class FruitTreeDraw
    {
        public static bool Prefix(FruitTree __instance, SpriteBatch spriteBatch, List<Leaf> ___leaves)
        {
            var tData = Game1.content.Load<Dictionary<string, CFruitTreeData>>($"{ModEntry.instance.ModManifest.UniqueID}/FruitTreeData");
            if (!tData.ContainsKey(__instance.treeId.Value))
                return true;

            var treeData = tData[__instance.treeId.Value];
            Texture2D? customTexture = ModEntry.TreeTexture(fruitTreeData: treeData);
            if (customTexture == null || __instance.isTemporarilyInvisible || __instance.texture == null || __instance.growthStage.Value < 4)
                return true;

            int seasonIndex = Game1.GetSeasonIndexForLocation(__instance.Location);
            int spriteRow = __instance.GetSpriteRowNumber();
            Vector2 tileLocation = __instance.Tile;
            Rectangle boundingBox = __instance.getBoundingBox();

            if (__instance.GreenHouseTileTree)
            {
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle(669, 1957, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-08f);
            }

            if (!__instance.stump.Value || __instance.falling.Value)
            {
                bool ignoreSeason = __instance.IgnoresSeasonsHere();
                if (!__instance.falling.Value)
                    spriteBatch.Draw(customTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f, tileLocation.Y * 64f + 64f)), new Rectangle(ignoreSeason ? 1 : seasonIndex * 3 * 16, 0 + (treeData.TreeHeight - 1) * 16, 48, 16), (__instance.struckByLightningCountdown.Value > 0) ? (Color.Gray * __instance.alpha) : (Color.White * __instance.alpha), 0f, new Vector2(24f, 16f), 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-07f);
                spriteBatch.Draw(customTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f, tileLocation.Y * 64f + 64f - ((treeData.TreeHeight - 5) * 64f))), new Rectangle(ignoreSeason ? 1 : seasonIndex * 3 * 16, 0, 48, (treeData.TreeHeight - 1) * 16), (__instance.struckByLightningCountdown.Value > 0) ? (Color.Gray * __instance.alpha) : (Color.White * __instance.alpha), __instance.shakeRotation, new Vector2(24f, 80f), 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)boundingBox.Bottom / 10000f + 0.001f - tileLocation.X / 1000000f);
            }

            if (__instance.health.Value >= 1f || (!__instance.falling.Value && __instance.health.Value > -99f))
                spriteBatch.Draw(__instance.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f + ((__instance.shakeTimer > 0f) ? ((float)Math.Sin(Math.PI * 2.0 / __instance.shakeTimer) * 2f) : 0f), tileLocation.Y * 64f + 64f)), new Rectangle(384, spriteRow * 5 * 16 + 48, 48, 32), (__instance.struckByLightningCountdown.Value > 0) ? (Color.Gray * __instance.alpha) : (Color.White * __instance.alpha), 0f, new Vector2(24f, 32f), 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (__instance.stump.Value && !__instance.falling.Value) ? (boundingBox.Bottom / 10000f) : (boundingBox.Bottom / 10000f - 0.001f - tileLocation.X / 1000000f));

            for (int i = 0; i < __instance.fruit.Count; i++)
            {
                ParsedItemData obj = (__instance.struckByLightningCountdown.Value > 0) ? ItemRegistry.GetDataOrErrorItem("(O)382") : ItemRegistry.GetDataOrErrorItem(__instance.fruit[i].QualifiedItemId);
                Texture2D texture = obj.GetTexture();
                Rectangle sourceRect2 = obj.GetSourceRect();
                switch (i)
                {
                    case 0:
                        spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - 64f + tileLocation.X * 200f % 64f / 2f, tileLocation.Y * 64f - 192f - tileLocation.X % 64f / 3f) - new Vector2(0, (treeData.TreeHeight - 5) * 64f)), sourceRect2, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, boundingBox.Bottom / 10000f + 0.002f - tileLocation.X / 1000000f);
                        break;
                    case 1:
                        spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f, tileLocation.Y * 64f - 256f + tileLocation.X * 232f % 64f / 3f) - new Vector2(0, (treeData.TreeHeight - 5) * 64f)), sourceRect2, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, boundingBox.Bottom / 10000f + 0.002f - tileLocation.X / 1000000f);
                        break;
                    case 2:
                        spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + tileLocation.X * 200f % 64f / 3f, tileLocation.Y * 64f - 160f + tileLocation.X * 200f % 64f / 3f) - new Vector2(0, (treeData.TreeHeight - 5) * 64f)), sourceRect2, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, boundingBox.Bottom / 10000f + 0.002f - tileLocation.X / 1000000f);
                        break;
                }
            }
            foreach (Leaf j in ___leaves)
                spriteBatch.Draw(__instance.texture, Game1.GlobalToLocal(Game1.viewport, j.position), new Rectangle((24 + seasonIndex) * 16, spriteRow * 5 * 16, 8, 8), Color.White, j.rotation, Vector2.Zero, 4f, SpriteEffects.None, (float)boundingBox.Bottom / 10000f + 0.01f);

            return false;
        }
    }
}
