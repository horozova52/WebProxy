using Cassandra;

namespace DWServer.Services
{
    public class CassandraService
    {
        public Cassandra.ISession Session { get; }


        public CassandraService(IConfiguration config)
        {
            var cassCfg = config.GetSection("Cassandra");

            var cluster = Cluster.Builder()
                .AddContactPoints(cassCfg.GetSection("ContactPoints").Get<string[]>())
                .WithPort(cassCfg.GetValue<int>("Port"))
                .Build();

            Session = cluster.Connect(cassCfg.GetValue<string>("Keyspace"));
        }
    }
}
