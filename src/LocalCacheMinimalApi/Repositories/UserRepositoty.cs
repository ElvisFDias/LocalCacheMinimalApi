using System.Text.Json;

namespace LocalCacheMinimalApi.Repositories
{
    public class UserRepositoty : IUserRepository
    {
        private readonly List<User> usersDb;
        
        public UserRepositoty()
        {
            var jsonDatabase = File.ReadAllText(GetDatabaseFilePath());
            usersDb = JsonSerializer.Deserialize<List<User>>(jsonDatabase, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? new List<User>();
        }

        private string GetDatabaseFilePath()
        {
            return $"{Environment.CurrentDirectory}/FakeData/fake-data-client.json";
        }
        public async Task<User> GetById(string ID)
        {
            return usersDb.FirstOrDefault(x => x.ID == ID);
        }
    }


    public interface IUserRepository
    {
        Task<User> GetById(string ID);
    }

    public class User
    {
        public void SetSource(string source)
        {
            this.Source = source;
        }

        public string Source { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string PostalZip { get; set; }
        public string Text { get; set; }
        public int NumberRange { get; set; }
        public string Currency { get; set; }
        public string Alphanumeric { get; set; }
        public string List { get; set; }
        public string Tags { get; set; }
        public string FavoriteCarsBrands { get; set; }
        public string FavoriteFoods { get; set; }
        public string ID { get; set; }
        public string Computed { get; set; }
    }
}
