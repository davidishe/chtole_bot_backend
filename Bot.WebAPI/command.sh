dotnet ef database drop -c AppDbContext
dotnet ef migrations add mew_migration_data_5 -c AppDbContext --project /Users/akobiyada/Documents/SpecialProjects/10_Bot/chtole_bot_backend/Bot.Infrastructure.Database/Bot.Infrastructure.Database.csproj
dotnet ef database update -c AppDbContext --project /Users/akobiyada/Documents/SpecialProjects/10_Bot/chtole_bot_backend/Bot.Infrastructure.Database/Bot.Infrastructure.Database.csproj
dotnet ef migrations script -c AppDbContext  --project  /Users/akobiyada/Documents/SpecialProjects/10_Bot/chtole_bot_backend/Bot.Infrastructure.Database/Bot.Infrastructure.Database.csproj

dotnet ef database drop -c IdentityContext
dotnet ef migrations add mew_migration_identity_4 -c IdentityContext --project /Users/akobiyada/Documents/SpecialProjects/10_Bot/chtole_bot_backend/Bot.Identity.Database/Bot.Identity.Database.csproj
dotnet ef database update -c IdentityContext --project /Users/akobiyada/Documents/SpecialProjects/10_Bot/chtole_bot_backend/Bot.Identity.Database/Bot.Identity.Database.csproj
dotnet ef migrations script -c IdentityContext --project /Users/akobiyada/Documents/SpecialProjects/10_Bot/chtole_bot_backend/Bot.Identity.Database/Bot.Identity.Database.csproj