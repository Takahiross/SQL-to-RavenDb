using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;

namespace RavenSharp
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            var documentsStore = new DocumentStore() // Creating Document Store 
            {
                Urls = new[] { "" }, // Connection RavenDB
                Database = "Events" // Name Database 
            }.Initialize();

            IEnumerable<Eventos> data = await GetAllEventsFromSqlAsync();
            await BulkInsertEvents(data, documentsStore);
            Console.WriteLine("Ended the migration");
            Console.ReadKey();

        }

        // Simple function to insert data to RavenDB
        private static async Task BulkInsertEvents(IEnumerable<Eventos> data, IDocumentStore documentStore)
        {
            using (var operation = documentStore.BulkInsert())
            {
                foreach(var datas in data)
                {
                    await operation.StoreAsync(datas);
                }
            }
        }
        // Function to get data from sql.
        private static async Task<IEnumerable<Eventos>> GetAllEventsFromSqlAsync()
        {
            List<Eventos> data = new List<Eventos>();

            using (SqlConnection conn = new SqlConnection("Connection SQL"))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM (table name)", conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        data.Add(new Eventos
                        {
                            //Sequencia = reader.GetInt64(0),
                            //IdCliente = reader.GetInt64(1),
                            // Objects that you want.
                        }) ;
                    }
                }
            }
            Console.WriteLine($"Loaded all events from sql database. Total: {data.Count()}.");

            return data;
        }
    }
}
