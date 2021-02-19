using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace MemFlights.Repository
{
    public class DatabaseConnection
    {
        public static void populateDatabase() {
            string[] text = File.ReadAllLines(@"./queries.txt");

            using (var session = DatabaseContext.dbDriver.Session()) {
                foreach (string line in text)
                {
                    var transaction = session.WriteTransaction(tx => {
                        var result = tx.Run(line);
                        return result.Single()[0].As<string>();
                    });
                }
            }
            


        }
    }
}
