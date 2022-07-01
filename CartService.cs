﻿using System;
using Library.ECommerceApp.Models;
using Library.ECommerceApp.Utility;
using Newtonsoft.Json;

namespace Library.ECommerceApp.Services
{
    public class CartService
    {
        private ProductServices inventoryList;
        private ListNavigator<Product> listNavigator;
        private List<Product> cartProducts;
        private String CardInfo;
        private String CVV;
        private String ExpirationDate;
        private String StreetAddress;
        private String State;
        private String Zipcode;
        private int pagecount;


        private CartService()
        {
            cartProducts = new List<Product>();
            inventoryList = ProductServices.Current;
            CardInfo = "";
            CVV = "";
            ExpirationDate = "";
            StreetAddress = "";
            State = "";
            Zipcode = "";
            pagecount = 0;
            listNavigator = new ListNavigator<Product>(cartProducts);
        }

        public List<Product> Cart
        {
            get { return cartProducts; }
        }

        private static CartService? current;


        public static CartService Current   // singletons
        {
            get
            {
                if (current == null)
                {
                    current = new CartService();
                }
                return current;
            }
        }


        public int NextId
        {
            get
            {
                if (!Cart.Any())
                {
                    return 1;
                }
                // makes a list of Id # and gets max returns + 1 for next Id #
                return Cart.Select(p => p.Id).Max() + 1;
            }
        }


        public void Create(Product product)
        {
            //product.Id = NextId;
            if (product is ProductByWeight)
            {
                var productbyweight = product as ProductByWeight;
                if (productbyweight != null)
                    productbyweight.Weight = 0;
            }
            else
            {
                var productbyq = product as ProductByQuantity;
                if (productbyq != null)
                    productbyq.Quantity = 0;
            }
            cartProducts.Add(product);
        }



