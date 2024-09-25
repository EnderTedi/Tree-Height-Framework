using Microsoft.Xna.Framework;
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
    }

    public class CWildTreeData
    {
        public List<TreeTextureData> Textures { get; set; } = null!;

        public List<TreeTextureData> StumpTextures { get; set; } = null!;

        public int TreeHeight { get; set; } = 6;

        public int TreeWidth { get; set; } = 3;

        public int BoundingBoxWidth { get; set; } = 1;
    }

    public class TreeTextureData
    {
        public string? Condition;

        public Season? Season;

        public string Texture = null!;
    }
}
