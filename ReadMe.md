# General
Timespinner Randomizer will randomize the location of items such as equipment, relics, familiars, stat boosts, use items. The logic makes sure that each game you play is beatable.

Unlike traditional randomizers, this randomizer will not alter the executable, instead it loads the game normally and alters its state in memory.
Save files will be loaded & stored in folder different from the normal game so they are not at risk.

The game considers Nightmare as the final boss, as such, to complete a seed you will need to collect all five pieces of the Timespinner and progress through the Ancient Pyramid. Your path to do so will be affected by the progression items (keycards, vertical movement, etc.) you find, as well as the flags you choose for the seed.

Quests are not considered part of the randomizer, and they will not contain any progression items, however Neliste will still offer you an item when you talk to her, as will the Librarian.

When you feel stuck, you can open the minimap and hold the melee button (the mapping to remove map markers), it will highlight all locations that you can reach which have remaining checks.

For questions / remarks and feedback, feel free to join the #randomizer channel on the official timespinner discord https://discord.gg/Timespinner
# Installation \ Startup
Download latest TsRandomizer.exe or Linux / Mac zip file from the release page https://github.com/Jarno458/TsRandomizer/releases

For Windows:
Extract the contents of the corresponding zip file to the same folder as Timespinner.exe is located, and start TsRandomizer.exe to start the game with the randomizer enabled, or Timespinner.exe to start the game normally
For Linux:
Extract the contents of the corresponding zip file to the same folder as Timespinner.exe is located, and start TsRandomizer to start the game with the randomizer enabled, or Timespinner to start the game normally
For Mac:
Copy the contents of the Mac/ folder in the mac zip file to the `Timespinner.app/Contents/MacOS` folder. You may need to run `xattr -dr com.apple.quarantine TsRandomizer TsRandomizer.bin.osx` and `chmod +x TsRandomizer` to make execution possible. Then start TsRandomizer to start the game with the randomizer enabled, or Timespinner to start the game normally.
 
