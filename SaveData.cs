using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeSizeFramework
{
    internal partial class ModEntry
    {
        /*
        public static Dictionary<Tree, List<TreeTextureData>> WildTreeTextures { get; set; } = new();
        public static int Trees { get; set; } = 0;
        public static Dictionary<Tree, string> WildTrees { get; set; } = new();
        public static Dictionary<FruitTree, List<TreeTextureData>> FruitTreeTextures { get; set; } = new();
        public static int FTrees { get; set; } = 0;
        public static Dictionary<FruitTree, string> FruitTrees { get; set; } = new();
         */
        private void ReadData (object? sender, SaveLoadedEventArgs e)
        {
            var SaveData = Helper.Data.ReadGlobalData<Dictionary<string, TreeSizeFarmeworkSaveData>>("TreeSizeFrameworkSaveData");
            if (SaveData == null || !SaveData.TryGetValue($"{Game1.uniqueIDForThisGame}", out var Data)) return;

            WildTreeTextures = Data.WildTreeTextures;
            Trees = Data.Trees;
            WildTrees = Data.WildTrees;
            FruitTreeTextures = Data.FruitTreeTextures;
            FTrees = Data.FTrees;
            FruitTrees = Data.FruitTrees;
        }

        private void WriteData(object? sender, SavedEventArgs e)
        {
            var SaveData = Helper.Data.ReadGlobalData<Dictionary<string, TreeSizeFarmeworkSaveData>>("TreeSizeFrameworkSaveData") ?? new Dictionary<string, TreeSizeFarmeworkSaveData>();

            if (SaveData == null || !SaveData.TryGetValue($"{Game1.uniqueIDForThisGame}", out var Data)) {
                Data = new TreeSizeFarmeworkSaveData();
            }

            Data.WildTreeTextures = WildTreeTextures;
            Data.Trees = Trees;
            Data.WildTrees = WildTrees;
            Data.FruitTreeTextures = FruitTreeTextures;
            Data.FTrees = FTrees;
            Data.FruitTrees = FruitTrees;

            SaveData?.Remove($"{Game1.uniqueIDForThisGame}");
            SaveData?.Add($"{Game1.uniqueIDForThisGame}", Data);

            Helper.Data.WriteGlobalData("TreeSizeFrameworkSaveData", SaveData);
        }
    }
}
