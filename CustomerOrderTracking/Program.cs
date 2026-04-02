using CustomerOrderTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace CustomerOrderTracking
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== Customer Order Tracker (EF Core) ===");

            // Ensure DB is up-to-date with migrations
            using (var contextx = new TrackerContext())
            {
                try
                {
                    contextx.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Database migration failed.");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Make sure you ran:");
                    Console.WriteLine("  dotnet ef migrations add InitialCreate");
                    Console.WriteLine("  dotnet ef database update");
                    return;
                }
            }
            //UI logic inside a while true loop, no need to modify
            while (true)
            {
                PrintMenu();
                Console.Write("Choose an option: ");
                var choice = (Console.ReadLine() ?? "").Trim();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await AddCustomerInteractive();
                            break;
                        case "2":
                            await AddOrderInteractive();
                            break;
                        case "3":
                            await ViewOrdersInteractive();
                            break;
                        case "4":
                            await UpdateCustomerEmailInteractive();
                            break;
                        case "5":
                            await DeleteCustomerInteractive();
                            break;
                        case "6":
                            await DeleteOrderInteractive();
                            break;
                        case "7":
                            await ListCustomersInteractive();
                            break;
                        case "8":
                            await SearchOrdersAboveAmountInteractive();
                            break;
                        case "0":
                            Console.WriteLine("Goodbye!");
                            return;
                        default:
                            Console.WriteLine("Invalid option. Try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Catch-all so the menu doesn't crash
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine();
            }

            // Method to Print Menu, no need to Modify

            static void PrintMenu()
            {
                Console.WriteLine("""
    
    Menu
    1) Add Customer
    2) Add Order (by Customer ID)
    3) View Orders (with Customer names)
    4) Update Customer Email
    5) Delete Customer
    6) Delete Order
    7) List Customers (with order counts)
    8) Search for Orders above a set order total amount
    0) Exit
    """);
            }

            // ---------------- Features ----------------


            //extra addition :)
            static async Task SearchOrdersAboveAmountInteractive()
            {
                decimal minAmount = ReadDecimal("Show orders with total >= ");

                if (minAmount < 0)
                {
                    Console.WriteLine("Amount must be >= 0.");
                    return;
                }

                using var context = new TrackerContext();

                var orders = await context.Orders
                    .Include(o => o.Customer)
                    .Where(o => o.TotalAmount >= minAmount)
                    .OrderByDescending(o => o.TotalAmount)
                    .ToListAsync();

                Console.WriteLine($"\nOrders with totals >= {minAmount:C}:");

                if (orders.Count == 0)
                {
                    Console.WriteLine(" (none found)");
                    return;
                }

                foreach (var o in orders)
                {
                    var customerName = o.Customer?.Name ?? "Unknown";

                    Console.WriteLine(
                        $" - Order {o.OrderId}: {o.OrderDate:g}  {o.TotalAmount:C}  Customer: {customerName} (ID {o.CustomerId})"
                    );
                }
            }

            static async Task AddCustomerInteractive()
            {
                //Code to get User Input:
                Console.Write("Customer name: ");
                var name = (Console.ReadLine() ?? "").Trim();

                Console.Write("Customer email: ");
                var email = (Console.ReadLine() ?? "").Trim();

                // Validate Name and Email input
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Name is required.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                {
                    Console.WriteLine("A valid email is required.");
                    return;
                }

                //Create a new TrackerContext as a var
                using var context = new TrackerContext();
                if (await context.Customers.AnyAsync(c => c.Email == email))
                {
                    Console.WriteLine("A customer with that email already exists.");
                    return;
                }

                //Create a var for a customer
                var customer = new Customer                 {
                    Name = name,
                    Email = email
                };

                //Then Add the new customer to Customers
                context.Add(customer);

                //Save Changes
                await context.SaveChangesAsync();

                //Report to end user that the customer was added
                Console.WriteLine($"Added customer {customer.CustomerId}-{customer.Name}({customer.Email})");

            }

            static async Task AddOrderInteractive()
            {
                int customerId = ReadInt("Customer ID: ");
                decimal totalAmount = ReadDecimal("Order Total Amount: ");

                // Validation
                if (totalAmount < 0)
                {
                    Console.WriteLine("TotalAmount must be >= 0.");
                    return;
                }

                using var context = new TrackerContext();

                // Verify customer exists to prevent orphaned orders
                var customer = await context.Customers.FindAsync(customerId);
                if (customer is null)
                {
                    Console.WriteLine($"Customer with ID {customerId} not found. Cannot create order.");
                    return;
                }

                // Create the new order and add it using the TrackerContext

                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    CustomerId = customerId
                };

                context.Orders.Add(order);

                // Save the information
                await context.SaveChangesAsync();

                // Inform the end user
                Console.WriteLine($"Order added on {order.OrderDate}: Order ID: {order.OrderId} for customer {customer.Name} with ID {customerId} - Total: ${order.TotalAmount:0.00}");

            }

            static async Task ViewOrdersInteractive()
            {
                using var context = new TrackerContext();

                //Code to include customer information in our Orders query
                var orders = await context.Orders
                    .Include(o => o.Customer)
                    .OrderBy(o => o.OrderDate)
                    .ToListAsync();

                //Begins the readback and handles empty Orders table
                Console.WriteLine("\nOrders:");
                if (orders.Count == 0)
                {
                    Console.WriteLine(" (none)");
                    return;
                }
                // Print each order with customer info
                foreach (var o in orders)
                {
                    var customerName = o.Customer?.Name ?? "Unknown";
                    Console.WriteLine($" - {o.OrderId}: {o.OrderDate:g}  ${o.TotalAmount:0.00}  For Customer: {customerName} (ID {o.CustomerId})");
                }


            }

            static async Task UpdateCustomerEmailInteractive()
            {
                //Code to get some User Input and Validate the Email:
                int customerId = ReadInt("Customer ID: ");
                Console.Write("New email: ");
                var newEmail = (Console.ReadLine() ?? "").Trim();

                if (string.IsNullOrWhiteSpace(newEmail))
                {
                    Console.WriteLine("Email is required.");
                    return;
                }
                else if (newEmail.Contains(" ") || !newEmail.Contains("@"))
                {
                    Console.WriteLine("Please enter a valid email address.");
                    return;
                }

                using var context = new TrackerContext();
                var customer = await context.Customers.FindAsync(customerId);

                //Create validaiton for customer not found (is null)
                if (customer is null)
                {
                    Console.WriteLine($"Customer with ID {customerId} not found.");
                    return;
                }


                //Set the customer Email to the newEmail
                Console.WriteLine($"Changing {customer.Name}'s email...");
                customer.Email = newEmail;

                //Save the changes
                await context.SaveChangesAsync();


                //Update the end user in the console
                Console.WriteLine($"Customer {customer.CustomerId} email updated to {customer.Email}");

            }

            static async Task DeleteCustomerInteractive()
            {
                int customerId = ReadInt("Customer ID to delete: ");

                using var context = new TrackerContext();

                var customer = await context.Customers
                    .Include(c => c.Orders)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (customer is null)
                {
                    Console.WriteLine("Customer not found.");
                    return;
                }

                Console.WriteLine($"Deleting customer: {customer.CustomerId} - {customer.Name} ({customer.Email})");
                if (customer.Orders.Count > 0)
                    Console.WriteLine($"NOTE: This customer has {customer.Orders.Count} order(s).");

                //Force the end user to write YES before we delete the customer
                //If anything other than YES or yes comes in, report that it was cancelled
                Console.Write("Type YES to confirm deletion: ");
                var confirm = (Console.ReadLine() ?? "").Trim();

                if (!confirm.Equals("YES", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Deletion cancelled.");
                    return;
                }

                // This will succeed if cascade delete is configured.
                // If not, you may need to delete orders first.
                context.Customers.Remove(customer);
                await context.SaveChangesAsync();

                Console.WriteLine("Customer deleted.");
            }

            static async Task DeleteOrderInteractive()
            {
                int orderId = ReadInt("Order ID to delete: ");

                using var context = new TrackerContext();
                var order = await context.Orders.FindAsync(orderId);

                //Validate if the order is null and exit the oeration
                if (order is null)
                {
                    Console.WriteLine("Order not found.");
                    return;
                }

                //Remove from the Orders table and then save the changes
                context.Orders.Remove(order);
                await context.SaveChangesAsync();

                ///Report the result to the end user:
                Console.WriteLine($"Order {order.OrderId} deleted.");
            }

            //Freebie select statement to use as an example. No modification needed:
            static async Task ListCustomersInteractive()
            {
                using var context = new TrackerContext();

                var customers = await context.Customers
                    .Select(c => new
                    {
                        c.CustomerId,
                        c.Name,
                        c.Email,
                        OrderCount = c.Orders.Count,
                        TotalSpent = c.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0m
                    })
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                Console.WriteLine("\nCustomers:");
                if (customers.Count == 0)
                {
                    Console.WriteLine(" (none)");
                    return;
                }

                foreach (var c in customers)
                {
                    Console.WriteLine($" - {c.CustomerId}: {c.Name,-20} {c.Email,-25} | Orders: {c.OrderCount,2} |  Total Spent: ${c.TotalSpent:0.00}");
                }
            }

            //!!! Helper Methods to make reading input easer, no need to modify
            // ---------------- Input Helpers ----------------

            static int ReadInt(string prompt)
            {
                while (true)
                {
                    Console.Write(prompt);
                    var s = (Console.ReadLine() ?? "").Trim();

                    if (int.TryParse(s, out int value))
                        return value;

                    Console.WriteLine("Please enter a valid whole number.");
                }
            }

            static decimal ReadDecimal(string prompt)
            {
                while (true)
                {
                    Console.Write(prompt);
                    var s = (Console.ReadLine() ?? "").Trim();

                    // Allow both current culture and invariant (helps student machines)
                    if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal value) ||
                        decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                        return value;

                    Console.WriteLine("Please enter a valid number (example: 249.99).");
                }
            }
        }
    }
   }

    