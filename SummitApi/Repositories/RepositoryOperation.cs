namespace SummitApi.Repositories;

public abstract record RepositoryOperation<T>
{
    public record Success(T Result) : RepositoryOperation<T>;

    public record Failure(string Reason) : RepositoryOperation<T>;

    public record Error(Exception Exception) : RepositoryOperation<T>;
}