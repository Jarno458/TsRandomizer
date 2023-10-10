using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.Gifting.Net.Gifts;
using Archipelago.Gifting.Net.Gifts.Versions.Current;
using Archipelago.Gifting.Net.Service;
using Archipelago.Gifting.Net.Traits;
using Archipelago.MultiClient.Net;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using APGiftingService = Archipelago.Gifting.Net.Service.GiftingService;

namespace TsRandomizer.Archipelago.Gifting
{
	class GiftingService
	{
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
			var motherBoxes = Client.DataStorage[$"GiftBoxes;{Client.Team}"].To<Dictionary<int, GiftBox>>();
			var acceptedTraitsPerSlot = new List<AcceptedTraits>(motherBoxes.Count);

			foreach (var playerMotherBox in motherBoxes)
			{
				if (!playerMotherBox.Value.IsOpen)
					continue;

				var desiredTraits = playerMotherBox.Value.DesiredTraits
					.Where(t => Enum.IsDefined(typeof(Trait), t))
					.Select(t => (Trait)Enum.Parse(typeof(Trait), t))
					.ToArray();

				var playerInfo = Client.Players.Players[Client.Team][playerMotherBox.Key];

				var acceptedTraits = new AcceptedTraits
				{
					Team = Client.Team,
					Slot = playerMotherBox.Key,
					Name = playerInfo.Alias,
					Game = playerInfo.Game,
					AcceptsAnyTrait = playerMotherBox.Value.AcceptsAnyGift,
					DesiredTraits = desiredTraits
				};

				acceptedTraitsPerSlot.Add(acceptedTraits);
			}

			return acceptedTraitsPerSlot;
		}

		public bool Send(InventoryItem item, AcceptedTraits playerInfo)
		{
			var giftItem = new GiftItem(item.Name, item.GetAmount(), 0);
			var traits = TraitMapping.ValuesPerItem[item]
				.Select(t => new GiftTrait(t.Key.ToString(), 1, t.Value))
				.ToArray();

			return service.SendGift(giftItem, traits, playerInfo.Name, playerInfo.Team);
		}
	}
}
