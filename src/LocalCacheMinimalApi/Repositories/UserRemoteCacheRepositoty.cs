using System.Text.Json;

namespace LocalCacheMinimalApi.Repositories
{
    public class UserRemoteCacheRepositoty : IUserRepository
    {
        private readonly UserRepositoty userRepositoty;
        private readonly RedisCacheOperations cacheOperations;

        public UserRemoteCacheRepositoty(UserRepositoty userRepositoty, RedisCacheOperations cacheOperations)
        {
            this.userRepositoty = userRepositoty;
            this.cacheOperations = cacheOperations;
        }

        public async Task<User> GetById(string ID)
        {
            var key = $"appName/user/{ID}";
            
            var userFromCache = await cacheOperations.Get<User>(key);

            if (userFromCache is null)
            {
                var user = await userRepositoty.GetById(ID);
                user.SetSource("REMOTE_CACHE");
                await cacheOperations.Set(key, user);
                return user;
            }

            return userFromCache;
        }
    }
}
