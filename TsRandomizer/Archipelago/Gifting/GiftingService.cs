using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;


namespace TsRandomizer.Archipelago.Gifting
{
	class GiftingService
	{
		static readonly string[] KnownTraits = Enum.GetNames(typeof(Trait));

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
		}


		public List<AcceptedTraits> GetAcceptedTraits()
		{

			return null;
		}
		
		public bool Send(InventoryItem item, AcceptedTraits playerInfo)
		{
			return false;
		}
	}
}
