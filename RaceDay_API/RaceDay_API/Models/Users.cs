using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RaceDay_API.Models
{
    public class Participants
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public string Username { get; set; }    
        public string Password { get; set; }
    }

    public class ParticipantsLogin
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ParticipantsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ParticipantsLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
    public class ParticipantsLoginResponse
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
    }

    public class ParticipantsUpdateRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ParticipantsUpdateResponse
    {
        public string Message { get; set; }
    }

    public class Organizer
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
    }
    public class OrganizerRequest
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class OrganizerResponse
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
    }

    public class OrganizerLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class OrganizerLoginResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }

    public class OrganizerUpdateRequest
    {
        public string Label { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
    }

    public class OrganizerUpdateResponse
    {
        public string Message { get; set; }
    }

    public class  Registration
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int ParticipantId { get; set; }

    }

    public class  Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
        public double Distance { get; set; }
        public string Categories { get; set; }
        public int Age { get; set; }
        public int OrganizerId { get; set; }
        public int ParticipantsId { get; set; }
    }

    public class  EventEnrollments
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int ParticipantId { get; set; }
        public int Age { get; set; }
        public int OrganizerId { get; set; }
        public string Category { get; set; }

    }

    public class  Resultss 
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int ParticipantId { get; set; }
        public TimeSpan Time { get; set; }
        public int Postion { get; set; }
        public double Distance { get; set; }
        public string Category { get; set; }

    }
    // i had to write results as 'resultss' because there was an error when i tried to write it as 'results' because it was a reserved keyword in c# so i had to add an extra 's' at the end of the word 'results' to make it work
    public class  ResultsResponse
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int ParticipantId { get; set; }
        public TimeSpan Time { get; set; }
        public double Distance { get; set; }
        public string Category { get; set; }
    }

}