        public void Add(int id) // Adds item to cart
        {
            var prod = inventoryList.Products.FirstOrDefault(p => p.Id == id);  // prod = inventory (base)



            Console.WriteLine($"Adding {prod}"); // DELETE
            if (prod == null)
            {
                Console.WriteLine("Product was not found in Inventory.");
                return;
            }

            if (prod is ProductByQuantity)      // --------- Quantity ---------
            {
                var productbyq = prod as ProductByQuantity;         // productbyq -> derived from prodctquantity of prod

                if (productbyq != null) // product in inventory list exisits 
                {
                    // get quantity needed
                    Console.WriteLine("How many would you like? ");
                    var q = int.Parse(Console.ReadLine() ?? "0");

                    if (productbyq.Quantity < q)            // checks quantity avalible in productbyq
                    {
                        Console.WriteLine("Quantity unavaliable.");
                        return;
                    }
                    var cartprod = cartProducts.FirstOrDefault(p => p.Id == id);        // cartprod = shoppingcart (base)

                    productbyq.Quantity = productbyq.Quantity - q;  // subtract by q reuesteted
                    Console.WriteLine($"Inventory Quantity = {productbyq.Quantity}");

                    // UPDATES inventory list quantity by - q
                    // ----- inventoryList.Update(prod);            MAYBE NEEDEDDDDDDD


                    // ------------ SHOPPING CART ------------ 

                    if (cartprod != null)   // Add to quantity cart if aleady existing and add current quantity to new 
                    {
                        var cp = cartprod as ProductByQuantity;
                        if (cp != null)
                        {
                            cp.Quantity = cp.Quantity + q;
                        }
                        //Create(cartprod);

                    }
                    else
                    {              // prod not in cart yet not in cart yet
                        cartprod = new ProductByQuantity();
                        //var cp = cartprod as ProductByQuantity;
                        cartprod.Id = prod.Id;
                        cartprod.Name = prod.Name;
                        cartprod.Description = prod.Description;
                        cartprod.priceinp = prod.priceinp;

                        var cartp = cartprod as ProductByQuantity;
                        //var cartp = cartprod as ProductByQuantity;
                        if (cartp != null)
                        {
                            cartp.Quantity = q;
                            cartp.Price = productbyq.Price;
                            cartp.Bogo = productbyq.Bogo;
                            cartProducts.Add(cartprod);
                        }
                    }

                    Console.WriteLine($"Successfull Added {q} units of {prod.Name} to Cart \n");
                }

            }
            else if (prod is ProductByWeight)   // by Weight
            {
                var productbyw = prod as ProductByWeight;         // productbyq -> derived from ProductByWeight of prod

                if (productbyw != null) // product in inventory list exisits 
                {
                    // get weight needed
                    Console.WriteLine("How many lbs would you like? ");
                    var w = int.Parse(Console.ReadLine() ?? "0");

                    if (productbyw.Weight < w)            // checks weight avalible in productbyw
                    {
                        Console.WriteLine("Weight unavaliable.");
                        return;
                    }
                    var cartprod = cartProducts.FirstOrDefault(p => p.Id == id);        // cartprod = shoppingcart (base)

                    productbyw.Weight = productbyw.Weight - w;  // subtract by q reuesteted
                    Console.WriteLine($"Inventory Weight = {productbyw.Weight}");


                    // ------------ SHOPPING CART ------------ 

                    if (cartprod != null)   // Add to quantity cart if aleady existing and add current quantity to new 
                    {
                        var cp = cartprod as ProductByWeight;
                        if (cp != null)
                        {
                            cp.Weight = cp.Weight + w;
                        }
                        //Create(cartprod);

                    }
                    else
                    {              // prod not in cart yet not in cart yet
                        cartprod = new ProductByWeight();
                        //var cp = cartprod as ProductByWeight;
                        cartprod.Id = prod.Id;
                        cartprod.Name = prod.Name;
                        cartprod.Description = prod.Description;
                        cartprod.priceinp = prod.priceinp;

                        var cartp = cartprod as ProductByWeight;
                        //var cartp = cartprod as ProductByWeight;
                        if (cartp != null)
                        {
                            cartp.Weight = w;
                            cartp.Price = productbyw.Price;
                            cartProducts.Add(cartprod);
                        }
                    }

                    Console.WriteLine($"Successfull Added {w} lbs of {prod.Name} to Cart \n");
                }
            }
        }

        public void Remove(int id) // Adds item to cart
        {

            var cartprod = cartProducts.FirstOrDefault(p => p.Id == id);

            Console.WriteLine($"Removing..."); // DELETE
            if (cartprod == null)
            {
                Console.WriteLine("Product was not found in Shopping Cart.");
                return;
            }

            // get quantity needed

            // ---- Quantity ----
            if (cartprod is ProductByQuantity)
            {
                Console.Write("How many would you like to remove? ");

                var q = int.Parse(Console.ReadLine() ?? "0");
                var byquantity = cartprod as ProductByQuantity;
                if (byquantity != null)
                {

                    if (byquantity.Quantity < q)
                    {
                        Console.WriteLine("Quantity unavaliable.");
                        return;
                    }
                    byquantity.Quantity -= q;
                    if (byquantity.Quantity <= 0)
                    {
                        cartProducts.Remove(cartprod);
                    }

                    var iprod = inventoryList.Products.FirstOrDefault(p => p.Id == id);
                    int newq = q;
                    if (iprod != null)   // Remove from cart if aleady existing and add current quantity to new 
                    {
                        newq = byquantity.Quantity + q;

                        var p = new ProductByQuantity();
                        p.Id = iprod.Id;
                        p.Name = iprod.Name;
                        p.Price = byquantity.Price;
                        p.Description = iprod.Description;
                        p.Quantity = newq;
                        inventoryList.Update(p);

                        Console.WriteLine($"Successfull Removed {q} units of {iprod.Name} to Cart \n");
                    }
                }
            }
            else if(cartprod is ProductByWeight)        // Weight
            {
                Console.Write("How many Pounds would you like to remove? ");
                var lbs = double.Parse(Console.ReadLine() ?? "0");

                var byweight = cartprod as ProductByWeight;
                if (byweight != null)
                {
                    if (byweight.Weight < lbs)
                    {
                        Console.WriteLine("Quantity unavaliable.");
                        return;
                    }
                    byweight.Weight -= lbs;
                    if (byweight.Weight <= 0)
                    {
                        cartProducts.Remove(cartprod);
                    }

                    var iprod = inventoryList.Products.FirstOrDefault(p => p.Id == id);
                    var newq = lbs;
                    var inbyweight = iprod as ProductByWeight;
                    if (inbyweight != null)   // Remove from cart if aleady existing and add current quantity to new 
                    {
                        newq = inbyweight.Weight + lbs;
                        var p = new ProductByWeight();
                        if (iprod != null)
                        {
                            p.Id = iprod.Id;
                            p.Name = iprod.Name;
                            p.Price = inbyweight.Price;
                            p.Description = iprod.Description;
                            p.Weight = newq;
                            inventoryList.Update(p);
                        }
                    }
                    Console.WriteLine($"Successfull Removed {newq} lbs to Cart \n");
                }
            }
        }

