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

        public bool Light { get; set; } = false;

        public LightData LightData { get; set; } = new();
    }

    public class CWildTreeData
    {
        public List<TreeTextureData> Textures { get; set; } = null!;

        public List<TreeTextureData> StumpTextures { get; set; } = null!;

        public int TreeHeight { get; set; } = 6;

        public int TreeWidth { get; set; } = 3;

        public int BoundingBoxWidth { get; set; } = 1;

        public bool Light { get; set; } = false;

        public LightData LightData { get; set; } = new();
    }

    public class TreeTextureData
    {
        public string? Condition;

        public Season? Season;

        public string Texture = null!;
    }

    public class LightData
    {
        public float Radius { get; set; } = 2;

        public string? Color { get; set; }

        public bool NightOnly { get; set; } = false;

        public bool GrownOnly { get; set; } = false;
    }
}
