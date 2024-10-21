using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;
using StardewValley;
using StardewModdingAPI.Events;

namespace TreeSizeFramework.Managers
{
    internal class TextureManager
    {
        public class TreeTexCache
        {
            public Texture2D? Stump { get; set; }
            public Texture2D? Bark { get; set; }
        }

        public static Dictionary<Tree, TreeTexCache> WildTreeTexs = new();
        public static Dictionary<FruitTree, TreeTexCache> FruitTreeTexs = new();


        public static void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            WildTreeTexs.Clear();
            FruitTreeTexs.Clear();
        }

        public static void OnDayStarted_Textures(object? sender, DayStartedEventArgs e)
        {
            WildTreeTexs.Clear();
            FruitTreeTexs.Clear();
        }

        public static Texture2D? GetTexture(List<TreeTextureData> texdata, bool stump, Tree? tree = null, FruitTree? fruit = null)
        {
            if (tree != null)
            {
                if (!WildTreeTexs.TryGetValue(tree, out var texs))
                    texs = new();

                if (stump && texs.Stump == null || !stump && texs.Bark == null)
                {
                    if (stump)
                    {
                        texs.Stump = ChooseTexture(texdata, tree.Location, tree: tree);
                    }
                    else
                    {
                        texs.Bark = ChooseTexture(texdata, tree.Location, tree: tree);
                    }
                }

                if (stump) return texs.Stump;
                else return texs.Bark;
            }

            if (fruit != null)
            {
                if (!FruitTreeTexs.TryGetValue(fruit, out var texs))
                    texs = new();

                if (stump && texs.Stump == null || !stump && texs.Bark == null)
                {
                    if (stump)
                    {
                        texs.Stump = ChooseTexture(texdata, fruit.Location, fruit: fruit);
                    }
                    else
                    {
                        texs.Bark = ChooseTexture(texdata, fruit.Location, fruit: fruit);
                    }
                }

                if (stump) return texs.Stump;
                else return texs.Bark;
            }

            return null;
        }

        private static Texture2D? ChooseTexture(List<TreeTextureData>? data, GameLocation location, bool IgnoreSeasonsHere = false, Tree? tree = null, FruitTree? fruit = null)
        {
            double value = TreeRandom(tree, fruit);

            if (data != null && data?.Count > 0)
            {
                foreach (TreeTextureData entry in data)
                {
                    if (location != null && location.IsGreenhouse && entry.Season.HasValue || entry.Season.HasValue && IgnoreSeasonsHere)
                    {
                        if (entry.Season == Season.Spring)
                        {
                            if (Game1.content.DoesAssetExist<Texture2D>(entry.Texture))
                                return Game1.content.Load<Texture2D>(entry.Texture);
                        }
                    }
                    else if ((!entry.Season.HasValue || entry.Season == location?.GetSeason()) && (entry.Condition == null || GameStateQuery.CheckConditions(entry.Condition, location, random: Utility.CreateRandom(value, Game1.uniqueIDForThisGame / 2))))
                    {
                        if (Game1.content.DoesAssetExist<Texture2D>(entry.Texture))
                            return Game1.content.Load<Texture2D>(entry.Texture);
                    }
                }
                if (Game1.content.DoesAssetExist<Texture2D>(data[0].Texture))
                    return Game1.content.Load<Texture2D>(data[0].Texture);
            }
            return null;
        }

        private static double TreeRandom(Tree? tree = null, FruitTree? fruit = null)
        {
            string modid = ModEntry.instance.ModManifest.UniqueID;
            int value = Utility.CreateRandomSeed(Game1.random.NextDouble(), Game1.random.NextDouble());
            if (tree != null)
            {
                if (!tree.modData.TryGetValue($"{modid}.TreeR", out string v))
                {
                    v = $"{value}";
                    tree.modData.TryAdd($"{modid}.TreeR", v);
                }
                else
                {
                    value = int.Parse(v);
                }
            }
            else
            {
                if (!fruit!.modData.TryGetValue($"TreeR", out string v))
                {
                    v = $"{value}";
                    fruit!.modData.TryAdd($"{modid}.TreeR", v);
                }
                else
                {
                    value = int.Parse(v);
                }
            }

            return value;
        }
    }
}
