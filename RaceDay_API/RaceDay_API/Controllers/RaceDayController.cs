using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
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
        private static List<EventEnrollments> EventEnrollments = new List<EventEnrollments>
        {
            new EventEnrollments
            {
                Id = 1,
                EventId = 1,
                ParticipantId = 1
            },
            new EventEnrollments
            {
                Id = 2,
                EventId = 1,
                ParticipantId = 2
            }
        };

        private static List<ResultsResponse> ResultsResponses = new List<ResultsResponse>
        {
            new ResultsResponse
            {
                Id = 1,
                EventId = 1,
                ParticipantId = 1,
                Time = new TimeSpan(0, 45, 30),
                Distance = 10.0,
                CategoryId = "1"
            },
            new ResultsResponse
            {
                Id = 2,
                EventId = 1,
                ParticipantId = 2,
                Time = new TimeSpan(0, 50, 15),
                Distance = 10.0,
                CategoryId = "1"
            }
        };

        private static List<category> Categories = new List<category>
        {
            new category
            {
                Id = "1",
                Name = "Open",
                Description = "Open category for all participants."
            },
            new category
            {
                Id = "2",
                Name = "Veteran",
                Description = "Category for participants aged 40 and above."
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
                CategoryId = "1",
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
                CategoryId = "1"
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

        [Authorize]


        // GET: api/RaceDay/Results
        [HttpGet("Results")]
        public async Task<ActionResult<IEnumerable<ResultsResponse>>> GetResults()
        {
            var results = await _context.Resultss
                .Select(r => new ResultsResponse
                {
                    Id = r.Id,
                    EventId = r.EventId,
                    ParticipantId = r.ParticipantId,
                    Time = r.Time,
                    Distance = r.Distance,
                    CategoryId = r.CategoryId
                })
                .ToListAsync();
            return Ok(results);
        }

        // GET: api/RaceDay/Results/5
        [HttpGet("Results/{id}")]
        public async Task<ActionResult<ResultsResponse>> GetResult(int id)
        {
            var result = await _context.Resultss.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var resultResponse = new ResultsResponse
            {
                Id = result.Id,
                EventId = result.EventId,
                ParticipantId = result.ParticipantId,
                Time = result.Time,
                Distance = result.Distance,
                CategoryId = result.CategoryId
            };
            return Ok(resultResponse);
        }

        //GET: api/RaceDay/Results/event/5
        [HttpGet("Results/event/{eventId}")]
        public async Task<ActionResult<IEnumerable<ResultsResponse>>> GetResultsByEvent(int eventId)
        {
            var results = await _context.Resultss
                .Where(r => r.EventId == eventId)
                .Select(r => new ResultsResponse
                {
                    Id = r.Id,
                    EventId = r.EventId,
                    ParticipantId = r.ParticipantId,
                    Time = r.Time,
                    Distance = r.Distance,
                    CategoryId = r.CategoryId
                })
                .ToListAsync();
            if (results == null || results.Count == 0)
            {
                return NotFound("No results found for the specified event.");
            }
            return Ok(results);
        }

        // POST: api/RaceDay/Results
        [HttpPost("Results")]
        public async Task<ActionResult<ResultsResponse>> CreateResult(ResultsResponse resultResponse, string role)
        {
            var organiser = await _context.Organizers.FindAsync(resultResponse.ParticipantId);
            if (organiser == null)
            {
                return NotFound("Organizer not found.");
            }
            if (role != "Organizer")
            {
                return Forbid("Only organizers can create results.");
            }
            var result = new Resultss
            {
                EventId = resultResponse.EventId,
                ParticipantId = resultResponse.ParticipantId,
                Time = resultResponse.Time,
                Distance = resultResponse.Distance,
                CategoryId = resultResponse.CategoryId
            };
            _context.Resultss.Add(result);
            await _context.SaveChangesAsync();
            resultResponse.Id = result.Id;
            return CreatedAtAction(nameof(GetResult), new { id = result.Id }, resultResponse);
        }

        //DELETE: api/RaceDay/Results/5
        [HttpDelete("Results/{id}")]
        public async Task<IActionResult> DeleteResult(int id, int organizerId)
        {
            var results = await _context.Resultss.FindAsync(id);
            var organiser = await _context.Organizers.FindAsync(results.ParticipantId);
            if (organiser == null)
            {
                return NotFound("Organizer not found.");
            }

            var resultss = await _context.Resultss.FindAsync(id);
            if (resultss == null)
            {
                return NotFound();
            }

            _context.Resultss.Remove(resultss);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Helper method to check if a result exists
        private bool ResultExists(int id)
        {
            return _context.Resultss.Any(r => r.Id == id);
        }

        //Event Enrollments
        [HttpGet("EventEnrollments")]
        public async Task<ActionResult<IEnumerable<EventEnrollments>>> GetEventEnrollments()
        {
            return await _context.EventEnrollments.ToListAsync();
        }

        // GET: api/RaceDay/EventEnrollments/5
        [HttpGet("EventEnrollments/{id}")]
        public async Task<ActionResult<EventEnrollments>> GetEventEnrollment(int id)
        {
            if (role
                != "organizer") 
            { 
                return Unauthorized("Only organizers can access event enrollments.");
            }
            var eventEnrollment = await _context.EventEnrollments.FindAsync(id)
                .Where (e => e.OrganizerId == organizerId)
                .Select(e => new EventEnrollments
                {
                    Id = e.Id,
                    EventId = e.EventId,
                    ParticipantId = e.ParticipantId,
                    Age = e.Age,
                    OrganizerId = e.OrganizerId,
                    CategoryId = e.CategoryId
                }).ToListAsync();
            if (eventEnrollment == null)
            {
                return NotFound();
            }
            return eventEnrollment;
        }

        // POST: api/RaceDay/EventEnrollments
        [HttpPost("EventEnrollments")]
        public async Task<ActionResult<EventEnrollments>> CreateEventEnrollment(EventEnrollments eventEnrollment)
        {
            var participant = await _context.Participants.FindAsync(eventEnrollment.ParticipantId);
            if (participant == null)
            {
                return NotFound("Participant not found.");
            }

            var raceEvent = await _context.Events.FindAsync(eventEnrollment.EventId);
            if (raceEvent == null)
            {
                return NotFound("Event not found.");
            }

            var eventEnrollments = await _context.EventEnrollments
                .FirstOrDefaultAsync(e => e.ParticipantId == eventEnrollment.ParticipantId && e.EventId == eventEnrollment.EventId);
            if (eventEnrollments != null)
            {
                return Conflict("Participant is already enrolled in this event.");
            }

            _context.EventEnrollments.Add(eventEnrollment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEventEnrollment), new { id = eventEnrollment.Id }, eventEnrollment);
        }

        // PUT: api/RaceDay/EventEnrollments/5
        [HttpPut("EventEnrollments/{id}")]
        public async Task<IActionResult> UpdateEventEnrollment(int id, EventEnrollments eventEnrollment)
        {
            if (id != eventEnrollment.Id)
            {
                return BadRequest();
            }
            _context.Entry(eventEnrollment).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventEnrollmentExists(id))
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

        // Helper method to check if an event enrollment exists
        private bool EventEnrollmentExists(int id)
        {
            return _context.EventEnrollments.Any(e => e.Id == id);
        }

        // DELETE: api/RaceDay/EventEnrollments/5
        [HttpDelete("EventEnrollments/{id}")]
        public async Task<IActionResult> DeleteEventEnrollment(int id)
        {
            var eventEnrollment = await _context.EventEnrollments.FindAsync(id);
            if (eventEnrollment == null)
            {
                return NotFound();
            }
            _context.EventEnrollments.Remove(eventEnrollment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}