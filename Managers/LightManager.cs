using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;

namespace TreeSizeFramework.Managers
{
    internal class LightManager
    {
        public static void OnDayStarted_Lights(object? sender, DayStartedEventArgs e)
        {
            Utility.ForEachLocation((l) =>
            {
                foreach (TerrainFeature f in l.terrainFeatures.Values.Where(t => t is Tree tr && AssetManager.WildData.ContainsKey(tr.treeType.Value) && AssetManager.WildData[tr.treeType.Value].Light && !AssetManager.WildData[tr.treeType.Value].LightData.NightOnly))
                {
                    Tree t = (f as Tree)!;
                    var data = AssetManager.WildData[t.treeType.Value];
                    if (data.LightData.GrownOnly && t.growthStage.Value < 5 || Game1.currentLightSources.ContainsKey($"{t.treeType.Value}_{t.Location.NameOrUniqueName}_{t.Tile.X}_{t.Tile.Y}_TreeLight")) continue;
                    float r = data.LightData.Radius;
                    LightSource lightSource = new($"{t.treeType.Value}_{t.Location.NameOrUniqueName}_{t.Tile.X}_{t.Tile.Y}_TreeLight", 2, t.Tile * Game1.tileSize + new Vector2(32, 32), data.LightData.Radius);
                    Color color = Utility.StringToColor(data.LightData.Color) ?? Color.White;
                    lightSource.color.Value = color;
                    Game1.currentLightSources.Add(lightSource.Id, lightSource);
                }


                foreach (TerrainFeature f in l.terrainFeatures.Values.Where(t => t is FruitTree tr && AssetManager.FruitData.ContainsKey(tr.treeId.Value) && AssetManager.WildData[tr.treeId.Value].Light && !AssetManager.WildData[tr.treeId.Value].LightData.NightOnly))
                {
                    FruitTree t = (f as FruitTree)!;
                    var data = AssetManager.WildData[t.treeId.Value];
                    if (data.LightData.GrownOnly && t.growthStage.Value < 4 || Game1.currentLightSources.ContainsKey($"{t.treeId.Value}_{t.Location.NameOrUniqueName}_{t.Tile.X}_{t.Tile.Y}_TreeLight")) continue;
                    float r = data.LightData.Radius;
                    LightSource lightSource = new($"{t.treeId.Value}_{t.Location.NameOrUniqueName}_{t.Tile.X}_{t.Tile.Y}_TreeLight", 2, t.Tile * Game1.tileSize + new Vector2(32, 32), data.LightData.Radius);
                    Color color = Utility.StringToColor(data.LightData.Color) ?? Color.White;
                    lightSource.color.Value = color;
                    Game1.currentLightSources.Add(lightSource.Id, lightSource);
                }
                return true;
            });
            Game1.currentLightSources.RemoveWhere(l => l.Value.Id.EndsWithIgnoreCase("TreeNightLight"));
        }

        public static void OnTimeChanged(object? sender, TimeChangedEventArgs e)
        {
            Utility.ForEachLocation((l) =>
            {
                if (e.NewTime == Game1.getStartingToGetDarkTime(l) + 100)
                {
                    foreach (TerrainFeature f in l.terrainFeatures.Values.Where(t => t is Tree tr && AssetManager.WildData.ContainsKey(tr.treeType.Value) && AssetManager.WildData[tr.treeType.Value].Light && AssetManager.WildData[tr.treeType.Value].LightData.NightOnly))
                    {
                        Tree t = (f as Tree)!;
                        var data = AssetManager.WildData[t.treeType.Value];
                        if (data.LightData.GrownOnly && t.growthStage.Value < 5) continue;
                        float r = data.LightData.Radius;
                        LightSource lightSource = new($"{t.treeType.Value}_{t.Location.NameOrUniqueName}_{t.Tile.X}_{t.Tile.Y}_TreeNightLight", 2, t.Tile * Game1.tileSize + new Vector2(32, 32), data.LightData.Radius);
                        Color color = Utility.StringToColor(data.LightData.Color) ?? Color.White;
                        lightSource.color.Value = color;
                        Game1.currentLightSources.Add(lightSource.Id, lightSource);
                    }


                    foreach (TerrainFeature f in l.terrainFeatures.Values.Where(t => t is FruitTree tr && AssetManager.FruitData.ContainsKey(tr.treeId.Value) && AssetManager.WildData[tr.treeId.Value].Light && AssetManager.WildData[tr.treeId.Value].LightData.NightOnly))
                    {
                        FruitTree t = (f as FruitTree)!;
                        var data = AssetManager.WildData[t.treeId.Value];
                        if (data.LightData.GrownOnly && t.growthStage.Value < 4) continue;
                        float r = data.LightData.Radius;
                        LightSource lightSource = new($"{t.treeId.Value}_{t.Location.NameOrUniqueName}_{t.Tile.X}_{t.Tile.Y}_TreeNightLight", 2, t.Tile * Game1.tileSize + new Vector2(32, 32), data.LightData.Radius);
                        Color color = Utility.StringToColor(data.LightData.Color) ?? Color.White;
                        lightSource.color.Value = color;
                        Game1.currentLightSources.Add(lightSource.Id, lightSource);
                    }
                }
                return true;
            });
        }
    }
}
