using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServiceLayer.RedisCache;
using StackExchange.Redis;

public class RedisCacheService : IRedisCacheService
{
	private readonly IDatabase _database;

	public RedisCacheService(IOptions<RedisCacheSettings> options)
	{
		var opts = ConfigurationOptions.Parse(options.Value.ConnectionString);
		var conn = ConnectionMultiplexer.Connect(opts);
		_database = conn.GetDatabase();
	}

	public async Task<T> GetAsync<T>(string key)
	{
		var value = await _database.StringGetAsync(key);
		return value.HasValue
			? JsonConvert.DeserializeObject<T>(value)
			: default;
	}

	public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
	{
		
		await _database.StringSetAsync(
			key,
			JsonConvert.SerializeObject(value),
			ttl.HasValue && ttl.Value > TimeSpan.Zero
				? ttl
				: (TimeSpan?)null
		);
	}
}
