﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.RedisCache
{
	public interface IRedisCacheService
	{
		Task<T> GetAsync<T>(string key);
		Task SetAsync<T>(string key, T value, TimeSpan? ttl = null);
	}
}
