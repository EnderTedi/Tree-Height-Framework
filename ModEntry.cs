using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Tools;
using System.Transactions;
using xTile.Tiles;

namespace TreeSizeFramework
{
    internal partial class ModEntry : Mod
    {
#nullable disable
        public static ModEntry instance;
#nullable enable

        public static Dictionary<Tree, Dictionary<bool, Texture2D>> WTexturesCache = new();
        public static Dictionary<FruitTree, Dictionary<bool, Texture2D>> FTexturesCache = new();
        public static int randomnum = 0;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;

            helper.ConsoleCommands.Add("resettree", "...", (cmd, args) =>
            {
                WTexturesCache.Clear();
                FTexturesCache.Clear();
                Utility.ForEachLocation(l =>
                {
                    foreach (TerrainFeature f in l.terrainFeatures.Values)
                    {
                        if (f is Tree or FruitTree)
                        {
                            f.modData.Remove($"{ModManifest.UniqueID}.TreeR");
                            f.modData.Remove($"{ModManifest.UniqueID}.TreeNum");
                        }
                    }
                    return true;
                });
            });

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll(typeof(ModEntry).Assembly);
        }

        private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            WTexturesCache.Clear();
            FTexturesCache.Clear();
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            randomnum = 0;
            WTexturesCache.Clear();
            FTexturesCache.Clear();
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

        public static double TreeRandom(Tree? tree = null, FruitTree? fruit = null)
        {
            string modid = ModEntry.instance.ModManifest.UniqueID;
            double value;
            if (tree != null)
            {
                if (!tree.modData.TryGetValue($"{modid}.TreeR", out string v))
                {

                    long a = Game1.random.NextInt64();
                    long b = Game1.random.NextInt64();

                    if (a > b) value = a / b; else value = b / a;

                    v = $"{value}";
                    tree.modData.TryAdd($"{modid}.TreeR", v);
                }
                else
                {
                    value = int.Parse(v);
                }

                if (!tree.modData.ContainsKey($"{modid}.TreeNum"))
                {
                    tree.modData.TryAdd($"{modid}.TreeNum", $"{randomnum}");
                    randomnum++;
                }
            }
            else
            {
                if (!fruit!.modData.TryGetValue($"TreeR", out string v))
                {

                    long a = Game1.random.NextInt64();
                    long b = Game1.random.NextInt64();

                    if (a > b) value = a / b; else value = b / a;

                    v = $"{value}";
                    fruit!.modData.TryAdd($"{modid}.TreeR", v);
                }
                else
                {
                    value = int.Parse(v);
                }

                if (!fruit.modData.ContainsKey($"{modid}.TreeNum"))
                {
                    fruit.modData.TryAdd($"{modid}.TreeNum", $"{randomnum}");
                    randomnum++;
                }
            }

            return value;
        }

        public static Texture2D? GetTexture(List<TreeTextureData> texdata, bool stump, Tree? tree = null, FruitTree? fruit = null) 
        {
            if (tree != null)
            {
                if (!WTexturesCache.TryGetValue(tree, out var data) || !data.TryGetValue(stump, out Texture2D? tex))
                {
                    tex = ChooseTexture(texdata, tree.Location, tree: tree);
                    if (tex != null)
                    {
                        WTexturesCache.TryAdd(tree, new());
                        WTexturesCache[tree].TryAdd(stump, tex);
                    }
                    
                    return tex;
                }
                else
                {
                    return tex;
                }
            }

            if (fruit != null)
            {
                if (!FTexturesCache.TryGetValue(fruit, out var data) || !data.TryGetValue(stump, out Texture2D? tex))
                {
                    tex = ChooseTexture(texdata, fruit.Location, fruit.IgnoresSeasonsHere(), fruit: fruit);
                    if (tex != null)
                    {
                        FTexturesCache.TryAdd(fruit, new());
                        FTexturesCache[fruit].TryAdd(stump, tex);
                        return tex;
                    }
                    return null;
                }
                else
                {
                    return tex;
                }
            }

            return null;
        }

        public static Texture2D? ChooseTexture(List<TreeTextureData>? data, GameLocation location, bool IgnoreSeasonsHere = false, Tree? tree = null, FruitTree? fruit = null)
        {
            double value = TreeRandom(tree, fruit);
            double value2; 
            
            string modid = ModEntry.instance.ModManifest.UniqueID;
            if (tree != null)
            {
                value2 = int.Parse(tree.modData[$"{modid}.TreeNum"]);
            }
            else
            {
                value2 = int.Parse(fruit!.modData[$"{modid}.TreeNum"]);
            }

            if (data != null && data?.Count > 0)
            {
                foreach (TreeTextureData entry in data)
                {
                    if ((location != null && location.IsGreenhouse && entry.Season.HasValue) || (entry.Season.HasValue && IgnoreSeasonsHere))
                    {
                        if (entry.Season == Season.Spring )
                        {
                            if (Game1.content.DoesAssetExist<Texture2D>(entry.Texture))
                                return Game1.content.Load<Texture2D>(entry.Texture);
                        }
                    }
                    else if ((!entry.Season.HasValue || entry.Season == location?.GetSeason()) && (entry.Condition == null || GameStateQuery.CheckConditions(entry.Condition, location, random: Utility.CreateRandom(value, Game1.uniqueIDForThisGame, value2))))
                    {
                        randomnum++;
                        if (Game1.content.DoesAssetExist<Texture2D>(entry.Texture))
                            return Game1.content.Load<Texture2D>(entry.Texture);
                    }
                }
                if (Game1.content.DoesAssetExist<Texture2D>(data[0].Texture))
                    return Game1.content.Load<Texture2D>(data[0].Texture);
            }
            return null;
        }
    }

}
