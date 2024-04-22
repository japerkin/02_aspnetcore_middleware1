namespace _02_aspnetcore_middleware1;
/*
 * This example accomplishes several objectives:
 *
 * 1. Add a custom header to the response.
 * 2. Create two separate middleware components and successfully store and pass
 * values between the two using "context.Items".
 * 3. Create inline middleware components without having to create separate
 * classes per middleware component.
 * 
 */
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // The .Use method is used to add middleware components directly in the middleware pipeline
        // without defining separate classes.
        // 
        // The middleware is defined using a lambda function. Inside the lambda function, we add a custom header
        // to the response and then call the "next()" delegate to pass control to the next middleware component
        // in the pipeline.
        //
        // This approach is useful when you have simple middleware logic that doesn't warrant creating a separate
        // middleware class. However, for more complex middleware logic or reusability, it's generally better
        // to define a separate middleware class.
        app.Use(async (context, next) =>
        {
            // This solution will throw an ArgumentException when attempting to add a duplicate key.
            // To avoid the ArgumentException when attempting to add a duplicate header you can use
            // Append or directly set the header using the indexer.
            //context.Response.Headers.Add("CustomHeader2", "jacob-per");
            
            // Using "IHeaderDictionary.Append" ensures that if a header with the same key already exists,
            // the new value will be appended to the existing header value rather than throwing an exception.
            context.Response.Headers.Append("Custom-Header1", "Jacob-Perkins");
            await next(context);
        });
        
        // This inline middleware adds an entry to 'context.Items' and then passes control to the next
        // middleware component.
        app.Use(async (context, next) =>
        {
            context.Items["jacob"] = "perkins";
            await next(context);
        });
        
        // This inline middleware retrieves a value from "context.Items" in a variable named "value" with a datatype
        // var.
        //
        // "value as string" indicates that the variable named "value" contained a string value. If an int value
        // was passed a key value to "context.Items", then you would change this to "value as int".
        //
        // If you wanted to check the data type of the key, you could write the following code:
        //
        // var storedValue = value as int;
        // if (value is int storedValue) {
        //     Use the value as an int
        // }
        //
        app.Use(async (context, next) =>
        {
            if (context.Items.TryGetValue("jacob", out var value))
            {
                var storedValue = value as string;
                context.Response.Headers.Append("Jacob-Header", storedValue);
            }
            await next(context);
        });
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}