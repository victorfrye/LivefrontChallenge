var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("db");

builder.AddProject<Projects.WebApi>("api")
       .WithReference(db)
       .WaitFor(db);

await builder.Build().RunAsync();
