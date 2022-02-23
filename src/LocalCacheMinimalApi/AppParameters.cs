namespace LocalCacheMinimalApi
{
    public class AppParameters
    {
        public AppParameters()
        {
            InstanceId = $"{Environment.MachineName}-{Guid.NewGuid().ToString().Substring(0, 4)}";
        }

        public string InstanceId { get; }
    }
}
