namespace Application.Interfaces.IApiClient.Redis
{
    public interface IRedisService
    {
        public T? Get<T>(string key);
        public Task Set<T>(string key, T value, int minutes);
        public void Remove(string key);

        public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken);
    }
}
