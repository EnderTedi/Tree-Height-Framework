using Microsoft.Xna.Framework;

namespace BiggerTrees
{
    internal class CWildTreeData
    {
        public TextureData[]? Textures { get; set; }

        public class TextureData
        {
            public string? Texture { get; set; }
            public string? Condition { get; set; }
            public string? Season { get; set; }
        }
        public int TreeHeight { get; set; } = 6;
    }
}
