﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
class Program
{
    static HttpClient client = new HttpClient();

    static void ShowProduct(User user)
    {
        Console.WriteLine($"Name: {user.Name}\tPrice: " +
            $"{product.Price}\tCategory: {product.Category}");
    }

    static async Task<Uri> CreateProductAsync(Product product)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "api/products", product);
        response.EnsureSuccessStatusCode();

        // return URI of the created resource.
        return response.Headers.Location;
    }

    static async Task<Product> GetProductAsync(string path)
    {
        Product product = null;
        HttpResponseMessage response = await client.GetAsync(path);
        if (response.IsSuccessStatusCode)
        {
            product = await response.Content.ReadAsAsync<Product>();
        }
        return product;
    }

    static async Task<Product> UpdateProductAsync(Product product)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync(
            $"api/products/{product.Id}", product);
        response.EnsureSuccessStatusCode();

        // Deserialize the updated product from the response body.
        product = await response.Content.ReadAsAsync<Product>();
        return product;
    }


    static void Main()
    {
        RunAsync().GetAwaiter().GetResult();
    }

    static async Task RunAsync()
    {
        // Update port # in the following line.
        client.BaseAddress = new Uri("http://localhost:64195/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        try
        {
            // Create a new product
            Product product = new Product
            {
                Name = "Gizmo",
                Price = 100,
                Category = "Widgets"
            };

            var url = await CreateProductAsync(product);
            Console.WriteLine($"Created at {url}");

            // Get the product
            product = await GetProductAsync(url.PathAndQuery);
            ShowProduct(product);

            // Update the product
            Console.WriteLine("Updating price...");
            product.Price = 80;
            await UpdateProductAsync(product);

            // Get the updated product
            product = await GetProductAsync(url.PathAndQuery);
            ShowProduct(product);

           

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.ReadLine();
    }
}