var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Mechkey_Gateway>("ReverseProxy");

builder.AddProject<Projects.MechkeyShop>("App");

builder.AddProject<Projects.WebAPI>("Api");

builder.AddProject<Projects.Notifcation_Api>("notifcation-api");

builder.AddProject<Projects.Order_Api>("order-api");

builder.AddProject<Projects.Invoice_Service_API>("invoice-api");

builder.AddProject<Projects.GenAI_API>("gen-ai-api");

builder.Build().Run();
