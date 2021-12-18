using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens.Settings;


namespace TsRandomizer.LevelObjects.Other
{
    [TimeSpinnerType("Timespinner.GameObjects.NPCs.MerchantCrowNPC")]
    class MerchantCrowNpc : LevelObject
    {

        MerchantInventory _merchandiseInventory = new MerchantInventory();
        public MerchantCrowNpc(Mobile typedObject) : base(typedObject)
        {

        }

        protected override void Initialize(SeedOptions options)
        {
			GameSettingsCollection gameSettings = new GameSettingsCollection();
			gameSettings.LoadSettingsFromFile();
			string fillType = gameSettings.ShopFill.CurrentValue;
			if (fillType == "Vanilla")
				return;
			if (fillType == "Empty")
			{
				Dynamic._merchandiseInventory = _merchandiseInventory;
				return;
			}
			PlayerInventory inventory = Dynamic._level.GameSave.Inventory;

            // Only sell warp shards if Pyramid Key is aquired
            if (inventory.RelicInventory.IsRelicActive(EInventoryRelicType.PyramidsKey))
                _merchandiseInventory.AddItem(EInventoryUseItemType.WarpCard);
            if (Dynamic._isInPresent)
            {
                _merchandiseInventory.AddItem(EInventoryUseItemType.FuturePotion);
                _merchandiseInventory.AddItem(EInventoryUseItemType.FutureEther);
            }
            else
            {
                _merchandiseInventory.AddItem(EInventoryUseItemType.Potion);
                _merchandiseInventory.AddItem(EInventoryUseItemType.Ether);
            }
            _merchandiseInventory.AddItem(EInventoryUseItemType.Biscuit);
            _merchandiseInventory.AddItem(EInventoryUseItemType.Antidote);
            _merchandiseInventory.AddItem(EInventoryUseItemType.SandBottle);
            _merchandiseInventory.AddItem(EInventoryUseItemType.ChaosHeal);

            Dynamic._merchandiseInventory = _merchandiseInventory;
        }
    }
}
