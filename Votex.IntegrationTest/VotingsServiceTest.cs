using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Votex.DataAccess;
using Votex.DataAccess.Models;
using Votex.DataAccess.Services;

namespace Votex.IntegrationTest
{
    [TestClass]
    public sealed class VotingsServiceTest
    {
        private VotexDbContext _context = null!;
        private UserManager<User> _userManager = null!;
        private ServiceProvider _serviceProvider = null!;
        private IVotingsService _votingsService = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            var services = new ServiceCollection();

            services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<VotexDbContext>()
            .AddDefaultTokenProviders();

            services.AddDbContext<VotexDbContext>(options =>
                options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

            services.AddLogging();

            _serviceProvider = services.BuildServiceProvider();

            _context = _serviceProvider.GetRequiredService<VotexDbContext>();
            _userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
            DbInitializer.Initialize(_context, _userManager);

            _votingsService = new VotingsService(_context);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        [TestMethod]
        public async Task IfAVotingIsRequestedById_ItIsReturnedCorrectly()
        {
            var voting = await _votingsService.GetVotingById(1);

            Assert.IsNotNull(voting);
            Assert.AreEqual(voting.Id, 1);
            Assert.AreEqual(voting.Question, "What is your favorite programming language?");
            Assert.IsTrue(voting.AreLiveResultsOn);

            Assert.IsTrue(voting.Users.Count > 2);
            Assert.IsTrue(voting.Votes.Count > 0);
            Assert.AreEqual(voting.Votes.Count, voting.AlreadyVoted.Count);
            Assert.AreEqual(voting.Options.Count, 4);
        }

        [TestMethod]
        public async Task IfTheOngoingVotingsAreRequestedForAUser_TheyAreReturnedCorrectly()
        {
            var user = _context.Users.First();

            var votings = await _votingsService.GetVotingsForUser(user, true);

            Assert.IsNotNull(votings);
            Assert.IsTrue(votings.Count > 0);
            foreach (var voting in votings)
            {
                Assert.IsTrue(voting.Users.Contains(user));
                Assert.IsTrue(voting.StartDate <= DateTime.Now && DateTime.Now <= voting.EndDate);
            }
        }

        [TestMethod]
        public async Task IfTheFinishedVotingsAreRequestedForAUser_TheyAreReturnedCorrectly()
        {
            var user = _context.Users.First();

            var votings = await _votingsService.GetVotingsForUser(user.Email!, false);

            Assert.IsNotNull(votings);
            Assert.AreEqual(votings.Count, 1);
        }

        [TestMethod]
        public async Task IfAVotingIsAdded_ItIsPresentInTheDatabase()
        {
            var users = _context.Users.ToList();

            Voting voting = new Voting
            {
                Question = "Will the test pass?",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(7),
                Options = new List<Option>
                {
                    new Option { Text = "Yes" },
                    new Option { Text = "No" }
                },
                Users = new List<User>
                {
                    users[0],
                    users[1]
                },
                AreLiveResultsOn = true
            };

            await _votingsService.AddVoting(voting);

            var requestedVoting = await _votingsService.GetVotingById(12);

            Assert.IsTrue(voting.Equals(requestedVoting));
        }

        [TestMethod]
        public async Task IfAVotingIsAddedWithLessUsersThanNeeded_TheCorrectExceptionIsThrown()
        {
            Voting voting = new Voting
            {
                Question = "Will the test pass?",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(7),
                Options = new List<Option>
                {
                    new Option { Text = "Yes" },
                    new Option { Text = "No" }
                },
                Users = new List<User>(),
                AreLiveResultsOn = true
            };

            try
            {
                await _votingsService.AddVoting(voting);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "A voting must have at least 2 added users!");
            }
        }

        [TestMethod]
        public async Task IfAVotingIsAddedWithLessOptionsThanNeeded_TheCorrectExceptionIsThrown()
        {
            var users = _context.Users.ToList();

            Voting voting = new Voting
            {
                Question = "Will the test pass?",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(7),
                Options = new List<Option>
                {
                    new Option { Text = "Yes" }
                },
                Users = new List<User>
                {
                    users[0],
                    users[1]
                },
                AreLiveResultsOn = true
            };

            try
            {
                await _votingsService.AddVoting(voting);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "A voting must have at least 2 options!");
            }
        }

        [TestMethod]
        public async Task IfAVotingIsAddedWithIncorrectDates_TheCorrectExceptionIsThrown()
        {
            var users = _context.Users.ToList();

            Voting voting = new Voting
            {
                Question = "Will the test pass?",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Options = new List<Option>
                {
                    new Option { Text = "Yes" },
                    new Option { Text = "No" }
                },
                Users = new List<User>
                {
                    users[0],
                    users[1]
                },
                AreLiveResultsOn = true
            };

            try
            {
                await _votingsService.AddVoting(voting);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "Invalid start or end date: both start and end has to be in the future!");
            }
        }

        [TestMethod]
        public async Task IfAVotingIsAddedWithTooShortOfATimespan_TheCorrectExceptionIsThrown()
        {
            var users = _context.Users.ToList();

            Voting voting = new Voting
            {
                Question = "Will the test pass?",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(1),
                Options = new List<Option>
                {
                    new Option { Text = "Yes" },
                    new Option { Text = "No" }
                },
                Users = new List<User>
                {
                    users[0],
                    users[1]
                },
                AreLiveResultsOn = true
            };

            try
            {
                await _votingsService.AddVoting(voting);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "Invalid start or end date: there must be at least 15 minutes between the start and end!");
            }
        }

        [TestMethod]
        public async Task IfAValidVoteIsAddedToAVoting_ItIsPresebtInTheDatabase()
        {
            var user = _context.Users.Single(x => x.Email!.ToLower().Contains("alice"));
            var voting = await _votingsService.GetVotingById(1);
            int currentVotes = voting.Votes.Count;

            Vote vote = new Vote
            {
                Option = voting.Options[0]
            };

            await _votingsService.VoteForVoting(voting, vote, user);

            Assert.AreEqual(currentVotes + 1, voting.Votes.Count);
        }

        [TestMethod]
        public async Task IfAnUnautgorizedUserVotesForAVoting_TheCorrectExceptionIsThrown()
        {
            var user = new User();
            var voting = await _votingsService.GetVotingById(1);

            Vote vote = new Vote
            {
                Option = voting.Options[0]
            };

            try
            {
                await _votingsService.VoteForVoting(voting, vote, user);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "This user cannot vote for this voting!");
            }
        }

        [TestMethod]
        public async Task IfAUserVotesForAVotingTwice_TheCorrectExceptionIsThrown()
        {
            var user = _context.Users.First();
            var voting = await _votingsService.GetVotingById(1);

            Vote vote = new Vote
            {
                Option = voting.Options[0]
            };

            try
            {
                await _votingsService.VoteForVoting(voting, vote, user);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "This user has already voted!");
            }
        }

        [TestMethod]
        public async Task IfAnInvalidOptionIsSet_TheCorrectExceptionIsThrown()
        {
            var user = _context.Users.Single(x => x.Email!.ToLower().Contains("alice"));
            var voting = await _votingsService.GetVotingById(1);

            Vote vote = new Vote
            {
                OptionId = 6969
            };

            try
            {
                await _votingsService.VoteForVoting(voting, vote, user);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "Not a valid vote for specified voting!");
            }
        }
    }
}
