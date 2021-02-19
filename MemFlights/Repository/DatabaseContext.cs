using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace MemFlights.Repository
{
    public class DatabaseContext
    {
        public static IDriver dbDriver;

        public static void Register() {
            dbDriver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.None);
        } 
    }
}
