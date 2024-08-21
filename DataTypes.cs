using StardewValley.GameData.WildTrees;

namespace TreeHeightFramework
{
    public class CFruitTreeData
    {
        public List<WildTreeTextureData> Textures { get; set; } = new() { new WildTreeTextureData { Texture = "" } };
        public List<WildTreeTextureData> StumpTextures { get; set; } = new() { new WildTreeTextureData { Texture = "" } };
        public int TreeHeight { get; set; } = 5;
        public int TreeWidth { get; set; } = 3;
        public int BoundingBoxWidth { get; set; } = 1;
    }

    public class CWildTreeData
    {
        public List<WildTreeTextureData> Textures { get; set; } = new() { new WildTreeTextureData { Texture = "" } };
        public List<WildTreeTextureData> StumpTextures { get; set; } = new() { new WildTreeTextureData { Texture = "" } };
        public int TreeHeight { get; set; } = 6;
        public int TreeWidth { get; set; } = 3;
        public int BoundingBoxWidth { get; set; } = 1;
    }
}