        public void Load(string fileName)
        {
            var productsJson = File.ReadAllText(fileName);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            cartProducts = JsonConvert.DeserializeObject<List<Product>>(productsJson, settings) ?? new List<Product>();

        }

        public void Save(string fileName)
        {

            File.WriteAllText(fileName, String.Empty);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            string strJson = JsonConvert.SerializeObject(cartProducts, settings);
            File.WriteAllText(fileName, strJson);
        }

        public double getTotal(Product prod)
        {
            double t = 0;

            if (prod is ProductByQuantity)
            {
                var productby = prod as ProductByQuantity;
                if (productby != null)
                {
                    //Console.WriteLine($"\t{prod.Name}\t{prod.Id}\t{productby.Price}\t{productby.Quantity}");
                    return productby.getTotalPrice();
                }
            }
            else if (prod is ProductByWeight)
            {

                var productby = prod as ProductByWeight;
                if (productby != null)
                {
                    //Console.WriteLine($"\t{prod.Name}\t{prod.Id}\t{productby.Price}\t{productby.Quantity}");
                    return productby.getTotalPrice();
                }

            }
            return t;
        }

        public void Checkout()
        {
            Console.WriteLine($"-----------------------------------------------------------------");
            Console.WriteLine($"-----------------------  C H E C K O U T  -----------------------");
            Console.WriteLine($"-----------------------------------------------------------------");
            var TotalP = 0.0;
            Console.WriteLine("\n\tQTY\tNAME\tPRICE\t\tTOTAL\n");
            for (int i = 0; i < cartProducts.Count; i++)
            {

                var prod = cartProducts[i];
                
                if (prod is ProductByQuantity)
                {
                    var productby = prod as ProductByQuantity;
                    if (productby != null)
                    {
                        Console.WriteLine($"\t{productby.Quantity}\t{prod.Name}\t$ {productby.Price}\t\t$ {productby.getTotalPrice()}");
                        TotalP += productby.getTotalPrice();
                    }
                }
                else
                {
                    var productby = prod as ProductByWeight;
                    if (productby != null)
                    {
                        Console.WriteLine($"\t{productby.Weight} lbs\t{prod.Name}\t$ {productby.Price}\t\t$ {productby.getTotalPrice()}");
                        TotalP += productby.getTotalPrice();
                    }
                }
            }

            var subtotal = TotalP * 0.07;
            Console.WriteLine($"* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *");

            Console.WriteLine($"\tsubtotal:\t$ {TotalP}");
            Console.WriteLine($"\tsales tax:\t$ {subtotal}");
            Console.WriteLine($"\n\tTOTAL:\t$ {TotalP + subtotal}\n");


            // ------------------ PAYMENT ------------------
            // gets shipping info 

            Console.WriteLine($"\n-----------------------------------------------------------------");
            Console.WriteLine($"-----------------------   P A Y M E N T   -----------------------");
            Console.WriteLine($"-----------------------------------------------------------------");

            Console.Write("Please Enter Credit Card Number :  ");
            CardInfo = Console.ReadLine() ?? string.Empty;

            Console.Write("Please Enter CVV Number:  ");
            CVV = Console.ReadLine() ?? string.Empty;

            Console.Write("Please Enter Credit Expiration Date (MO/YR):  ");
            ExpirationDate = Console.ReadLine() ?? string.Empty;

            Console.Write("Please Enter Steet Address:  ");
            StreetAddress = Console.ReadLine() ?? string.Empty;

            Console.Write("Please Enter City & State (City, State):  ");
            State = Console.ReadLine() ?? string.Empty;

            Console.Write("Please Enter Zipcode:  ");
            Zipcode = Console.ReadLine() ?? string.Empty;

            Console.WriteLine($"\n--------------------------------------------------\n");

            Console.WriteLine($"Payment Successful.\n");

            Console.Write($"SHIPPING TO:\n{StreetAddress}\n{State} {Zipcode}\n");
        }