##### Supported versions
* Windows Steam version 1.33 (latest)
* Windows DRM Free version 1.33 (latest)
* Linux Steam version 1.33 (latest)
  * Steam-Deck is supported after a manuel change, see [SteamDeck](#steamdeck) below
* Linux DRM Free version 1.33 (latest)
* Mac Steam version 1.33 (latest)
* Mac DRM Free version 1.33 (latest)

##### Unsupported versions
* Windows Store (not planned)

### SteamDeck
In order to use the randomizer on a steam deck, you will have to install the randomizer files manually in the desktop mode, and in order to get the steamdeck keybaord to work you will need to override the SDL2.dll inside your timespinner folder with [this one (yes this is a windows dll)](https://github.com/libsdl-org/SDL/releases/download/release-2.26.5/SDL2-2.26.5-win32-x86.zip)

# Gameplay changes
* Orb\Ring\Spell shop is no longer available, orbs\rings\spells are dropped from random item locations instead
* Umbra orb and familiar skins are in the item pool and do not require a password
* Achievements cannot be unlocked
* SaveStatue inside right elevator room is disabled to prevent a softlock
* A return warp to the Pyramid entrance has been added in the Pyramid pit to prevent a softlock
* SaveStatues in front of Temportal Gyre bosses Ifrit and Ravenlord are disabled to prevent softlocks
* Aelana and The Maw no longer require twins to be killed before their door unlocks
* Softlock exits are conditionally added the Lab and Hangar when Risky Warps are on and certain lasers are active

# New keybinds
* On the minimap screen, holding the melee button (default: X) will highlight all locations that you can reach and still have items for you
* In Load game menu, holding down the left trigger will show the seed id in numerical format
* In Load game menu, holding down the left trigger + X will delete all saves
* In Load game menu, pressing the right trigger will trigger the generation of a spoiler log, the spoiler logs are saved in same directory as the TsRandomizer.exe
* In Load game menu, Ctr+C can be used to copy seed id of the currently selected save to the clipboard
* In Seed selection menu, holding down the right trigger will allow you to force select a seed even when its logically unbeatable
* In Seed selection menu, Ctr+V can be used to past text in the seed field in order to copy paste seeds into the game
* In Seed selection menu, Ctr+C can be used to copy seed id to the clipboard
* Anywhere, ` (tilde) will open the console. the console can be used to activate some command see /help for a list of available commands, if you are connected to an archipelago server any text typed in the console is sent to the server, giving you access to server commands

# Routing Changes
* The drawbridge is now open by default, and doesn't require you to visit the Demon Twins in Lake Serene.
* Amadeus' Laboratory now requires keycard B to open
* To prevent players from getting stuck in the past when items are still available in the present, the transition room next to the refugee camp can now be used to freely travel back and forth between past and present
* When you obtain the twin pyramid key, a random transition room is unlocked that could open new routing opportunities. In the "Unchained Keys" flag, these bonus warps are found as items.
* When using the "Gyre Archives" flag, locations from the Temporal Gyre are accessible from the present. The main loop can be reached in the Military Hangar after clearing the past, Ifrit can be reached by aquiring Kobo and proceeding to the portrait room in the Library, while Ravenlord can be reached by aquiring Merchant Crow and proceeding to the room in Amadeus' Laboratory containing the Historical Archives
* When using the "Prism Break" flag, lasers protecting the Military Hangar are disabled by collecting items, rather than by killing the 3 relevant bosses.
* When using the "Lock Key Amadeus" flag, lab lasers are disabled via items, and the power remains on. Junk spawners will continue to fire, and experiments will also spawn. The blast door remains closed.

# Safety Checks
Certain considerations were taken as anti-frustration changes in the seed routing. Sometimes this means certain items cannot appear in certain locations, other times it means while a difficult and/or tedious check may potentially be possible, the game will not consider it "in logic" until another requirement is reached.
## General
* Selen will not give you Plasma Orb as a starter melee orb, as it quickly drains a low-level Lunais' aura.
* The warp gates for Xarion, Azure Queen, Emperor's Tower, and Amadeus' Laboratory are only given as free warps when Risky Warps is on, due to their high level enemies or immediate bosses.
* The Military Hangar chest guarded by falling bombs requires the Timespinner Wheel, though can be reached without it by using the Celestial Sash.
* The larger "Struggle Juggle" maneuver, which requires jumping off of and repositioning a frozen Royal Guard enemy multiple times is only in logic with double jumps available, even if technically possible with single jumps.
* While Meyef is able to burn vines, they do not count as a fire source in logic. (Two vines can be burned in this fashion by aggroing enemies into the correct locations, while the third can be burned by a second player)
* While possible to tank oxygen damage in poison areas (Lake Desolation post-Maw), doing so will not be considered in logic.
* * Note that when the Stinky Maw flag is on, the plasma crystal is considered in logic with the Talaria attachment, as it is reachable without fully expending oxygen (i.e. before the damaging phase).
## When using the Gyre Archives Flag
* The Military Hangar portal will not be in logic until you possess the Timespinner Wheel. The items within can be obtained without it, but if you enter a room with a Nethershade you will have to wait for the room timer to expire, opening the doors.
* Ifrit in the portrait room will not be required unless you have access to the past. This safeguard is to ensure that an early Ifrit fight does not block access to the past.

# Item Tracker
The TsRandomizer.ItemTracker.exe can be used to automagicly track any progression item you obtain ingame. It can for example be used by streamers that want to display their current progression items to their viewers
The item tracker features a few options:
* You resize the window width to make it wider or smaller, the items will automatically align properly.
* You can double click in the window to remove the borders
* You can use you scroll wheel to make the window bigger of smaller.
* You can click with your right mouse button to change the background to a few per-defined backgrounds.

# Damage Randomizer
There are several settings for randomization of how much damage orbs, spells, and some rings do. Some notes on that:
* Orbs, spells, and rings are connected. If an orb is buffed, then all three will be buffed, etc.
* Most ring buffs and nerfs do nothing, but it is noted on the item for information about the rest of the set. Pyro, Scythe, and Icicle ring damage is affected.
* There are 5 presets for damage randomization:
  * All Nerfs: Every orb is significantly weaker.
  * Mostly Nerfs: For fans of the way Damage Rando used to work. Each orb has a 5/8 chance of being nerfed, a 1/4 chance of being normal, and a 1/8 chance of being buffed.
  * Balanced: Each orb has an even chance to be buffed, nerfed, or unaffected.
  * Mostly Buffs: Each orb has a 1/8 chance of being nerfed, a 1/4 chance of being unaffected, and a 5/8 chance of being buffed.
  * All Buffs: Every orb is significantly stronger.
* There is also a "Manual" mode that can be used with a manual editing of your settings.json file.
  * The first time you launch Timespinner Randomizer, a path and a settings.json file will be added to your game folder.
  * The section "DamageRandoOverrides" in settings.json can be manually edited to weight the odds of each orb. You can set a specific odds to 0 in order to remove its possibility entirely.

# Sprite Selection
The sprites for all three of Lunais' skins (and any other sprites added to settings) can be customized in the 'Sprites' menu under settings.

The sprites for each setting will pull from XNB files both in the vanilla location of `Content/Sprites/Heroes` and the custom location of `Custom Sprites/<Character Name>`. The Custom Sprites folder comes preloaded with a few example sprites.
If additional sprites are copied to the appropriate character subfolder within the custom sprites folder, they can be selected in settings.

There exist several programs that can extract XNB files into an editable image format, and can then repackage them for use as XNB files.

# Known Bugs
##### I am aware of these issues, but I found them small enough not to fix at this moment
* Having the Varndagrath collected away will cause the door to close and softlock the player
* Orb pedestal in the room before Emperor Nuvius always spawns even if you didn't kill him yet
* If Timespinner is placed in the incorrect folder it will not display the FileNotFoundMessage as it needs SDL library to do that
* When Progressive Vertical Movement is enabled, collecting Hairpin/Lightwall/Sash may show a popup with the wrong item

# Ideas
##### Ideas that may never happen but I think will be fun when they are implemented:
* Randomize quest items
* Timesanity (entrance randomizer)
* Make warpshard optionally a progression item that unlocks all softlocks
