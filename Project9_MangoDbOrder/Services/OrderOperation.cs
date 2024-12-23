using MongoDB.Bson;
using MongoDB.Driver;
using Project9_MangoDbOrder.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project9_MangoDbOrder.Services
{
    public class OrderOperation
    {
        public void AddOrder(Order order)
        {
            var connection = new MongoDbConnection(); // baglantımın oldugu adrese git
            var orderCollection = connection.GetOrdersCollection();// sonra getorderscollenction isminde ki koleksiyona git

            var document = new BsonDocument // ardından atamaları yap
            {
                {"CustomerName", order.CustomerName },
                {"City", order.City },
                {"District", order.District },
                {"TotalPrice", order.TotalPrice },
            };

            orderCollection.InsertOne(document);  // ekleme işlemleri için ınsertOne
        }

        public List<Order> GetAllOrders()
        {
            var connection = new MongoDbConnection();
            var orderCollection = connection.GetOrdersCollection();

            var orders = orderCollection.Find(new BsonDocument()).ToList(); // bu metoda bi BsonDocument gonderilicek

            List<Order> orderList = new List<Order>(); // su anlık ici bos orderlist tanımladık

            foreach (var order in orders) // order isminde degisken bu degisken orderstan orderListe gecicek ordanda verileri alıyo
            {
                orderList.Add(new Order
                {
                    City = order["City"].ToString(),
                    CustomerName = order["CustomerName"].ToString(),
                    District = order["District"].ToString(),
                    OrderId = order["_id"].ToString(),
                    TotalPrice = order["TotalPrice"].AsDecimal
                });
            }
            return orderList;
        }

        public void DeleteOrder(string orderId)
        {
            var connection = new MongoDbConnection();
            var orderCollection = connection.GetOrdersCollection();
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(orderId)); // Eq ifadesi ıdnin eşit olup olmadıgına bakar // esit oldugu degeri bul 
            orderCollection.DeleteOne(filter);
        }

        public void UpdateOrder(Order order)
        {
            var connection = new MongoDbConnection();
            var orderCollection = connection.GetOrdersCollection();
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(order.OrderId));
            var updatedValue = Builders<BsonDocument>.Update
                .Set("CustomerName", order.CustomerName)
                .Set("District", order.District)
                .Set("City", order.City)
                .Set("TotalPrice", order.TotalPrice);

            orderCollection.UpdateOne(filter,updatedValue); 
        }

        public Order GetOrderById(string orderId)
        {
            var connection = new MongoDbConnection();
            var orderCollection = connection.GetOrdersCollection();

            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(orderId));
            var result = orderCollection.Find(filter).FirstOrDefault();
            if (result != null)
            {
                return new Order
                {
                    City = result["City"].ToString(),
                    CustomerName = result["CustomerName"].ToString(),
                    District = result["District"].ToString(),
                    TotalPrice = decimal.Parse(result["TotalPrice"].ToString()),
                };
            }
            else
            {
                return null;
            }
        }
    }
}
