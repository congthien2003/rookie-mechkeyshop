var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Mechkey_Yarp>("ReverseProxy");

builder.AddProject<Projects.MechkeyShop>("App");

builder.AddProject<Projects.WebAPI>("Api");

builder.Build().Run();
