namespace SummitApi.Models;

public abstract record PairResponse
{
    public record Success(string PairId) : PairResponse;
    
    public record Failure(string Reason) : PairResponse;

    public record Error(Exception Exception) : PairResponse;
}
