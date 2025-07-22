using SharpA2A.Core;

/// <summary>
/// Provides helper methods for creating agent cards related to flight booking functionality.
/// </summary>
public class AgentCardHelper
{
    /// <summary>
    /// Creates and returns an <see cref="AgentCard"/> instance configured for the Flight Booking Agent.
    /// The agent handles requests related to flight bookings, such as booking flights between cities.
    /// </summary>
    /// <returns>
    /// An <see cref="AgentCard"/> object representing the Flight Booking Agent, including its capabilities and skills.
    /// </returns>
    public static AgentCard GetFlightBookingAgentCard()
    {
        var capabilities = new AgentCapabilities()
        {
            Streaming = false,
            PushNotifications = false,
        };

        var flightBookingQuery = new AgentSkill()
        {
            Id = "id_flight_booking_agent",
            Name = "Flight Booking Agent",
            Description = "Handles requests relating to flight bookings.",
            Tags = ["flight", "booking", "semantic-kernel"],
            Examples =
            [
                "Book a flight from Seattle to New York.",
                ],
        };

        return new AgentCard()
        {
            Name = "FlightBookingAgent",
            Description = "An agent that can book flights between cities.",
            Capabilities = capabilities,
            Skills = [flightBookingQuery],
        };
    }
}