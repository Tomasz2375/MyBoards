using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyBoards.Entities;
using System.Text.Json.Serialization;

namespace MyBoards
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
            builder.Services.AddDbContext<MyBoardsContext>(
                option => option.UseSqlServer
                (
                    builder.Configuration.GetConnectionString("MyBoardsConnectionString"))
                );


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<MyBoardsContext>();

            var pendingMigrations = dbContext.Database.GetPendingMigrations();
            if(pendingMigrations.Any())
            {
                dbContext.Database.Migrate();
            }
            var users = dbContext.Users.ToList();
            if(!users.Any())
            {
                var user1 = new User()
                {
                    Email = "user1@test.com",
                    FullName = "User One",
                    Address = new Address()
                    {
                        City = "Warszawa",
                        Street = "Szeroka"
                    }
                };
                var user2 = new User()
                {
                    Email = "user2@test.com",
                    FullName = "User Two",
                    Address = new Address()
                    {
                        City = "Kraków",
                        Street = "D³uga"
                    }
                };
                dbContext.Users.AddRange(user1, user2);
                dbContext.SaveChanges();
            }
            app.MapGet("data", async (MyBoardsContext db) =>
            {
                var states = db.WorkItemStates
                .AsNoTracking()
                .ToList();


                db.SaveChanges();

                return states;
            });

            app.MapPost("update", async (MyBoardsContext db) =>
            {
                //Epic epic = await db.Epics.FirstAsync(epic => epic.Id == 1);
                //epic.Area = "Updated area";
                //epic.Priority = 1;
                //epic.StartDate= DateTime.Now;
                
                Epic epic = await db.Epics.FirstAsync(epic => epic.Id == 1);

                var rejectedState = await db.WorkItemStates.FirstAsync(a => a.State == "Rejected");

                epic.State = rejectedState;

                await db.SaveChangesAsync();

                return epic;

            });

            app.MapPost("create", async (MyBoardsContext db) =>
            {
                Tag mvctag = new Tag()
                {
                    Value = "MVC"
                };
                Tag asptag = new Tag()
                {
                    Value = "ASP"
                };
                var tags = new List<Tag>() { mvctag, asptag };
                //await db.AddAsync(tag);
                await db.Tags.AddRangeAsync(tags);
                //***************
                var address = new Address()
                {
                    Id = Guid.Parse("b323dd7c-776a-4cf6-a92a-12df154b4a2c"),
                    City = "Kraków",
                    Country = "Poland",
                    Street = "D³uga",
                };

                var user = new User()
                {
                    Email = "user@test.com",
                    FullName = "Test User",
                    Address = address,
                };
                db.Users.Add(user);
                //******************
                await db.SaveChangesAsync();

                return tags;
                
            });

            app.MapDelete("delete", async (MyBoardsContext db) => 
            {
                var user = await db.Users
                .Include(u => u.Comments)
                .FirstAsync(u => u.Id == Guid.Parse("4EBB526D-2196-41E1-CBDA-08DA10AB0E61"));

                db.Users.RemoveRange(user);
                await db.SaveChangesAsync();


            });

            app.Run();
        }
    }
}