        // ------------------------ Navigator ------------------------


        public void Navigator()
        {
            Console.Write("How would you like your Inventory Sorted By:\n1) Name\n2) Total Price\nEnter choice: ");

            var sortType = int.Parse(Console.ReadLine() ?? "0");
            var plist = cartProducts.OrderBy(p => p.Name).ToList();
            if (sortType == 1)   // Name Sort
            {
                plist = cartProducts.OrderBy(p => p.Name).ToList();
            }
            else if (sortType == 2)  // Price Sort
            {
                // newcart = shoppingcart.Cart.OrderBy(q => shoppingcart.getTotal(q));
                plist = cartProducts.OrderBy(q => getTotal(q)).ToList();
            }
            else
            {
                Console.WriteLine("Invalid Sort Type Printing as is. ");
            }

            Console.WriteLine("Printing Shopping Cart... \n");

            bool cont = true;

            listNavigator = new ListNavigator<Product>(plist);

            if (Cart.Count <= 0) { Console.WriteLine("\n***** SHOPPING CART EMPTY *****\n"); }
            else
            {
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------  Shopping Cart  -------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------\n\n");
                Console.WriteLine($"--------------------------------------------------    Page 1    ---------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

                Console.WriteLine($"  ID#:\tName:\t\tPrice per unit:\t\t\tUnits:\t\t\tDescription:\tDEALS:\tTotal:");
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

                pagecount = 1;
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

                CurrentPage();
                while (cont)
                {
                    // ---- choices of what to do next ----
                    Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

                    Console.WriteLine("(1) Next   (2) Previous   (3) First   (4) Last   (5) Enter Page Number   (6) EXIT "); // Create
                    Console.Write("\nEnter choice: ");             // Exit
                    var input = int.Parse(Console.ReadLine() ?? "0");
                    Console.Clear();

                    // -- switch for whats next
                    switch (input)
                    {
                        case 1:         // Next
                            Next();
                            break;
                        case 2:         // Previous
                            Previous();
                            break;
                        case 3:         // First
                            First();
                            break;
                        case 4:         // Last
                            Last();
                            break;
                        case 5:         // Enter Page Number
                            Console.Write("\nEnter Page number: ");             // Exit
                            var pagenum = int.Parse(Console.ReadLine() ?? "0");
                            Find(pagenum);
                            break;
                        case 6:         // Exit
                            cont = false;
                            break;

                        default:
                            Console.WriteLine("Invalid Choice, Try Again.\n");
                            break;
                    }

                }
            }
        }


        public void CurrentPage()
        {
            Dictionary<int, Product> dictionary = new Dictionary<int, Product>(listNavigator.GetCurrentPage());

            // dictionary.Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(Console.WriteLine);
            dictionary.Select(i => $"{i.Key}: {i.Value}\t{getTotal(i.Value)}").ToList().ForEach(Console.WriteLine);

        }

        public void Next()
        {
            if (listNavigator.HasNextPage)
            {
                pagecount++;
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"--------------------------------------------------    Page {pagecount}     ---------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
                //Console.WriteLine($"  ID#:\tName:\t\tPrice per:\tUnits:\t\tTotal");
                Console.WriteLine($"  ID#:\tName:\t\tPrice per unit:\t\t\tUnits:\t\t\tDescription:\tDEALS:\tTotal:");
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

                Dictionary<int, Product> dictionary = new Dictionary<int, Product>(listNavigator.GoForward());
                //dictionary.Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(Console.WriteLine);

                dictionary.Select(i => $"{i.Key}: {i.Value}\t{getTotal(i.Value)}").ToList().ForEach(Console.WriteLine);
            }
            else
            {
                Console.WriteLine("No Next Page.");
            }

        }

        public void Previous()
        {
            if (listNavigator.HasPreviousPage)
            {
                pagecount--;
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"--------------------------------------------------    Page {pagecount}     ---------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"  ID#:\tName:\t\tPrice per unit:\t\t\tUnits:\t\t\tDescription:\tDEALS:\tTotal:");
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

                Dictionary<int, Product> dictionary = new Dictionary<int, Product>(listNavigator.GoBackward());

                dictionary.Select(i => $"{i.Key}: {i.Value}\t{getTotal(i.Value)}").ToList().ForEach(Console.WriteLine);
            }
            else
            {
                Console.WriteLine("No Previous Page.");
            }

        }

        public void Find(int pagenum)
        {
            if (pagenum <= cartProducts.Count / 5 || pagenum < 0)
            {
                pagecount = pagenum;
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"--------------------------------------------------    Page {pagecount}     ---------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

                Console.WriteLine($"  ID#:\tName:\t\tPrice per unit:\t\t\tUnits:\t\t\tDescription:\tDEALS:\tTotal:");
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

                Dictionary<int, Product> dictionary = new Dictionary<int, Product>(listNavigator.GoToPage(pagecount));

                dictionary.Select(i => $"{i.Key}: {i.Value}\t{getTotal(i.Value)}").ToList().ForEach(Console.WriteLine);
            }
            else
            {
                Console.WriteLine("No Such Page Number.");
            }
        }

        public void First()
        {
            pagecount = 1;
            Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"--------------------------------------------------    Page {pagecount}     ---------------------------------------------------");
            Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

            Console.WriteLine($"  ID#:\tName:\t\tPrice per unit:\t\t\tUnits:\t\t\tDescription:\tDEALS:\tTotal:");
            Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

            Dictionary<int, Product> dictionary = new Dictionary<int, Product>(listNavigator.GoToFirstPage());

            dictionary.Select(i => $"{i.Key}: {i.Value}\t{getTotal(i.Value)}").ToList().ForEach(Console.WriteLine);
        }

        //public Dictionary<int, T> LastPage()
        public void Last()
        {
            pagecount = cartProducts.Count / 5;
            Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"--------------------------------------------------    Page {pagecount}     ---------------------------------------------------");
            Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"  ID#:\tName:\t\tPrice per unit:\t\t\tUnits:\t\t\tDescription:\tDEALS:\tTotal:");
            Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

            Dictionary<int, Product> dictionary = new Dictionary<int, Product>(listNavigator.GoToLastPage());

            dictionary.Select(i => $"{i.Key}: {i.Value.Id}\t{i.Value}\t{getTotal(i.Value)}").ToList().ForEach(Console.WriteLine);
        }
    }
}

