using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DatabasLabb3
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create connection with local server
            MongoClient mongoClient = new MongoClient("mongodb://localhost:27017"); 

            //Create reference to database "labb3"
            var db = mongoClient.GetDatabase("labb3"); 

            //If collection "restaurants" exists, drop it
            db.DropCollection("restaurants");

            //Create reference to collection "restaurants"
            var restaurants = db.GetCollection<Restaurant>("restaurants");

            InsertRestaurants(restaurants);

            PrintRestaurants(restaurants);
            PrintCafes(restaurants);
            
            IncrementXYZRating(restaurants);
            PrintRestaurants(restaurants);
            
            UpdateCookieName(restaurants);
            PrintRestaurants(restaurants);

            FindFourStarsAndAbove(restaurants);
        }

        private static void InsertRestaurants(IMongoCollection<Restaurant> restaurants)
        {
            //Create array of class Restaurant
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

            //Insert all Restaurants in collection
            restaurants.InsertMany(restaurantList);
        }

        private static void PrintRestaurants(IMongoCollection<Restaurant> restaurants)
        {
            Console.WriteLine("\nAll restaurants:\n");
            //Find all Restaurants in database collection, and print their name
            foreach (Restaurant restaurant in restaurants.Find(new BsonDocument()).ToEnumerable())
            {
                Console.WriteLine($"{restaurant.ToJson()}");
            }
        }
        private static void PrintCafes(IMongoCollection<Restaurant> restaurants)
        {
            Console.WriteLine("\nCafés:\n");
            //Create filter to find documents with "Cafe" in "categories"
            var filter = Builders<Restaurant>.Filter.Eq("categories", "Cafe");

            //Create projection to exclude "_id" and include "name"
            var projection = Builders<Restaurant>.Projection.Exclude("_id").Include("name");

            //Enumerate over all restaurants matching filter and project them using projection, print their name
            foreach (var restaurant in restaurants.Find(filter).Project(projection).ToEnumerable())
            {
                Console.WriteLine($"{restaurant.ToJson()}");
            }
        }

        private static void IncrementXYZRating(IMongoCollection<Restaurant> restaurants)
        {
            Console.WriteLine("\nIncrementing XYZ Coffee Bar stars by 1\n");

            //Create filter to find documents with "name" of "XYZ Coffee Bar"
            var filter = Builders<Restaurant>.Filter.Eq("name", "XYZ Coffee Bar");

            //Create action to update field "stars" by incrementing with value 1
            var action = Builders<Restaurant>.Update.Inc("stars", 1);

            //Perform action on one item matching filter
            restaurants.UpdateOne(filter, action);
        }

        private static void UpdateCookieName(IMongoCollection<Restaurant> restaurants)
        {
            Console.WriteLine("\nChanging name of \"456 Cookies Shop\" to \"123 Cookies Heaven:\"\n");

            //Create filter to find documents with "name" of "456 Cookies Shop"
            var filter = Builders<Restaurant>.Filter.Eq("name", "456 Cookies Shop");

            //Create action to set field "name" to "123 Cookies Heaven"
            var action = Builders<Restaurant>.Update.Set("name", "123 Cookies Heaven");

            //Perform action on one item matching filter
            restaurants.UpdateOne(filter, action);
        }

        private static void FindFourStarsAndAbove(IMongoCollection<Restaurant> restaurants)
        {
            // create matching aggregate stage to only include restaurants with a star rating of 4 or more

            Console.WriteLine("\nRestaurants with a star rating of 4 or more:\n");

            var match = new BsonDocument
            {
                { "$match",
                    new BsonDocument
                    {
                        { "stars",
                            new BsonDocument
                            {
                                { "$gte", 4 }
                            }
                        }
                    }
                }
            };

            // create projection aggregate stage to exclude field "_id" and include fields "name" and "stars"
            var project = new BsonDocument
            {
                { "$project", 
                    new BsonDocument
                    {
                        { "_id", 0 },
                        { "name", 1 },
                        { "stars", 1 }
                    } 
                }
            };

            // send array of aggregate pipeline stages to Aggregate, convert from cursor to enumerable to indented json
            var response = restaurants
                .Aggregate<BsonDocument>(new[] { match, project })
                .ToEnumerable()
                .ToJson(new JsonWriterSettings { Indent = true }
            );
            Console.WriteLine(response);
        }
    }

    /*
     * Restaurant class to represent a document in collection Restaurants
     * [BsonId] matches with _Id, [BsonElement("thing")] with field "thing" 
     */
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
