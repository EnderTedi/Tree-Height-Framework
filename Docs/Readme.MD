**Tree Height Framework** is a [Stardew Valley](http://stardewvalley.net/) Mod which allows content creators to create wild and fruit trees with custom heights above the vanilla limit.

## Content
- [For Players](#For-Players)
  - [Install](#Install)
- [For Modders](#For-Modders)
  - [Tree Data](#Tree-Data)
  - [Tree Sprites](#Tree-Sprites)
  - [Examples](#Examples)

## For Players
### Install
1. Install [the latest version of SMAPI](https://smapi.io/).
2. Install [this mod from Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/26081).
3. Run the game using SMAPI.

## For Modders
### Wild Tree Data
The `EnderTedi.TreeHeightFramework/WildTreeData` and `EnderTedi.TreeHeightFramework/FruitTreeData` assets contain the definition for custom tree heights for trees, where the key is the tree's ID from `Data/WildTrees` and `Data/FruitTrees` respectively, with the value being a model with these fields:

Field | Description
----------- | -------
`Textures` | (Required) (Wild Trees Only) A list of `Textures` models. The first matching entry will be used. The `Textures` model is explained below.
`Texture` | (Required) (Fruit Trees Only) The asset name for the new height-adjusted tree sprite.
`TreeHeight` | (Required) The new Tree Sprite's height. Is equivalent to the texture's height divided by 16. Anything below 4 or 3 may look odd or weird. Must be a whole number.

The `Textures` model contains these fields:

Field | Description
----------- | -------
`Texture` | (Required) The asset name for the new height-adjusted tree sprite.
`Season` | (Optional) The season in which this entry applies to.
`Condition` | (Optional) A [Game State Query](https://stardewvalleywiki.com/Modding:Game_state_queries) which dictates whether this entry should be chosen. If you're intending to only check the season use the `Season` field instead.

### Tree Sprites
#### Wild Trees
For Wild Trees the texture should be a 3 tile wide (48 pixel) texture with the height having to be a multiple of 16 pixels. For the stump the Wild Trees will use the sprite from the texture provided in `Data/WildTrees` and should not include it in the texture for the texture provided for the Tree Height Framework.
An example tree texture: ![](https://i.imgur.com/3edeySK.png)

#### Fruit Trees
For Fruit Trees the texture should be a 12 tile wide (192 pixel) texture with the height having to be a multiple of 16 pixels. The Fruit Tree texture will be split into 4 3 tile wide (48 pixel) sections, each containing a sprite for each season (in the order: `Spring, Summer, Fall, Winter`).
An example fruit tree texture: ![](https://i.imgur.com/yfA1dZs.png)

### Examples
#### Wild Tree Example
```json
"Changes": [
    {
        "Action": "Load",
        "Target": "Textures/WildTree",
        "FromFile": "WildTree.png"
    },
    {
        "Action": "EditData",
        "Target": "EnderTedi.TreeHeightFramework/WildTreeData",
        "Entries": {
            "1": {
                "Textures": [
                    {
                        "Texture": "Textures/WildTree",
                        "Season": null,
                        "Condition": null
                    }
                ],
                "TreeHeight": "8"
            }
        }
    },
]
```
For the texture:
![](https://i.imgur.com/3edeySK.png)

#### Fruit Tree Example
```json
"Changes": [
    {
        "Action": "Load",
        "Target": "Textures/FruitTree",
        "FromFile": "FruitTree.png"
    },
    {
        "Action": "EditData",
        "Target": "EnderTedi.TreeHeightFramework/FruitTreeData",
        "Entries": {
            "628": {
                "Texture": "Textures/FruitTree",
                "TreeHeight": "3",
            }
        }
    }
]
```
For the texture:
![](https://i.imgur.com/yfA1dZs.png)