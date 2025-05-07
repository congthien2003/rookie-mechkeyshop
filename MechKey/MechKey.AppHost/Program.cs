var builder = DistributedApplication.CreateBuilder(args);
/*
var cache = builder.AddRedis("redis");
var rabbitmq = builder.AddRabbitMQ("messaging");*/

builder.AddProject<Projects.Mechkey_Yarp>("ReverseProxy");

builder.AddProject<Projects.MechkeyShop>("App");

builder.AddProject<Projects.WebAPI>("Api");

builder.Build().Run();
