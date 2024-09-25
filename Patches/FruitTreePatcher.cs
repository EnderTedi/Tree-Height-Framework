using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TerrainFeatures;
using StardewValley;
using Microsoft.Xna.Framework;

namespace TreeSizeFramework.Patches
{
    internal class FruitTreePatcher
    {
        [HarmonyPatch(typeof(FruitTree), "draw", new Type[] { typeof(SpriteBatch) })]
        public static class FruitTreeDraw
        {
            public static bool Prefix(FruitTree __instance, SpriteBatch spriteBatch, List<Leaf> ___leaves)
            {
                if (!Context.IsWorldReady)
                {
                    return true;
                }

                var tData = Game1.content.Load<Dictionary<string, CFruitTreeData>>($"{ModEntry.instance.ModManifest.UniqueID}/FruitTreeData");
                if (!tData.TryGetValue(__instance.treeId.Value, out var treeData))
                {
                    return true;
                }
                Texture2D? texture = ModEntry.GetTexture(treeData.Textures, false, fruit: __instance);
                Texture2D? stumpTexture = ModEntry.GetTexture(treeData.Textures, true, fruit: __instance);
                Vector2 tileLocation = __instance.Tile;
                float baseSortPosition = __instance.getBoundingBox().Bottom;

                if (texture == null || stumpTexture == null || __instance.isTemporarilyInvisible || __instance.texture == null || __instance.growthStage.Value < 4)
                    return true;

                if (__instance.GreenHouseTileTree)
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle(669, 1957, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-08f);

                if (!__instance.stump.Value || __instance.falling.Value)
                {
                    Rectangle source_rect = new(0, 0, treeData.TreeWidth * 16, treeData.TreeHeight * 16);

                    spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - (treeData.TreeWidth - 3) / 2 * 64f + treeData.TreeWidth % 2 == 1 ? 32f : 0, tileLocation.Y * 64f + 64f - ((treeData.TreeHeight - 6) * 64f))), source_rect, Color.White * __instance.alpha, __instance.shakeRotation, new Vector2(24f, 96f), 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (baseSortPosition + 2f) / 10000f - tileLocation.X / 1000000f);
                }

                Rectangle stumpSource = new(0, 0, stumpTexture.Width, 32);
                if (__instance.health.Value >= 1f || (!__instance.falling.Value && __instance.health.Value > -99f))
                    spriteBatch.Draw(stumpTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - (stumpTexture.Width - 16) * 2f + ((__instance.shakeTimer > 0f) ? ((float)Math.Sin(Math.PI * 2.0 / (double)__instance.shakeTimer) * 3f) : 0f), tileLocation.Y * 64f - 64f)), stumpSource, Color.White * __instance.alpha, 0f, Vector2.Zero, 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, baseSortPosition / 10000f);
                if (__instance.stump.Value && __instance.health.Value < 4f && __instance.health.Value > -99f)
                    spriteBatch.Draw(stumpTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - (stumpTexture.Width - 16) * 2f + ((__instance.shakeTimer > 0f) ? ((float)Math.Sin(Math.PI * 2.0 / (double)__instance.shakeTimer) * 3f) : 0f), tileLocation.Y * 64f - 64f)), stumpSource, Color.White * __instance.alpha, 0f, Vector2.Zero, 4f, __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (baseSortPosition + 1f) / 10000f);

                for (int i = 0; i < __instance.fruit.Count; i++)
                {
                    ParsedItemData obj = (__instance.struckByLightningCountdown.Value > 0) ? ItemRegistry.GetDataOrErrorItem("(O)382") : ItemRegistry.GetDataOrErrorItem(__instance.fruit[i].QualifiedItemId);
                    Texture2D objTexture = obj.GetTexture();
                    Rectangle sourceRect2 = obj.GetSourceRect();
                    switch (i)
                    {
                        case 0:
                            spriteBatch.Draw(objTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - 64f + tileLocation.X * 200f % 64f / 2f, tileLocation.Y * 64f - 192f - tileLocation.X % 64f / 3f) - new Vector2(0, (treeData.TreeHeight - 5) * 64f)), sourceRect2, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, baseSortPosition);
                            break;
                        case 1:
                            spriteBatch.Draw(objTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f, tileLocation.Y * 64f - 256f + tileLocation.X * 232f % 64f / 3f) - new Vector2(0, (treeData.TreeHeight - 5) * 64f)), sourceRect2, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, baseSortPosition);
                            break;
                        case 2:
                            spriteBatch.Draw(objTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + tileLocation.X * 200f % 64f / 3f, tileLocation.Y * 64f - 160f + tileLocation.X * 200f % 64f / 3f) - new Vector2(0, (treeData.TreeHeight - 5) * 64f)), sourceRect2, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, baseSortPosition);
                            break;
                    }
                }

                foreach (Leaf i in ___leaves)
                    spriteBatch.Draw(__instance.texture, Game1.GlobalToLocal(Game1.viewport, i.position), new Rectangle(16 + i.type % 2 * 8, 112 + i.type / 2 * 8, 8, 8), Color.White, i.rotation, Vector2.Zero, 4f, SpriteEffects.None, baseSortPosition / 10000f + 0.01f);

                return false;
            }
        }

        [HarmonyPatch(typeof(FruitTree), nameof(FruitTree.getRenderBounds))]
        public static class FruitTreeGetRenderBounds
        {
            public static bool Prefix(FruitTree __instance, ref Rectangle __result)
            {
                var tData = Game1.content.Load<Dictionary<string, CFruitTreeData>>($"{ModEntry.instance.ModManifest.UniqueID}/FruitTreeData");
                Vector2 tileLocation = __instance.Tile;
                if (__instance.stump.Value || __instance.growthStage.Value < 5 || !tData.TryGetValue(__instance.treeId.Value, out var treeData))
                {
                    return true;
                }
                __result = new Rectangle((int)(tileLocation.X - (treeData.TreeWidth / 2)) * 64, (int)(tileLocation.Y - treeData.TreeHeight) * 64, treeData.TreeWidth * 64 + 392, treeData.TreeHeight * 64 + 128);
                return false;
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
    }
}
