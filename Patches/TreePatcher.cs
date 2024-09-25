using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using StardewValley;
using Microsoft.Xna.Framework;

namespace TreeSizeFramework.Patches
{
    internal class TreePatcher
    {
        [HarmonyPatch(typeof(Tree), "draw", new Type[] { typeof(SpriteBatch) })]
        public static class TreeDraw
        {
            public static bool Prefix(Tree __instance, SpriteBatch spriteBatch, List<Leaf> ___leaves)
            {
                if (!Context.IsWorldReady)
                {
                    return true;
                }

                var tData = Game1.content.Load<Dictionary<string, CWildTreeData>>($"{ModEntry.instance.ModManifest.UniqueID}/WildTreeData");
                if (!tData.TryGetValue(__instance.treeType.Value, out var treeData))
                {
                    return true;
                }

                Texture2D? texture = ModEntry.GetTexture(treeData.Textures, false, tree: __instance);
                Texture2D? stumpTexture = ModEntry.GetTexture(treeData.StumpTextures, true, tree: __instance);
                Vector2 tileLocation = __instance.Tile;
                float baseSortPosition = __instance.getBoundingBox().Bottom;

                if (texture == null || stumpTexture == null || __instance.isTemporarilyInvisible || __instance.texture.Value == null || !Tree.TryGetData(__instance.treeType.Value, out var data) || __instance.growthStage.Value < 5)
                    return true;

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

                    spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - (treeData.TreeWidth - 3) / 2 * 64f + (treeData.TreeWidth % 2 == 1 ? 32f : 0f), tileLocation.Y * 64f + 64f - ((treeData.TreeHeight - 6) * 64f))), source_rect, Color.White * __instance.alpha, __instance.shakeRotation, new Vector2(24f, 96f), 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (baseSortPosition + 2f) / 10000f - tileLocation.X / 1000000f);
                }

                Rectangle stumpSource = new(0, 0, stumpTexture.Width, 32);
                if (__instance.health.Value >= 1f || (!__instance.falling.Value && __instance.health.Value > -99f))
                    spriteBatch.Draw(stumpTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - (stumpTexture.Width - 16) * 2f + ((__instance.shakeTimer > 0f) ? ((float)Math.Sin(Math.PI * 2.0 / (double)__instance.shakeTimer) * 3f) : 0f), tileLocation.Y * 64f - 64f)), stumpSource, Color.White * __instance.alpha, 0f, Vector2.Zero, 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, baseSortPosition / 10000f);
                if (__instance.stump.Value && __instance.health.Value < 4f && __instance.health.Value > -99f)
                    spriteBatch.Draw(stumpTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - (stumpTexture.Width - 16) * 2f + ((__instance.shakeTimer > 0f) ? ((float)Math.Sin(Math.PI * 2.0 / (double)__instance.shakeTimer) * 3f) : 0f), tileLocation.Y * 64f - 64f)), stumpSource, Color.White * __instance.alpha, 0f, Vector2.Zero, 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (baseSortPosition + 1f) / 10000f);

                foreach (Leaf i in ___leaves)
                    spriteBatch.Draw(__instance.texture.Value, Game1.GlobalToLocal(Game1.viewport, i.position), new Rectangle(16 + i.type % 2 * 8, 112 + i.type / 2 * 8, 8, 8), Color.White, i.rotation, Vector2.Zero, 4f, SpriteEffects.None, baseSortPosition / 10000f + 0.01f);

                return false;
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

        [HarmonyPatch(typeof(Tree), nameof(Tree.getRenderBounds))]
        public static class TreeGetRenderBounds
        {
            public static bool Prefix(Tree __instance, ref Rectangle __result)
            {
                var tData = Game1.content.Load<Dictionary<string, CWildTreeData>>($"{ModEntry.instance.ModManifest.UniqueID}/WildTreeData");
                Vector2 tileLocation = __instance.Tile;
                if (__instance.stump.Value || __instance.growthStage.Value < 5 || !tData.TryGetValue(__instance.treeType.Value, out var treeData))
                {
                    return true;
                }
                __result = new Rectangle((int)(tileLocation.X - (treeData.TreeWidth / 2)) * 64, (int)(tileLocation.Y - treeData.TreeHeight) * 64, treeData.TreeWidth * 64 + 392, treeData.TreeHeight * 64 + 128);
                return false;
            }
        }
    }
}
