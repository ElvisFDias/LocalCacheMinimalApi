using StackExchange.Redis;
using System.Text.Json;

namespace LocalCacheMinimalApi.Repositories
{
    public class RedisCacheOperations
    {
        private readonly ILogger<RedisCacheOperations> logger;

        public RedisCacheOperations(ILogger<RedisCacheOperations> logger)
        {
            this.logger = logger;
        }

        public async Task<T?> Get<T>(string key)
        {
            try
            {
                var cacheValue = await RedisCacheOperations.Connection.GetDatabase().StringGetAsync(key);

                if(string.IsNullOrEmpty(cacheValue))
                {
                    logger.LogInformation("Key {key} was not found on cache", key);
                    return default(T);
                }
                    
                logger.LogInformation("Key {key} was found on cache", key);
                
                return JsonSerializer.Deserialize<T>(cacheValue);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "error finding key {key} on cache", key);
                return default(T);
            }
        }

        public async Task Set<T>(string key, T content)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(content);
                await RedisCacheOperations.Connection.GetDatabase().StringSetAsync(key, jsonContent, TimeSpan.FromMinutes(60));

                logger.LogInformation("Key {key} was saved on cache", key);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "error saving key {key} on cache", key);
            }
        }

    }

    public class RedisCacheOperations
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = CreateConnection();

        private static Lazy<ConnectionMultiplexer> CreateConnection()
        {
            return new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheConnection = "localhost";
                return ConnectionMultiplexer.Connect(cacheConnection);
            });
        }

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}
