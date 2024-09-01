using Microsoft.Xna.Framework.Graphics;
using Netcode;
namespace TreeSizeFramework
{
    public class TreeExtData
    {
        public Lazy<Texture2D> BarkTexture;

        public Texture2D StumpTexture
        {
            get
            {
                return StumpTexture;
            }
            set
            {
                if (value != null)
                {
                    StumpTexture = value;
                }
            }
        }

        public readonly NetLong TreeGUID = new();
    }
}
