namespace ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;

public sealed class GameSeasonExistsException : Exception
{
    public GameSeasonExistsException()
    {
    }

    public GameSeasonExistsException(string message) : base(message)
    {
    }

    public GameSeasonExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
