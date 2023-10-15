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
			var acceptTraitsPerPlayer = service.GetAcceptedTraitsByPlayer(Client.Team, KnownTraits);
			var acceptedTraitsPerSlot = new List<AcceptedTraits>(acceptTraitsPerPlayer.Count);
			
			foreach (var acceptTraitsForPlayer in acceptTraitsPerPlayer)
			{
				var team = Client.Team;
				var slot = acceptTraitsForPlayer.Key;

				var playerInfo = Client.Players.Players[team][slot];

				var acceptedTraits = new AcceptedTraits {
					Team = team,
					Slot = slot,
					Name = playerInfo.Alias,
					Game = playerInfo.Game,
					AcceptsAnyTrait = acceptTraitsForPlayer.Value.Count() == KnownTraits.Length,
					DesiredTraits = acceptTraitsForPlayer.Value
						.Select(t => (Trait)Enum.Parse(typeof(Trait), t))
						.ToArray()
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

		public void SetAcceptedGifts(Trait[] traits)
		{
			if (!traits.Any())
				service.CloseGiftBox();
			else
				service.OpenGiftBox(false, traits.Select(t => t.ToString()).ToArray());
		}

		public Trait[] EnabledTraits()
		{

		}
	}
}
