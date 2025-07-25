using System;
using System.Collections.Generic;
using Microsoft.Applications.Events.DataModels;
using PlayFab.EventsModels;

namespace PlayFab
{
	public class PlayFabEvent : IPlayFabEvent
	{
		public PlayFabEventType EventType { get; set; }

		public string Name
		{
			get
			{
				return EventContents.Name;
			}
			set
			{
				EventContents.Name = value;
			}
		}

		internal EventContents EventContents { get; private set; }

		public PlayFabEvent()
		{
			EventType = PlayFabEventType.Default;
			EventContents = new EventContents
			{
				EventNamespace = "com.playfab.events.default",
				Payload = new Microsoft.Applications.Events.DataModels.Data()
			};
		}

		public void SetProperty(string name, string value)
		{
			Value value2 = new Value
			{
				type = ValueKind.ValueString,
				stringValue = value
			};
			((Microsoft.Applications.Events.DataModels.Data)EventContents.Payload).properties[name] = value2;
		}

		public void SetProperty(string name, bool value)
		{
			Value value2 = new Value
			{
				type = ValueKind.ValueBool,
				longValue = (value ? 1 : 0)
			};
			((Microsoft.Applications.Events.DataModels.Data)EventContents.Payload).properties[name] = value2;
		}

		public void SetProperty(string name, DateTime value)
		{
			Value value2 = new Value
			{
				type = ValueKind.ValueDateTime,
				longValue = value.ToUniversalTime().Ticks
			};
			((Microsoft.Applications.Events.DataModels.Data)EventContents.Payload).properties[name] = value2;
		}

		public void SetProperty(string name, long value)
		{
			Value value2 = new Value
			{
				type = ValueKind.ValueInt64,
				longValue = value
			};
			((Microsoft.Applications.Events.DataModels.Data)EventContents.Payload).properties[name] = value2;
		}

		public void SetProperty(string name, double value)
		{
			Value value2 = new Value
			{
				type = ValueKind.ValueDouble,
				doubleValue = value
			};
			((Microsoft.Applications.Events.DataModels.Data)EventContents.Payload).properties[name] = value2;
		}

		public void SetProperty(string name, Guid value)
		{
			Value value2 = new Value
			{
				type = ValueKind.ValueGuid,
				guidValue = new List<List<byte>>
				{
					new List<byte>(value.ToByteArray())
				}
			};
			((Microsoft.Applications.Events.DataModels.Data)EventContents.Payload).properties[name] = value2;
		}
	}
}
