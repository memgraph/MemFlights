using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;
using MemFlights.Models;

namespace MemFlights.Repository
{

    public interface IFlightsRepository
    {
        Task<D3Graph> FetchD3Graph(int limit);
    }
    public class FlightsRepository : IFlightsRepository
    {
        private readonly IDriver _driver;

        public FlightsRepository(IDriver driver) {
            _driver = driver;
        }
        public async Task<D3Graph> FetchD3Graph(int limit)
        {
            var session = _driver.AsyncSession(WithDatabase);
            try
            {
                return await session.ReadTransactionAsync(async transaction =>
                {
                    var cursor = await transaction.RunAsync(@"MATCH (origin:Airport)-[f:IS_FLYING_TO]->(dest:Airport) " +
                        "WITH origin, dest ORDER BY origin.name, dest.name " +
                        "RETURN origin.NAME AS origin_air, dest.NAME AS dest_air", new { limit });
                    var nodes = new List<D3Node>();
                    var links = new List<D3Link>();
                    var records = await cursor.ToListAsync();
                    foreach (var record in records)
                    {
                        var orgAirport = new D3Node(title: record["origin_air"].As<string>(), label: "airport");
                        var originAirportIndex = nodes.Count;
                        nodes.Add(orgAirport);
                        /*
                        foreach (var destination in record["dest_air"].As<IList<string>>())
                        {
                            var destAirport = new D3Node(destination, "airport");
                            var destAirportIndex = nodes.IndexOf(destAirport);
                            destAirportIndex = destAirportIndex == -1 ? nodes.Count : destAirportIndex;
                            nodes.Add(destAirport);
                            links.Add(new D3Link(destAirportIndex, originAirportIndex));
                        }
                        */
                        var destAirport = new D3Node(record["dest_air"].As<string>(), "airport");
                        var destAirportIndex = nodes.IndexOf(destAirport);
                        destAirportIndex = destAirportIndex == -1 ? nodes.Count : destAirportIndex;
                        nodes.Add(destAirport);
                        links.Add(new D3Link(destAirport.Title, orgAirport.Title));

                    }
                    return new D3Graph(nodes, links);

                });
            }
            finally {
                await session.CloseAsync();
            }
        }

        private void WithDatabase(SessionConfigBuilder sessionConfigBuilder)
        {
            sessionConfigBuilder.WithDatabase(Database());
        }

        private string Database()
        {
            return System.Environment.GetEnvironmentVariable("DATABASE") ?? "flights";
        }
    }
}
