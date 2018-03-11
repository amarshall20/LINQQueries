<Query Kind="Statements">
  <Connection>
    <ID>5fa387b5-8e14-4089-8988-1f600e9bcf26</ID>
    <Server>.</Server>
    <Database>GroceryList</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

//1. Create a product list which indicates what products are purchased by our customers and how many times that product was purchased. Order the list by most popular product by alphabetic description
var result1 = from x in Products
	orderby x.OrderLists.Count() descending
	select new{
	x.Description,
	TimesPurchased = x.OrderLists.Count()
	};
result1.Dump();

//2. We want a mailing list for a Valued Customers flyer that is being sent out. List the customer addresses for customers who have shopped at each store. List by the store. Include the store location as well as the customer's address. Do NOT include the customer name in the results.
var result2 = from x in Stores
	select new{
	x.Location,
	Clients = (from y in x.Orders
		group y by y.Customer into gCustomer
		select new{
		gCustomer.Key.Address,
		gCustomer.Key.City,
		gCustomer.Key.Province
		})
	};
result2.Dump();

//3. Create a Daily Sales per Store request for a specified month. Order stores by city by location. For Sales, show order date, number of orders, total sales without GST tax and total GST tax.
var month = 12;
var result3 = from x in Stores
	select new{
	x.City,
	x.Location,
	sales = from y in x.Orders
		where y.OrderDate.Month.Equals(month)
		group y by y.OrderDate into gDay
		select new{
			date = gDay.Key,
			numberoforders = gDay.Count(),
			productsales = (from s in gDay select s.SubTotal).Sum(),
			gst = (from g in gDay select g.GST).Sum()
		}
	};
result3.Dump();

//4. Print out all product items on a requested order (use Order #33). Group by Category and order by Product Description. You do not need to format money as this would be done at the presentation level. Use the QtyPicked in your calculations. Hint: You will need to using type casting (decimal). Use of the ternary operator will help.
var result4 = from x in OrderLists
	where x.OrderID.Equals(33)
	group x by x.Product.Category.Description into gCategory
	select new{
		Category = gCategory.Key,
		OrderProducts = from y in gCategory orderby y.Product.Description select new{
			Product = y.Product.Description,
			PickedQty = y.QtyPicked,
			Price = y.Price,
			Discount = y.Discount,
			Subtotal = y.Price*(decimal)y.QtyPicked,
			Tax = y.Product.Taxable ? y.Price*(decimal)0.05 : 0,
			ExtendedPrice = y.Price*(decimal)y.QtyPicked + (y.Product.Taxable ? y.Price*(decimal)0.05 : 0)
		}
	};
result4.Dump();

//5. Select all orders a picker has done on a particular week (Sunday through Saturday). List by picker and order by picker and date. Hint: you will need to use the join operator.
var startDate = 17;
var endDate = 23;
var result5 = from x in Orders
	where ((DateTime)x.PickedDate).Day >= startDate && ((DateTime)x.PickedDate).Day <= endDate
	group x by x.PickerID into gPicker
	join y in Pickers on gPicker.Key equals y.PickerID
	orderby y.LastName
	select new{
		picker = y.LastName + ", " + y.FirstName,
		pickdates = from d in gPicker orderby d.PickedDate select new{
			ID = d.OrderID,
			Date = d.PickedDate
		}
	};
result5.Dump();

//6. List all the products a customer (use Customer #1) has purchased and the number of times the product was purchased. Order by number of times purchased then description
var result6 = from x in Customers
	where x.CustomerID.Equals(1)
	select new{
		Customer = x.LastName + ", " + x.FirstName,
		OrdersCount = x.Orders.Count(),
		Items = from y in OrderLists where y.Order.CustomerID.Equals(x.CustomerID)
			group y by y.Product into gProduct
			orderby gProduct.Count() descending, gProduct.Key.Description
			select new{
			description = gProduct.Key.Description,
			timesbought = gProduct.Count()
		}
	};
result6.Dump();