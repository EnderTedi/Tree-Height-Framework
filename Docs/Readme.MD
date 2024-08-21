**Tree Size Framework** is a [Stardew Valley](http://stardewvalley.net/) Mod which allows content creators to create wild and fruit trees with custom heights above the vanilla limit.

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
2. Install [this mod from Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/27123).
3. Run the game using SMAPI.

## For Modders
### Wild Tree Data
The `EnderTedi.TreeSizeFramework/WildTreeData` and `EnderTedi.TreeSizeFramework/FruitTreeData` assets contain the definition for custom tree heights for trees. These assets are editable through Content Patcher. 
The key should be tree's ID from `Data/WildTrees` and `Data/FruitTrees` respectively, with the value being a model with these fields:

Field | Description
----------- | -------
`Textures` | (Required) A list of `Textures` models. The first matching entry will be used. The `Textures` model is explained below.
`StumpTextures` | (Required) Same as `Textures` but for the stump texture.
`TreeHeight` | The new Tree Sprite's height. Is equivalent to the texture's height divided by 16. Must be a whole number. Default 6 for Wild Trees and 5 for Fruit Trees. 
`TreeWidth` | The new Tree Sprite's width. Is equivalent to the texture's width divided by 16. Must be a whole number. Default 3.
`BoundingBoxWidth` | The width of the area which you can interact with the tree. This should roughly match the size of the tree stump. Default 1. 

The `Textures` model contains these fields:

Field | Description
----------- | -------
`Texture` | (Required) The asset name for the new height-adjusted tree sprite.
`Season` | (Optional) The season in which this entry applies to.
`Condition` | (Optional) A [Game State Query](https://stardewvalleywiki.com/Modding:Game_state_queries) which dictates whether this entry should be chosen. If you're intending to only check the season use the `Season` field instead.

### Tree Sprites
The texture for Trees should be as follows:
Width: `TreeWidth` * 16 Pixels. For a tree with a width of 3 that would be 48 pixels wide.
Height: `TreeHeight` * 16 Pixels. For a tree with a dirth of 6 that would be 96 pixels tall.

The texture for the Stump should be as follows:
Width: `TreeWidth` * 16 Pixels. For a tree with a width of 3 that would be 48 pixels wide.
Height: `TreeHeight` * 16 Pixels. For a tree with a dirth of 6 that would be 96 pixels tall.

You are highly encouraged to also provide a default texture when adding a custom tree to `Data/WildTrees` or `Data/FruitTrees` as that will be the texture used if there's an error with your `WildTreeData` or this mod is uninstalled or stops working.

An example tree and stump texture:

![](https://i.imgur.com/3edeySK.png)
![](https://i.imgur.com/XE8NXlY.png)

### Examples

There's an example pack in the Optional Files section on Nexus Mods, however if you're just looking for example code:

```json
{
    "Format": "2.3.0",
    "Changes": [
        {
            "LogName": "Loading The Textures",
            "Action": "Load",
            "Target": "Textures/Tree, Textures/TreeStump, Textures/TreeBase",
            "FromFile": "{{TargetWithoutPath}}.png"
        },
        {
            "LogName": "Adding the Tree",
            "Action": "EditData",
            "Target": "Data/WildTrees",
            "Entries": {
                "EnderTedi.BigTree": {
                    "Textures": [
                        {
                            "Texture": "Textures/TreeBase",
                        },
                    ],
                    "SeedItemId": "(O)613",
                    "DebrisColor": 100004,
                    "IsLeafyInWinter": true,
                    "UseAlternateSpriteWhenNotShaken": false,
                    "ShakeItems": [
                        {
                            "ItemId": "(O)613",
                            "MinStack": 1,
                            "MaxStack": 1,
                            "Chance": 1
                        }
                    ],
                    "TapItems": [
                        {
                            "ItemId": "(O)613",
                            "DaysUntilReady": 5
                        }
                    ]
                }
            }
        },
        {
            "LogName": "Adjusting the Tree's Size",
            "Action": "EditData",
            "Target": "EnderTedi.TreeSizeFramework/WildTreeData",
            "Entries": {
                "EnderTedi.BigTree": {
                    "Textures": [
                        {
                            "Texture": "Textures/Tree",
                        }
                    ],
                    "StumpTextures": [
                        {
                            "Texture": "Textures/TreeStump",
                        }
                    ],
                    "TreeHeight": 8,
                }
            }
        }
    ]
}
```
For the tree and stump textures:

![](https://i.imgur.com/3edeySK.png)
![](https://i.imgur.com/XE8NXlY.png)
