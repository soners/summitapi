using Dapper;
using MySqlConnector;
using SummitApi.Models;

namespace SummitApi.Repositories;

public interface ITeacherRepository
{
    Task<RepositoryOperation<Teacher>> GetTeacher(string teacherId, CancellationToken cancellationToken);
}

public class TeacherRepository(MySqlConnection mySqlConnection) : ITeacherRepository
{
    public async Task<RepositoryOperation<Teacher>> GetTeacher(string teacherId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(teacherId))
        {
            return new RepositoryOperation<Teacher>.Failure("TeacherId is required");
        }

        try
        {
            await mySqlConnection.OpenAsync(cancellationToken);

            var teacher = await mySqlConnection.QueryFirstOrDefaultAsync<Teacher>(
                "SELECT * FROM teachers WHERE teacherId = @teacherId",
                new { teacherId });

            if (teacher == null)
            {
                return new RepositoryOperation<Teacher>.Failure("Teacher not found");
            }

            return new RepositoryOperation<Teacher>.Success(teacher);
        }
        catch (Exception ex)
        {
            return new RepositoryOperation<Teacher>.Error(ex);
        }
        finally
        {
            await mySqlConnection.CloseAsync();
        }
    }
}