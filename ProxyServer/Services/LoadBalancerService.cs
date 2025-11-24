namespace ProxyServer.Services
{
    public class LoadBalancerService
    {
        private readonly List<string> _servers;
        private int _index = 0;
        private readonly object _lock = new object();

        public LoadBalancerService(IConfiguration config)
        {
            _servers = config.GetSection("DWServers").Get<List<string>>() ?? new();
        }

        public string GetNextServer()
        {
            lock (_lock)
            {
                var server = _servers[_index];
                _index = (_index + 1) % _servers.Count;
                return server;
            }
        }
    }
}
