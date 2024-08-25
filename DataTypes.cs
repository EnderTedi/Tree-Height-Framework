using StardewValley;
using StardewValley.TerrainFeatures;

namespace TreeSizeFramework
{
    public class CFruitTreeData
    {
        public List<TreeTextureData> Textures { get; set; } = null!;

        public List<TreeTextureData> StumpTextures { get; set; } = null!;

        public int TreeHeight { get; set; } = 5;

        public int TreeWidth { get; set; } = 3;

        public int BoundingBoxWidth { get; set; } = 1;

        public string UpdateRate { get; set; } = "TimeChanged, LocationChanged, DayStarted";
    }

    public class CWildTreeData
    {
        public List<TreeTextureData> Textures { get; set; } = null!;

        public List<TreeTextureData> StumpTextures { get; set; } = null!;

        public int TreeHeight { get; set; } = 6;

        public int TreeWidth { get; set; } = 3;

        public int BoundingBoxWidth { get; set; } = 1;

        public string UpdateRate { get; set; } = "TimeChanged, LocationChanged, DayStarted";
    }

    public class TreeTextureData
    {
        public string? Condition;

        public Season? Season;

        public string Texture = null!;
    }

    public class TreeSizeFarmeworkSaveData
    {
        public Dictionary<Tree, List<TreeTextureData>> WildTreeTextures { get; set; } = new();
        public int Trees { get; set; } = 0;
        public Dictionary<Tree, string> WildTrees { get; set; } = new();
        public Dictionary<FruitTree, List<TreeTextureData>> FruitTreeTextures { get; set; } = new();
        public int FTrees { get; set; } = 0;
        public Dictionary<FruitTree, string> FruitTrees { get; set; } = new();
    }
}
