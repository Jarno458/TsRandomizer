using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	public enum CustomItem 
	{
		Archipelago,
		MeteorSparrowTrap,
		NeurotoxinTrap,
		ChaosTrap,
		PoisonTrap
	}

	public abstract class CustomItemBase : SingleItemInfo
	{
		const int Offset = 500;

		readonly int iconIndex;

		public override int AnimationIndex => iconIndex;

		protected CustomItemBase(CustomItem identifier, string name, string description, int iconIndex) : 
			base(new ItemIdentifier((EInventoryUseItemType)identifier + Offset))
		{
			this.iconIndex = iconIndex;
			//
			//TimeSpinnerGame.Localizer.OverrideKey("inv_use_MagicMarbles", "Archipelago Item");
			//TimeSpinnerGame.Localizer.OverrideKey("inv_use_MagicMarbles_desc", "Item that belongs to a distant timeline somewhere in the Archipelago (cannot be sold)");
		}
	}

	public class Trap : CustomItemBase
	{
		public Trap(CustomItem customItem)
			: base(customItem, customItem.ToString(), customItem.ToString(), 208)
		{
		}
		protected static void ApplyStatus(Level level, string status)
		{
			var statusEnumType = TimeSpinnerType.Get("Timespinner.GameObjects.StatusEffects.EStatusEffectType");
			level.MainHero.AsDynamic().GiveStatusEffect(statusEnumType.GetEnumValue(status), 100);
		}
	}

	public class SparrowTrap : Trap
	{
		public SparrowTrap() : base(CustomItem.MeteorSparrowTrap) {}

		public override void OnPickup(Level level)
		{
			var enemyTile = new ObjectTileSpecification {
				Category = EObjectTileCategory.Enemy,
				Layer = ETileLayerType.Objects,
				ObjectID = (int)EEnemyTileType.FlyingCheveux
			};

			var lunaisPos = level.MainHero.LastPosition;
			var sprite = level.GCM.SpGyreMeteorSparrow;
			var enemyType = TimeSpinnerType.Get("Timespinner.GameObjects.Enemies.GyreMeteorSparrow");
			var enemy = enemyType.CreateInstance(false, new Point(lunaisPos.X + 100, lunaisPos.Y - 50), level, sprite, -1, enemyTile);

			enemy.AsDynamic()._isAggroed = true;
			level.AsDynamic().RequestAddObject(enemy);
			
			enemy = enemyType.CreateInstance(false, new Point(lunaisPos.X - 100, lunaisPos.Y - 50), level, sprite, -1, enemyTile);
			enemy.AsDynamic()._isAggroed = true;
			level.AsDynamic().RequestAddObject(enemy);
		}
	}

	public class NeurotoxinTrap : Trap
	{
		public NeurotoxinTrap() : base(CustomItem.NeurotoxinTrap) {}

		public override void OnPickup(Level level) => ApplyStatus(level, "NeuroToxin");
	}

	public class ChaosTrap : Trap
	{
		public ChaosTrap() : base(CustomItem.ChaosTrap) {}

		public override void OnPickup(Level level) => ApplyStatus(level, "Chaos");
	}

	public class PoisonTrap : Trap
	{
		public PoisonTrap() : base(CustomItem.PoisonTrap) {}

		public override void OnPickup(Level level) => ApplyStatus(level, "Poison");
	}
}
