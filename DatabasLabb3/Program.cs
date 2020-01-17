using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DatabasLabb3
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoClient mongoClient = new MongoClient("mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("labb3");
            var restaurants = db.GetCollection<Restaurant>("restaurants");

            InsertRestaurants(restaurants);
            PrintRestaurants(restaurants);
        }

        private static void PrintRestaurants(IMongoCollection<Restaurant> restaurants)
        {
            foreach(Restaurant restaurant in restaurants.Find(new BsonDocument()).ToEnumerable())
            {
                Console.WriteLine($"{restaurant.Name}");
            }
        }

        private static void InsertRestaurants(IMongoCollection<Restaurant> restaurants)
        {
            var restaurantList = new Restaurant[] {
                new Restaurant {
                    Name = "Sun Bakery Trattoria",
                    Stars = 4,
                    Categories = new List<string>() {"Pizza", "Pasta", "Italian", "Coffee", "Sandwiches"}
                },
                new Restaurant {
                    Name = "Blue Bagels Grill",
                    Stars = 3,
                    Categories = new List<string>() {"Bagels", "Cookies", "Sandwiches"}
                },
                new Restaurant {
                    Name = "Hot Bakery Cafe",
                    Stars = 4,
                    Categories = new List<string>() {"Bakery", "Cafe", "Coffee", "Dessert"}
                },
                new Restaurant {
                    Name = "XYZ Coffee Bar",
                    Stars = 5,
                    Categories = new List<string>() {"Coffee", "Cafe", "Bakery", "Chocolates"}
                },
                new Restaurant {
                    Name = "456 Cookies Shop",
                    Stars = 4,
                    Categories = new List<string>() {"Bakery", "Cookies", "Cake", "Coffee"}
                }
            };

            restaurants.InsertMany(restaurantList);
        }
    }
  
    class Restaurant
    {
        [BsonId]
        public ObjectId ID { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("stars")]
        public int Stars { get; set; }
        [BsonElement("categories")]
        public List<string> Categories { get; set; }
    }
}
