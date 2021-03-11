# General
Timespinner Randomizer will randomize the location of items such as enquipment, relics, familiars, stat boosts, use items. The logic makes sure that each game you play is beatable

Unlike traditional randomizers, this randomizer will not alter the rom or exe of the game, instead it loads the game normally and alters its state in memory
Save files will be loaded & stored in folder different from the normal game so they are not at risk

The game considers nightmare as the final boss,	thus you require atleast access to the past (to break lazer gate), and timespinner wheel, spindle, three gears, keycard A or B and essence of space or lightwall spell to beat the game

Quest are not concidered part of the game, and they wont contain any progression item, however Neliste will still offer you an item when you talk to her

When you feel stuck, you can open the minimap and hold X (the key to remove map markers), it will highligh all locations that you can reach and still have items for you

For questions / remarks and feedback, feel free to join the #randomizer channel on the offical timespinner discord https://discord.gg/sZMqD9

# Instalation \ Startup
Download latests TsRandomizer.exe from the release page https://github.com/JarnoWesthof/TsRandomizer/releases

Just copy the TsRandomizer.exe to the same folder as Timespinner.exe is located, and start TsRandomizer.exe to start the game with the randomizer enabled, or Timespinner.exe to start the game normaly

##### Supported verisons
* Windows Steam verison 1.32 (latest)
* Windows DRM Free version 1.31 (latest)

##### Unsupported verisons
* Windows Store (not planned)
* Linux (under construction)
* Max (under construction)

# Gameplay changes
* Orb\Ring\Spell shop is nolonger available, orbs\rings\spells are dropped from random item locations instead
* Umbra orb is randomized in even if you dont own the password
* Achievements cannot be unlocked
* SaveStatue inside right elevator room is disabled to prevent a softlock
* SaveStatue down in the pit of the ancient pyramid is disabled to prevent a softlock
* Aelana and The Maw nolonger require twins to be killed before thier door unlocks

# New keybinds
* On the minimap screen, holding the secondairy button (default: X) will highligh all locations that you can reach and still have items for you
* In Load game menu, holding down the last orb button (default: left trigger) will show the seed id in numerical format
* In Load game menu, pressing the next orb button (default: right trigger) will the generation of a spoiler log, the spoiler logs are saved in same directory as the TsRandomizer.exe
* In Seed selection menu, holding down the next orb button (default: right trigger) will allow you to force select a seed even when its logicly unbeatable
* In Seed selection menu, Ctr+V can now be used to past text in the seed field in order to copy paste seeds into the game
* In Seed selection menu, Ctr+C can now be used to copy seed id to the clipboard

# Routing changes
* The drawbridge is now open by default, and doesnt require you to visit the twins demons
* Amadues Labaratory now requires keycard B to open
* To prevent players from getting stuck in the past when items are still available in the present, the transition room next to the refugee camp can now be used to freely travel back and forth between past and present
* When you obtain the pyramid keys, a random transition room is unlocked that could open new routing oppertunities
* The temporal gyre is closed, due to its high randomness inside an already randomized timezone, the timezone would be highly unstable

# Item Tracker
The TsRandomizer.ItemTracker.exe can be used to automagicly track any progression item you obtain ingame. It can for example be used by streamers that want to display thier current progression items to thier viewers
The item tracker features a few options:
* You resize the window width to make it wider or smaller, the items will automaticly align properly
* You can double click in the window to remove the borders
* You can use you scroll wheel to make the window bigger of smaller
* You can click with your right mouse button to change the background to a few pre-defined backgrounds

# Known bugs
##### I am aware of these issues, but i found them small enough not to fix at this moment
* When you already got the blade-orb and kill the kitty boss while standing at the items spawn point you will trigger the item pickup message but no item is rewarded
* To prevent a crash, the pedistal after killing dad is now temporary not randomized anymore
* Orb pedistal in room before emperor always spawn even if you didnt kill him yet
* Gassmask can be in area like library or xarion caves where the use has to acces to as his only tp is the starter left one
* Sometimes do download terminals do not correctly reward thier items
* Meyef & Talaria should be removed from droplist if they are a starter item
* Talaria should be tracked if its a starter item and starting progression will nolonger be required
* Skeleton animation shows the next progressive item but reward the correct one
* If Timespinner is placed in the incorrect folder it will display two error messages

# Todo
##### Things i plan on doing in the future
* Extend names of item locations in the spoiler log (with help from community on discord)
* Maybe make items drop from memories and journals
* option: make key doors require a specific key

# Ideas
##### Ideas that may never happen but i think will be fun when they are implemenbted
* Randomize quest items
* Randomize enemies
* Timesanity
* Make warpshard optionally a progressin item that unlocks all softlocks
* Randomize shop(s)