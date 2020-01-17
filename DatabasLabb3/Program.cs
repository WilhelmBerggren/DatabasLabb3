using System;

using MongoDB.Driver;

namespace DatabasLabb3
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoClient mongoClient = new MongoClient("mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("test");
            Console.WriteLine("Hello World!");
        }
    }
}
