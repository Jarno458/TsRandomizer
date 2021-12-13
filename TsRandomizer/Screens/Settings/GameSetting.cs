using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsRandomizer.Screens.Settings
{
	abstract class GameSetting
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public dynamic CurrentValue { get; set; }
		public dynamic DefaultValue { get; set; }

		void Reset()
		{
			CurrentValue = DefaultValue;
		}
		
	}
}
