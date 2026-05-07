using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Votex.DataAccess.Models;

namespace Votex.DataAccess
{
    public static class DbInitializer
    {
        public static void Migrate(VotexDbContext context)
        {
            // won't migrate if its an InMemory database
            if (context.Database.IsRelational())
                context.Database.Migrate();
        }
        
        public static void Initialize(VotexDbContext context, UserManager<User> userManager)
        {
            Migrate(context);

            if (context.Users.Any())
                return;

            // Create admin user
            var admin = new User
            {
                UserName = "admin@votex.com",
                Email = "admin@votex.com",
                EmailConfirmed = true,
                RefreshToken = Guid.NewGuid()
            };
            userManager.CreateAsync(admin, "admin1");


            // Create normal application users
            var users = new List<(User user, string password)>
            {
                (
                    new User
                    {
                        UserName = "alice@votex.com",
                        Email = "alice@votex.com",
                        EmailConfirmed = true,
                        RefreshToken = Guid.NewGuid()
                    },
                    "alice1"
                ),
                (
                    new User
                    {
                        UserName = "bob@votex.com",
                        Email = "bob@votex.com",
                        EmailConfirmed = true,
                        RefreshToken = Guid.NewGuid()
                    },
                    "bobby1"
                ),
                (
                    new User
                    {
                        UserName = "cintia@votex.com",
                        Email = "cintia@votex.com",
                        EmailConfirmed = true,
                        RefreshToken = Guid.NewGuid()
                    },
                    "cinti1"
                ),
                (
                    new User
                    {
                        UserName = "daniel@votex.com",
                        Email = "daniel@votex.com",
                        EmailConfirmed = true,
                        RefreshToken = Guid.NewGuid()
                    },
                    "daniel1"
                ),
                (
                    new User
                    {
                        UserName = "emma@votex.com",
                        Email = "emma@votex.com",
                        EmailConfirmed = true,
                        RefreshToken = Guid.NewGuid()
                    },
                    "emma11"
                ),
                (
                    new User
                    {
                        UserName = "fred@votex.com",
                        Email = "fred@votex.com",
                        EmailConfirmed = true,
                        RefreshToken = Guid.NewGuid()
                    },
                    "freddy1"
                ),
            };

            foreach (var item in users)
            {
                userManager.CreateAsync(item.user, item.password);
            }

            // Create votings
            var votings = new List<Voting>
            {
                new Voting
                {
                    Question = "What is your favorite programming language?",
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(7),
                    Options = new List<Option>
                    {
                        new Option { Text = "C#" },
                        new Option { Text = "Python" },
                        new Option { Text = "JavaScript" },
                        new Option { Text = "Java" }
                    },
                    Users = new List<User>
                    {
                        admin,
                        users[0].user,
                        users[1].user,
                        users[2].user,
                        users[3].user,
                        users[4].user,
                        users[5].user
                    },
                    AreLiveResultsOn = true
                },
                new Voting
                {
                    Question = "Which database do you prefer?",
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(7),
                    Options = new List<Option>
                    {
                        new Option { Text = "SQL Server" },
                        new Option { Text = "PostgreSQL" },
                        new Option { Text = "MySQL" },
                        new Option { Text = "MongoDB" }
                    },
                    Users = new List<User>
                    {
                        admin,
                        users[0].user,
                        users[1].user,
                        users[3].user
                    },
                    AreLiveResultsOn = true
                },
                new Voting
                {
                    Question = "Which cloud provider do you use?",
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(7),
                    Options = new List<Option>
                    {
                        new Option { Text = "AWS" },
                        new Option { Text = "Azure" },
                        new Option { Text = "Google Cloud" },
                        new Option { Text = "Oracle Cloud" }
                    },
                    Users = new List<User>
                    {
                        admin,
                        users[2].user,
                        users[4].user,
                        users[5].user
                    },
                    AreLiveResultsOn = true
                },
                new Voting
                {
                    Question = "What is your favorite front-end framework?",
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(7),
                    Options = new List<Option>
                    {
                        new Option { Text = "React" },
                        new Option { Text = "Angular" },
                        new Option { Text = "Vue" },
                        new Option { Text = "Svelte" }
                    },
                    Users = new List<User>
                    {
                        admin,
                        users[0].user,
                        users[5].user
                    },
                    AreLiveResultsOn = true
                },
                new Voting
                {
                    Question = "Which operating system do you prefer?",
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(7),
                    Options = new List<Option>
                    {
                        new Option { Text = "Windows" },
                        new Option { Text = "Linux" },
                        new Option { Text = "MacOS" },
                        new Option { Text = "Other" }
                    },
                    Users = new List<User>
                    {
                        admin,
                        users[1].user,
                        users[4].user,
                        users[5].user
                    },
                    AreLiveResultsOn = true
                },
                new Voting
                {
                    Question = "Which IDE do you use most?",
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(7),
                    Options = new List<Option>
                    {
                        new Option { Text = "Visual Studio" },
                        new Option { Text = "VS Code" },
                        new Option { Text = "JetBrains Rider" },
                        new Option { Text = "Eclipse" }
                    },
                    Users = new List<User>
                    {
                        admin,
                        users[1].user,
                        users[2].user,
                        users[3].user,
                        users[5].user
                    },
                    AreLiveResultsOn = true
                },
                new Voting
                {
                    Question = "What is your favorite backend framework?",
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(7),
                    Options = new List<Option>
                    {
                        new Option { Text = "ASP.NET Core" },
                        new Option { Text = "Spring Boot" },
                        new Option { Text = "Django" },
                        new Option { Text = "Express.js" }
                    },
                    Users = new List<User>
                    {
                        admin,
                        users[4].user,
                        users[5].user
                    },
                    AreLiveResultsOn = true
                },
                new Voting
                {
                    Question = "What version control system do you use?",
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(7),
                    Options = new List<Option>
                    {
                        new Option { Text = "Git" },
                        new Option { Text = "SVN" },
                        new Option { Text = "Mercurial" },
                        new Option { Text = "Other" }
                    },
                    Users = new List<User>
                    {
                        admin,
                        users[0].user,
                        users[1].user,
                        users[2].user,
                        users[5].user
                    },
                    AreLiveResultsOn = true
                },
                new Voting
                {
                    Question = "Which CI/CD tool do you prefer?",
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(7),
                    Options = new List<Option>
                    {
                        new Option { Text = "GitHub Actions" },
                        new Option { Text = "Jenkins" },
                        new Option { Text = "GitLab CI/CD" },
                        new Option { Text = "Azure DevOps" }
                    },
                    Users = new List<User>
                    {
                        admin,
                        users[3].user,
                        users[4].user,
                        users[5].user
                    },
                    AreLiveResultsOn = true
                },
                new Voting
                {
                    Question = "Which mobile development framework do you use?",
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(7),
                    Options = new List<Option>
                    {
                        new Option { Text = "Flutter" },
                        new Option { Text = "React Native" },
                        new Option { Text = "SwiftUI" },
                        new Option { Text = "Kotlin" }
                    },
                    Users = new List<User>
                    {
                        admin,
                        users[1].user,
                        users[3].user,
                        users[4].user,
                    },
                    AreLiveResultsOn = true
                },
                new Voting
                {
                    Question = "A very old voting",
                    StartDate = new DateTime(1969, 9, 11, 9, 0, 0, 0, 0),
                    EndDate = new DateTime(1970, 9, 11, 9, 0, 0, 0, 0),
                    Options = new List<Option>
                    {
                        new Option { Text = "1" },
                        new Option { Text = "2" },
                        new Option { Text = "3" },
                        new Option { Text = "4" }
                    },
                    Users = new List<User>
                    {
                        admin,
                        users[0].user,
                        users[1].user,
                        users[2].user,
                        users[3].user,
                        users[4].user,
                        users[5].user
                    },
                    AreLiveResultsOn = false
                }
            };

            // Create votes for votings
            foreach (var voting in votings)
            {
                if (!voting.AreLiveResultsOn)
                    continue;

                Random r = new Random();
                voting.Votes = new List<Vote>
                {
                    new Vote { Option = voting.Options[r.Next(4)] }
                };
                voting.AlreadyVoted = new List<User>
                {
                    admin
                };
            }

            context.Votings.AddRange(votings);

            context.SaveChanges();
        }
    }
}

