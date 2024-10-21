using StardewModdingAPI.Events;
using StardewValley;

namespace TreeSizeFramework.Managers
{
    internal class AssetManager
    {
        private static Dictionary<string, CWildTreeData>? _WildData = null;
        public static Dictionary<string, CWildTreeData> WildData
        {
            get
            {
                _WildData ??= Game1.content.Load<Dictionary<string, CWildTreeData>>($"{ModEntry.instance.ModManifest.UniqueID}/WildTreeData");
                return _WildData!;
            }
        }

        private static Dictionary<string, CFruitTreeData>? _FruitData = null;
        public static Dictionary<string, CFruitTreeData> FruitData
        {
            get
            {
                _FruitData ??= Game1.content.Load<Dictionary<string, CFruitTreeData>>($"{ModEntry.instance.ModManifest.UniqueID}/FruitTreeData");
                return _FruitData!;
            }
        }

        public static void AssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.Name == $"{ModEntry.instance.ModManifest.UniqueID}/WildTreeData")
                e.LoadFrom(() => new Dictionary<string, CWildTreeData>(), AssetLoadPriority.Exclusive);
            if (e.NameWithoutLocale.Name == $"{ModEntry.instance.ModManifest.UniqueID}/FruitTreeData")
                e.LoadFrom(() => new Dictionary<string, CFruitTreeData>(), AssetLoadPriority.Exclusive);
        }

        public static void AssetInvalidated(object? sender, AssetsInvalidatedEventArgs e)
        {
            if (e.NamesWithoutLocale.Any(n => n.IsEquivalentTo($"{ModEntry.instance.ModManifest.UniqueID}/WildTreeData")))
                _WildData = null;
            if (e.NamesWithoutLocale.Any(n => n.IsEquivalentTo($"{ModEntry.instance.ModManifest.UniqueID}/FruitTreeData")))
                _FruitData = null;
        }
    }
}
