using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Votex.DataAccess.Models;
using Votex.Shared.DTO;
using Votex.DataAccess.Services;
using Microsoft.AspNetCore.SignalR;
using Votex.Shared.SignalR;
using System.Net.Mail;
using System.Net;

namespace Votex.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/votings")]
    public class VotingController : ControllerBase
    {
        private readonly IVotingsService _votingsService;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<ResultsHub> _hubContext;

        public VotingController(IVotingsService votingsService, UserManager<User> userManager, IHubContext<ResultsHub> hubContext)
        {
            _votingsService = votingsService;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetVotings()
        {
            var currentUser = GetCurrentUser();
            var votings = await _votingsService.GetVotingsForUser(currentUser, true);

            List<VotingListingRequestDto> dtos = new List<VotingListingRequestDto>();
            foreach (var v in votings)
            {
                dtos.Add(new VotingListingRequestDto()
                {
                    Id = v.Id,
                    Question = v.Question,
                    StartDate = v.StartDate.ToString("g"),
                    EndDate = v.EndDate.ToString("g"),
                    AlreadyVoted = v.AlreadyVoted.Any(x => x.UserName == currentUser)
                });
            }

            return Ok(dtos);
        }   
        
        [HttpGet]
        [Route("finished")]
        public async Task<IActionResult> GetFinishedVotings()
        {
            var currentUser = GetCurrentUser();
            var votings = await _votingsService.GetVotingsForUser(currentUser, false);

            List<VotingListingRequestDto> dtos = new List<VotingListingRequestDto>();
            foreach (var v in votings)
            {
                dtos.Add(new VotingListingRequestDto()
                {
                    Id = v.Id,
                    Question = v.Question,
                    StartDate = v.StartDate.ToString("g"),
                    EndDate = v.EndDate.ToString("g"),
                    AlreadyVoted = v.AlreadyVoted.Any(x => x.UserName == currentUser)
                });
            }

            return Ok(dtos);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetVoting([FromRoute] int id)
        {
            var user = _userManager.Users.Single(x => x.Email == GetCurrentUser());
            var voting = await _votingsService.GetVotingById(id);

            if(voting.EndDate <= DateTime.Now || voting.Users.Count == voting.AlreadyVoted.Count)
                return BadRequest("The voting has already ended :(");

            if (!voting.Users.Contains(user))
                return BadRequest("The current user is not allowed to vote on that :(");    
            
            if (voting.AlreadyVoted.Contains(user))
                return BadRequest("The current user already has a vote on this voting :(");

            var dto = new VotingResponseDto
            {
                Id = voting.Id,
                Question = voting.Question,
                OptionIds = voting.Options.Select(x => x.Id).ToArray(),
                OptionValues = voting.Options.Select(x => x.Text).ToArray()
            };

            return Ok(dto);
        }


        [HttpPost]
        [Route("vote")]
        public async Task<IActionResult> Vote([FromBody] VoteRequestDto requestDto)
        {
            var voting = await _votingsService.GetVotingById(requestDto.VotingId);
            var user = _userManager.Users.Single(x => x.Email == GetCurrentUser());

            if (voting.EndDate <= DateTime.Now || voting.Users.Count == voting.AlreadyVoted.Count)
                return BadRequest("The voting has already ended :(");

            if (!voting.Users.Contains(user))
                return BadRequest("The current user is not allowed to vote on that :(");

            if (voting.AlreadyVoted.Contains(user))
                return BadRequest("The current user already has a vote on this voting :(");

            var vote = new Vote
            {
                Voting = voting,
                OptionId = requestDto.OptionId
            };

            await _votingsService.VoteForVoting(voting, vote, user);

            if (voting.AreLiveResultsOn)
            {
                await SendNotificationForVote(voting.Id);
            }
            
            return Ok();
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateVotingRequestDto requestDto)
        {
            foreach (var userEmail in requestDto.UserEmails)
            {
                User user = new User
                {
                    Email = userEmail,
                    UserName = userEmail,
                    EmailConfirmed = true,
                };

                var creationResult = await _userManager.CreateAsync(user);
                if (creationResult.Succeeded) // user did not exist before
                {
                    await SendEmailAsync(userEmail);
                }

            };

            var voting = new Voting
            {
                Question = requestDto.Question,
                Users = _userManager.Users.Where(x => requestDto.UserEmails.Contains(x.Email!)).ToList(),
                StartDate = DateTime.Parse(requestDto.StartDate),
                EndDate = DateTime.Parse(requestDto.EndDate),
                AreLiveResultsOn = requestDto.AreLiveResultsEnabled,
            };

            voting.Options = requestDto.Options.Select(x => new Option { Text = x, Voting = voting }).ToList();

            try
            { 
                await _votingsService.AddVoting(voting);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpGet]
        [Route("results/{id}")]
        public async Task<IActionResult> Results([FromRoute] int id)
        {
            var voting = await _votingsService.GetVotingById(id);

            VotingResultRequestDto dto = new VotingResultRequestDto
            {
                Id = id,
                Question = voting.Question,
                Options = voting.Options.OrderBy(x => x.Id).Select(x => x.Text).ToList(),
                VoteCountForOptions = new(),
                AllVotersCount = voting.Users.Count
            };
            foreach (var option in dto.Options)
            {
                dto.VoteCountForOptions.Add(
                    voting.Votes.Where(x => x.Option.Text == option).Count()
                );
            }

            return Ok(dto);
        }

        private string GetCurrentUser()
        {
            return HttpContext.User.Claims.Single(x => x.Type == "username").Value;
        }

        private async Task SendNotificationForVote(int votingId)
        {
            var voting = await _votingsService.GetVotingById(votingId);

            VotingResultRequestDto dto = new VotingResultRequestDto
            {
                Id = votingId,
                Question = voting.Question,
                Options = voting.Options.OrderBy(x => x.Id).Select(x => x.Text).ToList(),
                VoteCountForOptions = new(),
                AllVotersCount = voting.Users.Count
            };
            foreach (var option in dto.Options)
            {
                dto.VoteCountForOptions.Add(
                    voting.Votes.Where(x => x.Option.Text == option).Count()
                );
            }

            await _hubContext.Clients.All.SendAsync("ResultChanged", dto);
        }

        private async Task SendEmailAsync(string toEmail)
        {
            using MailMessage mail = new MailMessage();
            mail.From = new MailAddress("rvr826@inf.elte.hu");
            mail.To.Add(toEmail);
            mail.Subject = "Votex - you have been added to a voting!";
            mail.Body = "Dear client,\n you have been added to a voting on our site. Please register with you e-mail of this account on our site!\n\n Yours truly,\nVotex team";
            mail.IsBodyHtml = false;

            using SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.Credentials = new NetworkCredential("rvr826@inf.elte.hu", "nhdtb-103");
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(mail);
        }

    }
}
