using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
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

        // Constructor
        public RaceDayController(UsersDbContext context)
        {
            _context = context;
        }

        // GET: api/RaceDay/participants
        [HttpGet("participants")]
        public async Task<ActionResult<IEnumerable<Participants>>> GetParticipants()
        {
            return await _context.Participants.ToListAsync();
        }
        
        // GET: api/RaceDay/participants/5
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

        // POST: api/RaceDay/participants
        [HttpPost("participants")]
        public async Task<ActionResult<Participants>> CreateParticipant(Participants participant)
        {
            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetParticipant), new { id = participant.Id }, participant);
        }

        // PUT: api/RaceDay/participants/5
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

        // Helper method to check if a participant exists
        private bool ParticipantExists(int id)
        {
            return _context.Participants.Any(e => e.Id == id);
        }

        // DELETE: api/RaceDay/participants/5
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
        
        //login for participants
        [HttpPost("participants/login")]
        public async Task<ActionResult<Participants>> LoginParticipant([FromBody] DtoParticipantsLogin loginRequest)
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.Email == loginRequest.Email && p.PasswordHash == loginRequest.PasswordHash);
            if (participant == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.PasswordHash, participant.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized("Invalid email or password.");
            }
            return Ok(participant);
        }

        // GET: api/RaceDay/Organizers
        [HttpGet("Organizers")]
        public async Task<ActionResult<IEnumerable<Organizer>>> GetOrganizers()
        {
            return await _context.Organizers.ToListAsync();
        }
        
        // GET: api/RaceDay/Organizers/5
        [HttpGet("Organizers/{id}")]
        public async Task<ActionResult<Organizer>> GetOrganizer(int id)
        {
            var organizer = await _context.Organizers.FindAsync(id);
            if (organizer == null)
            {
                return NotFound();
            }
            return organizer;
        }

        // POST: api/RaceDay/Organizers
        [HttpPost("Organizers")]
        public async Task<ActionResult<Organizer>> CreateOrganizer(Organizer organizer)
        {
            _context.Organizers.Add(organizer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrganizer), new { id = organizer.Id }, organizer);
        }

        // PUT: api/RaceDay/Organizers/5
        [HttpPut("Organizers/{id}")]
        public async Task<IActionResult> UpdateOrganizer(int id, Organizer organizer)
        {
            if (id != organizer.Id)
            {
                return BadRequest();
            }
            _context.Entry(organizer).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizerExists(id))
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
        
        // Helper method to check if an organizer exists
        private bool OrganizerExists(int id)
        {
            return _context.Organizers.Any(e => e.Id == id);
        }
        
        // DELETE: api/RaceDay/Organizers/5
        [HttpDelete("Organizers/{id}")]
        public async Task<IActionResult> DeleteOrganizer(int id)
        {
            var organizer = await _context.Organizers.FindAsync(id);
            if (organizer == null)
            {
                return NotFound();
            }
            _context.Organizers.Remove(organizer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // login for organizers
        [HttpPost("Organizers/login")]
        public async Task<ActionResult<Organizer>> LoginOrganizer([FromBody] DtoOrganizerLogin loginRequest)
        {
            var organizer = await _context.Organizers
                .FirstOrDefaultAsync(o => o.Email == loginRequest.Email && o.PasswordHash == loginRequest.PasswordHash);
            if (organizer == null)
            {
                return Unauthorized("Invalid email or password.");
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.PasswordHash, organizer.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized("Invalid email or password.");
            }
            return Ok(organizer);
        }

        [Authorize]
        [HttpGet("Events/organizer/{organizerId}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsByOrganizer(int organizerId)
        {
            var events = await _context.Events
                .Where(e => e.OrganizerId == organizerId)
                .ToListAsync();
            if (events == null || events.Count == 0)
            {
                return NotFound("No events found for the specified organizer.");
            }
            return Ok(events);
        }

        // GET: api/RaceDay/Events
        [HttpGet("Events")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events.ToListAsync();
        }
        
        // GET: api/RaceDay/Events/5
        [HttpGet("Events/{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return @event;
        }
        
        // POST: api/RaceDay/Events
        [HttpPost("Events")]
        public async Task<ActionResult<Event>> CreateEvent(Event @event)
        {
            _context.Events.Add(@event);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
        }
        
        // PUT: api/RaceDay/Events/5
        [HttpPut("Events/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, Event @event)
        {
            if (id != @event.Id)
            {
                return BadRequest();
            }
            _context.Entry(@event).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
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
        
        // Helper method to check if an event exists
        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);    
        }

        // DELETE: api/RaceDay/Events/5
        [HttpDelete("Events/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // registration for participants to events
        [HttpPost("Events/{eventId}/register")]
        public async Task<IActionResult> RegisterParticipant(int eventId, [FromBody] Registration registration)
        {
            var @event = await _context.Events.FindAsync(eventId);
            if (@event == null)
            {
                return NotFound("Event not found.");
            }
            var participant = await _context.Participants.FindAsync(registration.ParticipantId);
            if (participant == null)
            {
                return Unauthorized("Participant not found.");
            }
            var existingRegistration = await _context.EventEnrollments
                .FirstOrDefaultAsync(e => e.EventId == eventId && e.ParticipantId == registration.ParticipantId);
            if (existingRegistration != null)
            {
                return BadRequest("Participant is already registered for this event.");
            }
            var eventEnrollment = new EventEnrollments
            {
                EventId = eventId,
                ParticipantId = registration.ParticipantId
            };
            _context.EventEnrollments.Add(eventEnrollment);
            await _context.SaveChangesAsync();
            return Ok(
                new Registration
                {
                    Id = eventEnrollment.Id,
                    Message = "Participant registered successfully.",   
                    EventId = eventId,
                    ParticipantId = registration.ParticipantId
                    } 
                );
        }
        [Authorize]
        [HttpGet("Events/{eventId}/register participants")]
        public async Task<ActionResult<IEnumerable<Participants>>> GetRegisteredParticipants(int eventId)
        {
            var @event = await _context.Events.FindAsync(eventId);
            if (@event == null)
            {
                return NotFound("Event not found.");
            }
            var registeredParticipants = await _context.EventEnrollments
                .Where(e => e.EventId == eventId)
                .Join(_context.Participants,
                    enrollment => enrollment.ParticipantId,
                    participant => participant.Id,
                    (enrollment, participant) => participant)
                .ToListAsync();
            return Ok(registeredParticipants);
        }

        // GET: api/RaceDay/Results
        [HttpGet("Results")]
        public async Task<ActionResult<IEnumerable<Resultss>>> GetResults()
        {
            return await _context.Resultss.ToListAsync();
        }

        // GET: api/RaceDay/Results/5
        [HttpGet("Results/{id}")]
        public async Task<ActionResult<Resultss>> GetResult(int id)
        {
            var result = await _context.Resultss.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }

        // POST: api/RaceDay/Results
        [HttpPost("Results")]
        public async Task<ActionResult<Resultss>> PostResult(Resultss result)
        {
            _context.Resultss.Add(result);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetResult", new { id = result.Id }, result);
        }

        // PUT: api/RaceDay/Results/5
        [HttpPut("Results/{id}")]
        public async Task<IActionResult> PutResult(int id, Resultss result)
        {
            if (id != result.Id)
            {
                return BadRequest();
            }
            _context.Entry(result).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResultExists(id))
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

        private bool ResultExists(int id)
        {
            return _context.Resultss.Any(e => e.Id == id);
        }

        // DELETE: api/RaceDay/Results/5
        [HttpDelete("Results/{id}")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var result = await _context.Resultss.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            _context.Resultss.Remove(result);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/RaceDay/Results/event/5
        [HttpGet("Results/event/{eventId}")]
        public async Task<ActionResult<IEnumerable<Resultss>>> GetResultsByEvent(int eventId)
        {
            var results = await _context.Resultss
                .Where(r => r.EventId == eventId)
                .ToListAsync();
            if (results == null || results.Count == 0)
            {
                return NotFound();
            }
            return Ok(results);
        }

        // GET: api/RaceDay/Results/participant/5
        [HttpGet("Results/participant/{participantId}")]
        public async Task<ActionResult<IEnumerable<Resultss>>> GetResultsByParticipant(int participantId)
        {
            var results = await _context.Resultss
                .Where(r => r.ParticipantId == participantId)
                .ToListAsync();
            if (results == null || results.Count == 0)
            {
                return NotFound();
            }
            return Ok(results);
        }

        // GET: api/RaceDay/Results/event/5/participant/5
        [HttpGet("Results/event/{eventId}/participant/{participantId}")]
        public async Task<ActionResult<Resultss>> GetResultByEventAndParticipant(int eventId, int participantId)
        {
            var result = await _context.Resultss
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.ParticipantId == participantId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // GET: api/RaceDay/Results/event/5/category/Open
        [HttpGet("Results/event/{eventId}/category/{category}")]
        public async Task<ActionResult<IEnumerable<Resultss>>> GetResultsByEventAndCategory(int eventId, string category)
        {
            var results = await _context.Resultss
                .Where(r => r.EventId == eventId && r.Category == category)
                .ToListAsync();
            if (results == null || results.Count == 0)
            {
                return NotFound();
            }
            return Ok(results);
        }

    }
}