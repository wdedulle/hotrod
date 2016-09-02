using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinispan.HotRod.Impl;
using Infinispan.HotRod.Config;
using Infinispan.HotRod;
using Org.Infinispan.Query.Remote.Client;
using System.IO;

namespace Dulle.Education.Hotrod
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddServer()                                
                .Host(args.Length > 1 ? args[0] : "127.0.0.1")
                .Port(args.Length > 2 ? int.Parse(args[1]) : 11222)                
                .ValueSizeEstimate(1073741824);

            
            Configuration config = builder.Build();
            
            RemoteCacheManager cacheManager = new RemoteCacheManager(config);
            
            if (cacheManager.IsStarted())
            {
                IRemoteCache<Int64, Person> cache = cacheManager.GetCache<Int64, Person>();
                
                putPersons(cache);

                QueryRequest queryRequest = new QueryRequest();
                queryRequest.JpqlString = "from Person where firstName like '%udp%'";
                QueryResponse queryRespone = cache.Query(queryRequest);
                Console.WriteLine(queryRespone.NumResults);

            }
            Console.ReadKey();
        }

        private static void putPersons(IRemoteCache<long, Person> cache)
        {
            Console.WriteLine(">>> Loading persons");
            string[] firstNames = File.ReadAllLines(@"..\..\First_Names.csv", Encoding.UTF8);
            string[] lastNames = File.ReadAllLines(@"..\..\Last_Names.csv", Encoding.UTF8);
            int key = 0;

            foreach (string lastName in lastNames)
            {
                foreach (string firstName in firstNames)
                {
                    Random randomAge = new Random();

                    //IDictionary<Int64, Person> persons= new Dictionary<Int64,Person>();
                    //persons.Add(key, new Person { Name = lastName, Age = randomAge.Next(120), FirstName = firstName });
                    cache.Put(key, new Person { Name = lastName, Age = randomAge.Next(120), FirstName = firstName });                        
                    
                    key++;
                    if ((key % 100000) == 0)
                    {
                        Console.WriteLine("  " + key);
                        
                        if (key == 1000000)
                        {
                            return;
                        }
                    }
                }
            }
            Console.WriteLine(">>> Loading persons DONE");
        }
    }
}
