using System;
using System.Collections.Generic;
using System.Text;

namespace NetLoc
{
	public class LocKey
	{
		public string Path;
		public string DefaultValue;

		public LocKey()
		{
		}

		public LocKey(string path, string defaultValue)
		{
			this.Path = path;
			this.DefaultValue = defaultValue;
		}

		public LocKey Clone()
		{
			return new LocKey(Path, DefaultValue);
		}
	}

	/// <summary>
	/// Attribute which is applied to a field or method or property to indicate that
	/// the LocKey should be found by reflection and included in the list of LocKeys
	/// to localize
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property |AttributeTargets.Enum,
			   AllowMultiple = false,
			   Inherited = true)]
	public class LocalizeAttribute : Attribute
	{
	}
}
