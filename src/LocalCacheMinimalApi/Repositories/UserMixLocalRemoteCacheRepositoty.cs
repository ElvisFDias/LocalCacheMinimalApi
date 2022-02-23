using Microsoft.Extensions.Caching.Memory;

namespace LocalCacheMinimalApi.Repositories
{
    public class UserMixLocalRemoteCacheRepositoty : IUserRepository
    {
        private UserRepositoty userRepositoty;
        private RedisCacheOperations cacheOperations;
        private readonly AppParameters appParameters;
        private readonly IMemoryCache memoryCache;

        public UserMixLocalRemoteCacheRepositoty(
            UserRepositoty userRepositoty, 
            RedisCacheOperations cacheOperations, 
            AppParameters appParameters,
            IMemoryCache memoryCache)
        {
            this.userRepositoty = userRepositoty;
            this.cacheOperations = cacheOperations;
            this.appParameters = appParameters;
            this.memoryCache = memoryCache;
        } 

        public async Task<User> GetById(string ID)
        {
            var key = $"appName/{appParameters.InstanceId}/user/{ID}";
            var lastSynchronizedIsValidKey = $"appName/{appParameters.InstanceId}/user/{ID}/lastValidation";

            var userFromMemory = memoryCache.Get<User>(key);
            var lastSynchronizedIsValid = memoryCache.Get<bool>(lastSynchronizedIsValidKey);

            if(userFromMemory != null)
            {

                if(lastSynchronizedIsValid)
                    return userFromMemory;

                var localCacheIsValid = await cacheOperations.Get<bool>(key);
                if (localCacheIsValid)
                {
                    memoryCache.Set(lastSynchronizedIsValidKey, true, TimeSpan.FromMinutes(1));
                    return userFromMemory;
                }
            }
                       
            var userFromDb = await userRepositoty.GetById(ID);

            userFromDb.SetSource("LOCAL_CACHE");
            memoryCache.Set(key, userFromDb);
            memoryCache.Set(lastSynchronizedIsValidKey, true, TimeSpan.FromMinutes(1));
            await cacheOperations.Set(key, true);
            return userFromDb;
        }
    }
}
