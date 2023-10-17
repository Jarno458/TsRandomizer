namespace TsRandomizer.Archipelago.Gifting
{
	class AcceptedTraits
	{
		public int Team { get; set; }
		public int Slot { get; set; }
		public string Name { get; set; }
		public string Game { get; set; }
		public bool AcceptsAnyTrait { get; set; }
		public Trait[] DesiredTraits { get; set; }
	}
}