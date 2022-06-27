/*
This folder contains the source code for Love/Hate.

- Core: The core faction scripts.
	- Faction Database: Faction database and its contents.
	- Faction Members: GameObject components for faction members and faction manager.
- Interaction: Scripts for interaction between factions.
	- Deeds: Deed template scripts to tie gameplay action to factions.
	- Gossip: Gossip trigger scripts for sharing rumors between factions.
	- Greeting: Greeting trigger script to react based on affinity when factions meet.

To support easy switching between source code and compiled DLLs, Love/Hate
uses wrapper classes located in the Wrappers folder.

Love/Hate uses the Pixel Crushers Common code library, located in 
Plugins/Pixel Crushers/Common.

If you want to use Assembly Definitions, import LoveHateAssemblyDefinitions.unitypackage.
Note: If you are using TextMesh Pro (i.e., have defined the scripting 
symbol TMP_PRESENT), you will need to update the Assembly Definitions
in this folder and in Common/Scripts to reference TextMesh Pro.
If you aren't using TextMesh Pro, you can use the Assembly Definitions
as-is.
*/