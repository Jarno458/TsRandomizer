using System;
using System.Linq;
using TsRandomizer.Extensions;
using System.Collections.Generic;
using Archipelago.Gifting.Net.Gifts;
using Archipelago.Gifting.Net.Gifts.Versions.Current;
using Archipelago.Gifting.Net.Service;
using Archipelago.Gifting.Net.Traits;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Newtonsoft.Json.Linq;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using APGiftingService = Archipelago.Gifting.Net.Service.GiftingService;

namespace TsRandomizer.Archipelago.Gifting
{
	class GiftingService
	{
		static readonly string[] KnownTraits = Enum.GetNames(typeof(Trait));

		readonly IGiftingServiceSync service;
		readonly IPlayerHelper players;

		public int NumberOfGifts {
			get;
#if DEBUG
			set;
#else
			private set;
#endif
		}

		public GiftingService(ArchipelagoSession session, int team, int slot)
		{
			service = new APGiftingService(session);
			players = session.Players;

			//service.SubscribeToNewGifts(OnGiftsChanged);
			Client.DataStorage[$"GiftBox;{team};{slot}"].OnValueChanged += OnOnValueChanged;

			service.CheckGiftBox();
		}

		void OnOnValueChanged(JToken originalvalue, JToken newvalue, Dictionary<string, JToken> additionalArguments)
		{
			try
			{
				NumberOfGifts = newvalue.ToObject<Dictionary<string, JToken>>()?.Count ?? 0;
			}
			catch (Exception e)
			{
				ScreenManager.Console.AddException(e, "GiftingService.OnOnValueChanged() Failed");
			}
		}

		void OnGiftsChanged(Dictionary<string, Gift> gifts) =>
			NumberOfGifts = gifts.Count;

		public List<AcceptedTraits> GetAcceptedTraits()
		{
			try
			{
				var acceptTraitsPerPlayer = service.GetAcceptedTraitsByPlayer(Client.Team, KnownTraits);
				var acceptedTraitsPerSlot = new List<AcceptedTraits>(acceptTraitsPerPlayer.Count);

				foreach (var acceptTraitsForPlayer in acceptTraitsPerPlayer)
				{
					var team = acceptTraitsForPlayer.Value.Team;
					var slot = acceptTraitsForPlayer.Value.Player;

					var playerInfo = Client.Players.GetPlayerInfo(team, slot);
					var playerGame = playerInfo?.Game ?? "Unknown Game";
					var playerAlias = playerInfo?.Alias ?? $"Unknown Player {slot}";

					var acceptedTraits = new AcceptedTraits
					{
						Team = team,
						Slot = slot,
						Name = playerAlias,
						Game = playerGame, 
						AcceptsAnyTrait = acceptTraitsForPlayer.Value.Traits.Length == KnownTraits.Length,
						DesiredTraits = acceptTraitsForPlayer.Value.Traits
							.Select(t => (Trait)Enum.Parse(typeof(Trait), t))
							.ToArray()
					};

					acceptedTraitsPerSlot.Add(acceptedTraits);
				}

				return acceptedTraitsPerSlot;
			}
			catch (Exception e)
			{
				ScreenManager.Console.AddException(e, "GiftingService.GetAcceptedTraits() Failed");

				return new List<AcceptedTraits>();
			}
		}

		public bool Send(InventoryItem item, AcceptedTraits playerInfo, int amount)
		{
			try
			{
				var giftItem = new GiftItem(GetItemName(item), amount, 0);
				var traits = TraitMapping.ValuesPerItem[item]
					.Select(t => new GiftTrait(t.Key.ToString(), 1, t.Value))
					.ToArray();

				return service.SendGift(giftItem, traits, players.GetPlayerName(playerInfo.Slot), playerInfo.Team);
			}
			catch (Exception e)
			{
				ScreenManager.Console.AddException(e, "GiftingService.Send() Failed");
			}

			return false;
		}

		string GetItemName(InventoryItem item)
		{
			ItemIdentifier identifier;

			switch (item)
			{
				case InventoryUseItem useItem:
					identifier = new ItemIdentifier(useItem.UseItemType);
					break;
				case InventoryEquipment equipment:
					identifier = new ItemIdentifier(equipment.EquipmentType);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(item), "parameter should be either UseItem or Equipment");
			}

			return Client.ItemsHelper.GetItemName(ItemMap.GetItemId(identifier)) ?? item.Name;
		}

		public void AcceptGift(Gift gift, int acceptedAmount)
		{
			try
			{
				if (acceptedAmount >= gift.Amount)
					service.RemoveGiftFromGiftBox(gift.ID);
				else
				{
					gift.Amount -= acceptedAmount;

					var updatedGift = new Dictionary<string, Gift>(1);

					Client.DataStorage[$"GiftBox;{Client.Team};{Client.Slot}"] += Operation.Update(updatedGift);
				}
			}
			catch (Exception e)
			{
				ScreenManager.Console.AddException(e, "GiftingService.AcceptGift() Failed");
			}
		}

		public void RejectGift(Gift gift)
		{
			try
			{
				service.RefundGift(gift);
				service.RemoveGiftFromGiftBox(gift.ID);
			}
			catch (Exception e)
			{
				ScreenManager.Console.AddException(e, "GiftingService.RejectGift() Failed");
			}
		}

		public IEnumerable<Gift> GetGifts()
		{
			try
			{
				return service.CheckGiftBox().Values;
			}
			catch (Exception e)
			{
				ScreenManager.Console.AddException(e, "GiftingService.GetGifts() Failed");

				return Array.Empty<Gift>();
			}
		}

		public void SetAcceptedGifts(Trait[] traits)
		{
			try
			{
				if (!traits.Any())
					service.CloseGiftBox();
				else
					service.OpenGiftBox(false, traits.Select(t => t.ToString()).ToArray());
			}
			catch (Exception e)
			{
				ScreenManager.Console.AddException(e, "GiftingService.SetAcceptedGifts() Failed");
			}
		}

		public Trait[] EnabledTraits()
		{
			try
			{
				var box = service.GetCurrentGiftboxState();
				if (box == null)
					return new Trait[0];

				return box.DesiredTraits
					.Select(t => (Trait)typeof(Trait).GetEnumValue(t))
					.ToArray();
			}
			catch (Exception e)
			{
				ScreenManager.Console.AddException(e, "GiftingService.EnabledTraits() Failed");

				return new Trait[0];
			}
		}
	}
}
