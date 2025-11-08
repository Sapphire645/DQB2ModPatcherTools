# DQB2 Mod Patcher Tools
Two programs, one that packs a collection of modded files, and the other that patches the modded files into the game.<br>
This is a very crude mod installer, but it might do the job for those that want to mod the game.

## DQB2 Mod Patcher 
This program pathces mods into the game. <br>
Please read the [information file](./DQB2ModInstaller/information.txt) before using it.<br>
Mods use the .dqb2.bin format. Those can be created with the companion tool.<br>

## DQB2 Mod Packer
This program packs files. It basically bundles a bunch of .idxzrc (what the [LINKDATA browser](https://github.com/turtle-insect/DQB2) gives you) alongside where they go per version of the game.
### How it works
- You can select .idxzrc files in the "add files" button.
- You can either add a new game version by inputting the name and number of indices, or select a linkdata to have the program calculate the number.
- Per file, double click on the number of the version to tell it what index it corresponds to.

This way, a mod can be compatible with multiple versions of the game, including switch, demos, steam, etc... but only as long as you make it so.

## How to edit the .idxzrc files to create mods
- [Information](https://github.com/default-kramer/default-kramer.github.io/wiki)
  
## Special thanks
- [turtle-insect](https://github.com/turtle-insect/)
- [Benxc](https://gbatemp.net/threads/dragon-quest-builders-2-modding.572482/)

## Requirements
Windows 10 and .NET 8 I believe.
