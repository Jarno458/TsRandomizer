# General
Timespinner Randomizer will randomize the location of items such as enquipment, relics, familiars, stat boosts, use items. The logic makes sure that each game you play is beatable

Unlike traditional randomizers, this randomizer will not alter the rom or exe of the game, instead it loads the game normally and alters its state in memory
Save files will be loaded & stored in folder different from the normal game so they are not at risk

The game considers nightmare as the final boss,	thus you require atleast access to the past (to break lazer gate), and timespinner wheel, spindle, three gears, keycard A or B and essence of space or lightwall spell to beat the game

Quest are not concidered part of the game, and they wont contain any progression item, however Neliste will still offer you an item when you talk to her

# Instalation \ Startup
Download TsRandomizer.exe from the release page https://github.com/JarnoWesthof/TsRandomizer/releases

Just copy the TsRandomizer.exe to the same folder as Timespinner.exe is located, and start TsRandomizer.exe to start the game with the randomizer enabled, or Timespinner.exe to start the game normaly

##### Supported verisons
* Windows Steam verison 1.32 (latest)
* Windows DRM Free version 1.31 (latest)

# Gameplay changes
* Orb\Ring\Spell shop is nolonger available, orbs\rings\spells are dropped from random item locations instead
* Umbra orb is randomized in even if you dont own the password
* Achievements cannot be unlocked
* SaveStatue inside right elevator room is disabled to prevent a softlock

# Routing changes
* The drawbridge is now open by default, and doesnt require you to visit the twins demons
* Amadues Labaratory now requires keycard B to open
* To prevent got getting stuck in the past when items are still available in the present, the transition room next to the refugee camp can now be used to freely travel back and forth between past and present
* When you obtain the pyramid keys, a random transition room is unlocked that could open new routing oppertunities
* The temporal gyre is closed, due to its high randomness inside an already randomized timezone, the timezone would be highly unstable

# Known bugs
##### I am aware of these issues, but i found them small enough to fix at this moment
* Deleting save file does not delete seed representation
* Resizing render size, requires a restart of the game
* Menu tooltip is wrong on diffuclty selection screen
* Using the transition room near refugee camp without having pyramid keys renders the portal to small
* Player obtains Meyef to early

# Minor Todo
##### Things i plan on doing in the future
* disable quests maybe?
* Replace Replaces to Treasure Chest to replaces to ItemDropPickup

# Ideas
##### Ideas that may never happen but i think will be fun when they are implemenbted
* Make ice orb freeze enemies
* Randomize quest items
* Timesanity
* Upgradeable items (duuble jump => space jump) (Card D => C => B => A)
