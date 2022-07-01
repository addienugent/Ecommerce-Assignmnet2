using System;
//using Library.ECommerceApp.Interfaces;

namespace Library.ECommerceApp
{

    //public class Product

    public class Product
    {
    
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        //public double Price { get; }
        //public int Quantity { get; set; }
        //public double TotalPrice { get; set; }
        //public  string Type { get; }
        public int AssignedUser { get; set; }
        public double priceinp;

        public Product()
        {

        }

        public override string ToString()
        {
            return $"{Id}\t{Name}\t \t{Description}"; // { Price}\t" ; //{Quantity}\t\t{Description}"
        }
    }
}   