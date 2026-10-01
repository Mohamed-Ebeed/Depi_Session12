using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Depi_Session12.ListGenerator;

namespace Depi_Session12
{
	// Custom comparer: case-insensitive string comparison
	internal class CaseInsensitiveComparer : IComparer<string>
	{
		public int Compare(string? x, string? y)
			=> string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
	}

	internal class Program
	{
		static void Print<T>(string title, IEnumerable<T> items)
		{
			Console.WriteLine($"--- {title} ---");
			foreach (var item in items)
				Console.WriteLine(item);
			Console.WriteLine();
		}

		static void Print(string title, object? value)
		{
			Console.WriteLine($"--- {title} ---");
			Console.WriteLine(value is null ? "null" : value);
			Console.WriteLine();
		}

		static void Main()
		{
			string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
			int[] nums = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
			string[] fruits = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry" };
			var comparer = new CaseInsensitiveComparer();

			#region LINQ - Restriction Operators
			Console.WriteLine("===== Restriction Operators =====");

			// 1. Products that are out of stock
			Print("R1: Out of stock", ProductsList.Where(p => p.UnitsInStock == 0));

			// 2. In stock and cost more than 3.00
			Print("R2: In stock & price > 3.00",
				ProductsList.Where(p => p.UnitsInStock > 0 && p.UnitPrice > 3.00M));

			// 3. Digits whose name is shorter than their value (value = index in the array)
			Print("R3: Name shorter than value", digits.Where((d, i) => d.Length < i));
			#endregion

			#region LINQ - Element Operators
			Console.WriteLine("===== Element Operators =====");

			// 1. First product out of stock
			Print("E1: First out of stock", ProductsList.First(p => p.UnitsInStock == 0));

			// 2. First product with price > 1000, or null
			Print("E2: First price > 1000", ProductsList.FirstOrDefault(p => p.UnitPrice > 1000));

			// 3. Second number greater than 5
			Print("E3: Second number > 5", nums.Where(n => n > 5).Skip(1).First());
			#endregion

			#region LINQ - Aggregate Operators
			Console.WriteLine("===== Aggregate Operators =====");

			// 1. Count of odd numbers
			Print("A1: Odd count", nums.Count(n => n % 2 != 0));

			// 2. Customers and number of orders
			Print("A2: Customers & order counts",
				CustomersList.Select(c => new { c.CustomerName, OrdersCount = c.Orders.Length }));

			// 3. Categories and number of products
			Print("A3: Categories & product counts",
				ProductsList.GroupBy(p => p.Category)
							.Select(g => new { Category = g.Key, ProductsCount = g.Count() }));

			// 4. Total of the numbers
			Print("A4: Sum", nums.Sum());

			// 5-8. Dictionary statistics
			string[] words = File.ReadAllLines("dictionary_english.txt");
			Print("A5: Total characters", words.Sum(w => (long)w.Length));
			Print("A6: Shortest word length", words.Min(w => w.Length));
			Print("A7: Longest word length", words.Max(w => w.Length));
			Print("A8: Average word length", words.Average(w => w.Length));
			#endregion

			#region LINQ - Ordering Operators
			Console.WriteLine("===== Ordering Operators =====");

			// 1. Products by name
			Print("O1: Products by name", ProductsList.OrderBy(p => p.ProductName));

			// 2. Case-insensitive sort using a custom comparer
			Print("O2: Case-insensitive sort", fruits.OrderBy(w => w, comparer));

			// 3. Products by units in stock, highest to lowest
			Print("O3: Units in stock desc", ProductsList.OrderByDescending(p => p.UnitsInStock));

			// 4. Digits by name length, then alphabetically
			Print("O4: Length then name", digits.OrderBy(d => d.Length).ThenBy(d => d));

			// 5. Word length, then case-insensitive alphabetical
			Print("O5: Length then case-insensitive", fruits.OrderBy(w => w.Length).ThenBy(w => w, comparer));

			// 6. Products by category, then unit price descending
			Print("O6: Category then price desc",
				ProductsList.OrderBy(p => p.Category).ThenByDescending(p => p.UnitPrice));

			// 7. Word length, then case-insensitive descending
			Print("O7: Length then case-insensitive desc",
				fruits.OrderBy(w => w.Length).ThenByDescending(w => w, comparer));

			// 8. Digits whose second letter is 'i', reversed from original order
			Print("O8: Second letter 'i', reversed",
				digits.Where(d => d.Length > 1 && d[1] == 'i').Reverse());
			#endregion

			#region LINQ - Transformation Operators
			Console.WriteLine("===== Transformation Operators =====");

			// 1. Names of products
			Print("T1: Product names", ProductsList.Select(p => p.ProductName));

			// 2. Upper and lower versions of each word (anonymous type)
			string[] sample = { "aPPLE", "BlUeBeRrY", "cHeRry" };
			Print("T2: Upper & lower",
				sample.Select(w => new { Upper = w.ToUpper(), Lower = w.ToLower() }));

			// 3. Some product properties, UnitPrice renamed to Price
			Print("T3: Product projection",
				ProductsList.Select(p => new { p.ProductName, p.Category, Price = p.UnitPrice }));

			// 4. Does each number match its position?
			Print("T4: Number matches position",
				nums.Select((n, i) => $"{n}: {n == i}"));

			// 5. Pairs where a < b
			int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
			int[] numbersB = { 1, 3, 5, 7, 8 };
			Print("T5: Pairs (a < b)",
				from a in numbersA
				from b in numbersB
				where a < b
				select $"{a} is less than {b}");

			// 6. Orders with total < 500
			Print("T6: Orders total < 500",
				CustomersList.SelectMany(c => c.Orders).Where(o => o.Total < 500.00M));

			// 7. Orders made in 1998 or later
			Print("T7: Orders in 1998 or later",
				CustomersList.SelectMany(c => c.Orders).Where(o => o.OrderDate.Year >= 1998));
			#endregion
		}
	}
}
