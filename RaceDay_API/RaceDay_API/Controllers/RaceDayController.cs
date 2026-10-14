using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_API.Data;
using RaceDay_API.Models;

namespace RaceDay_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RaceDayController : ControllerBase
    {
        private readonly UsersDbContext _context;

        // In-memory lists
        private static List<Participants> Participants = new List<Participants> {
            new Participants
            {
                Id = 1,
                Name = "John",
                Surname = "Doe",
                Age = 30,
                Weight = 70,
                Height = 175,
                PasswordHash = "password123"
            },
            new Participants
            {
                Id = 2,
                Name = "Jane",
                Surname = "Smith",
                Age = 25,
                Weight = 60,
                Height = 165,
                PasswordHash = "password456"
            }
        };

        private static List<Organizer> Organizers = new List<Organizer> {
            new Organizer
            {
                Id = 1,
                Label = "Cyber razor",
                Email = "CyberRazor.raz.co.za",
                Description = "Description for Organizer 1"
            },
            new Organizer
            {
                Id = 2,
                Label = "FRNB",
                Email = "FRNB2@example.com",
                Description = "Description for Organizer 2"
            }
        };

        private static List<Event> Events = new List<Event> 
        {
            new Event 
            {
                Id = 1,
                Name = "Dodge",
                Description = "Join us for an unforgettable day of pavement, pace, and community spirit at the annual Dodge!",
                OrganizerId = 1,
                Date = new DateTime(2024, 6, 15),
                Location = "Dodge City, Cape Town",
                Distance = 10.0,
                Categories = "Open, Junior, Senior",
                Age = 18 - 50,
                Email = "CyberRazor.raz.co.za",
            }
        };
        private static List<Resultss> Results = new List<Resultss> 
        { 
            new Resultss 
            {
                Id = 1,
                EventId = 1,
                ParticipantId = 1,
                Time = new TimeSpan(0, 45, 30),
                Position = 1,
                Distance = 10.0,
                Category = "Open"   
            }
        };


        public RaceDayController(UsersDbContext context)
        {
            _context = context;
        }

        [HttpGet("participants")]
        public async Task<ActionResult<IEnumerable<Participants>>> GetParticipants()
        {
            return await _context.Participants.ToListAsync();
        }

        [HttpGet("participants/{id}")]
        public async Task<ActionResult<Participants>> GetParticipant(int id)
        {
            var participant = await _context.Participants.FindAsync(id);

            if (participant == null)
            {
                return NotFound();
            }

            return participant;
        }

        [HttpPost("participants")]
        public async Task<ActionResult<Participants>> CreateParticipant(Participants participant)
        {
            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetParticipant), new { id = participant.Id }, participant);
        }

        [HttpPut("participants/{id}")]
        public async Task<IActionResult> UpdateParticipant(int id, Participants participant)
        {
            if (id != participant.Id)
            {
                return BadRequest();
            }
            _context.Entry(participant).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParticipantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        private bool ParticipantExists(int id)
        {
            return _context.Participants.Any(e => e.Id == id);
        }

        [HttpDelete("participants/{id}")]
        public async Task<IActionResult> DeleteParticipant(int id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null)
            {
                return NotFound();
            }
            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}