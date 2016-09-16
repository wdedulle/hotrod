using Google.Protobuf;
using Infinispan.HotRod;
using Infinispan.HotRod.Config;
using Org.Infinispan.Protostream;
using Org.Infinispan.Query.Remote.Client;
using Dulle.Education.Hotrod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dulle.Education.Hotrod
{
    class Program
    {
        static void Main(string[] args)
        {
            RemoteCacheManager remoteManager;
            const string ERRORS_KEY_SUFFIX = ".errors";
            const string PROTOBUF_METADATA_CACHE_NAME = "___protobuf_metadata";

            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddServer()
                  .Host(args.Length > 1 ? args[0] : "127.0.0.1")
                  .Port(args.Length > 2 ? int.Parse(args[1]) : 11222);

            builder.Marshaller(new BasicTypesProtoStreamMarshaller());
            remoteManager = new RemoteCacheManager(builder.Build(), true);

            IRemoteCache<String, String> metadataCache = remoteManager.GetCache<String, String>(PROTOBUF_METADATA_CACHE_NAME);
            IRemoteCache<int, Person> testCache = remoteManager.GetCache<int, Person>("namedCache");

            // Installing the entities model into the Infinispan __protobuf_metadata cache 
            metadataCache.Put("sample_person/person.proto", File.ReadAllText("../../resources/proto2/person.proto"));

            // Console.WriteLine(File.ReadAllText("../../resources/proto2/person.proto"));
            if (metadataCache.ContainsKey(ERRORS_KEY_SUFFIX))
            {
                Console.WriteLine("fail: error in registering .proto model");
                Environment.Exit(-1);
            }

            testCache.Clear();
            // Fill the application cache
            putPersons(testCache);

            // Run a query
            QueryRequest qr = new QueryRequest();
            qr.JpqlString = "from quickstart.Person where surname like '%ou%'";
            QueryResponse result = testCache.Query(qr);


            List<Person> listOfUsers = new List<Person>();


            unwrapResults(result, listOfUsers);
            Console.WriteLine("There are " + listOfUsers.Count + " Users:");

            foreach (Person user in listOfUsers)
            {
                Console.WriteLine(user.ToString());
                System.Threading.Thread.Sleep(1000);
            }
            System.Threading.Thread.Sleep(5000);

        }
        private static void putPersons(IRemoteCache<int, Person> cache)
        {
            Console.WriteLine(">>> Loading persons");
            string[] firstNames = File.ReadAllLines(@"..\..\First_Names.csv", Encoding.UTF8);
            string[] lastNames = File.ReadAllLines(@"..\..\Last_Names.csv", Encoding.UTF8);
            int key = 1;

            foreach (string lastName in lastNames)
            {
                foreach (string firstName in firstNames)
                {
                    Random randomAge = new Random();

                    cache.Put(key, new Person { Id = key, FirstName = firstName, Surname = lastName, Age = randomAge.Next(120) });

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
        // Convert Protobuf matter into C# objects
        private static bool unwrapResults<T>(QueryResponse resp, List<T> res) where T : IMessage<T>
        {
            if (resp.ProjectionSize > 0)
            {  // Query has select
                return false;
            }
            for (int i = 0; i < resp.NumResults; i++)
            {
                WrappedMessage wm = resp.Results.ElementAt(i);

                if (wm.WrappedBytes != null)
                {
                    WrappedMessage wmr = WrappedMessage.Parser.ParseFrom(wm.WrappedBytes);

                    if (wmr.WrappedMessageBytes != null)
                    {

                        System.Reflection.PropertyInfo pi = typeof(T).GetProperty("Parser");

                        MessageParser<T> p = (MessageParser<T>)pi.GetValue(null);
                        T u = p.ParseFrom(wmr.WrappedMessageBytes);
                        res.Add(u);
                    }
                }
            }
            return true;
        }
    }
}
