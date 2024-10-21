using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;
using TreeSizeFramework.Managers;

namespace TreeSizeFramework
{
    internal partial class ModEntry : Mod
    {
        public static ModEntry instance = null!;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            helper.Events.Content.AssetRequested += AssetManager.AssetRequested;
            helper.Events.Content.AssetsInvalidated += AssetManager.AssetInvalidated;
            helper.Events.GameLoop.TimeChanged += LightManager.OnTimeChanged;
            helper.Events.GameLoop.DayStarted += LightManager.OnDayStarted_Lights;
            helper.Events.GameLoop.DayStarted += TextureManager.OnDayStarted_Textures;
            helper.Events.GameLoop.ReturnedToTitle += TextureManager.OnReturnedToTitle;

            helper.ConsoleCommands.Add("resettree", "...", (cmd, args) =>
            {
                TextureManager.FruitTreeTexs.Clear();
                TextureManager.WildTreeTexs.Clear();
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
    }

}
