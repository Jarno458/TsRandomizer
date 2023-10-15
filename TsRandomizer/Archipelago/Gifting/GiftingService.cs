using System;
using System.Linq;
using TsRandomizer.Extensions;
using System.Collections.Generic;
using Archipelago.Gifting.Net.Gifts;
using Archipelago.Gifting.Net.Gifts.Versions.Current;
using Archipelago.Gifting.Net.Service;
using Archipelago.Gifting.Net.Traits;
using Archipelago.MultiClient.Net;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Screens;
using APGiftingService = Archipelago.Gifting.Net.Service.GiftingService;

namespace TsRandomizer.Archipelago.Gifting
{
	class GiftingService
	{
		static readonly string[] KnownTraits = Enum.GetNames(typeof(Trait));

		readonly IGiftingServiceSync service;

		public int NumberOfGifts {
			get;
#if DEBUG
			set;
#else
			private set;
#endif
		}

		public GiftingService(ArchipelagoSession session)
		{
			service = new APGiftingService(session);
			service.SubscribeToNewGifts(OnGiftsChanged);
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
					var team = Client.Team;
					var slot = acceptTraitsForPlayer.Key;

					var playerInfo = Client.GetPlayerInfo(team, slot);
					var playerGame = playerInfo?.Game ?? "Unknown Game";
					var playerAlias = playerInfo?.Alias ?? $"Unknown Player {slot}";

					var acceptedTraits = new AcceptedTraits
					{
						Team = team,
						Slot = slot,
						Name = playerAlias,
						Game = playerGame,
						AcceptsAnyTrait = acceptTraitsForPlayer.Value.Count() == KnownTraits.Length,
						DesiredTraits = acceptTraitsForPlayer.Value
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

		public bool Send(InventoryItem item, AcceptedTraits playerInfo)
		{
			try
			{
				var giftItem = new GiftItem(item.Name, item.GetAmount(), 0);
				var traits = TraitMapping.ValuesPerItem[item]
					.Select(t => new GiftTrait(t.Key.ToString(), 1, t.Value))
					.ToArray();

				return service.SendGift(giftItem, traits, playerInfo.Name, playerInfo.Team);
			}
			catch (Exception e)
			{
				ScreenManager.Console.AddException(e, "GiftingService.Send() Failed");
			}

			return false;
		}

		public void AcceptGift(Gift gift)
		{
			try
			{
				service.RemoveGiftFromGiftBox(gift.ID);
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

				return new Gift[0];
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
				var giftMotherBoxes = Client.DataStorage[$"GiftBoxes;{Client.Team}"].To<Dictionary<int, GiftBox>>();

				if (!giftMotherBoxes.TryGetValue(Client.Slot, out var giftBox))
					return new Trait[0];

				return giftBox.DesiredTraits
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
