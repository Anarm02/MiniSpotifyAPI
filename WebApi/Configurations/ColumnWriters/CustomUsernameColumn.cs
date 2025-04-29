using Serilog.Core;
using Serilog.Events;

namespace WebApi.Configurations.ColumnWriters
{
	public class CustomUsernameColumn : ILogEventEnricher
	{
		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
			var (username, value) = logEvent.Properties.FirstOrDefault(x => x.Key == "UserName");

			if (value != null)

			{

				var getValue = propertyFactory.CreateProperty(username, value);

				logEvent.AddPropertyIfAbsent(getValue);

			}

		}
	}
}
