using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TsRandomizer.Settings
{
	[AttributeUsage(AttributeTargets.Property)]
	class JsonIgnoreDeserialize : Attribute
	{
	}

	public class JsonSettingsContractResolver : DefaultContractResolver
	{
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			// list the properties to ignore
			var propertiesToIgnore = type.GetProperties()
				.Where(x => x.GetCustomAttributes(true).OfType<JsonIgnoreDeserialize>().Any());

			// Build the properties list
			var properties = base.CreateProperties(type, memberSerialization);

			// only serialize properties that are not ignored
			properties = properties
				.Where(p => propertiesToIgnore.All(info => info.Name != p.UnderlyingName))
				.ToList();

			return properties;
		}
	}
}

