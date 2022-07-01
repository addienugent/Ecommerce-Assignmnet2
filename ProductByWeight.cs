using System;
namespace Library.ECommerceApp.Models
{
	public partial class ProductByWeight : Product
	{
		
		public double Weight { get; set; }
		public double Price { get; set; }
		public double TotalPrice { get; set; }

		public ProductByWeight()
		{
			priceinp = Price;
		}
		public override string ToString()
		{
			//Weight.ToString("0.00");
			//Price.ToString("0.00");
			//TotalPrice.ToString("0.00");
			return $"{Id}\t{Name}\t\t$ {Price} per lb\t\t{Weight} lbs avalible\t{Description}\t\t";
			//return $"t$ {Price} per lbs\t {Weight} lbs avalible\t";
		}

		public double getTotalPrice()
        {
			TotalPrice = Weight * Price;

			return TotalPrice;

        }
	}
}

