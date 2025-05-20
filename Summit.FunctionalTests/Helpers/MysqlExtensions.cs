using MySqlConnector;

namespace Summit.FunctionalTests.Helpers;

public static class MysqlExtensions
{
    public static async Task InsertTeacher(this MySqlConnection mysqlConnection, string teacherId, string name, string email)
    {
        const string query = "INSERT INTO teachers (teacherId, name, email) VALUES (@teacherId, @name, @email)";
        
        await using var command = new MySqlCommand(query, mysqlConnection);
        
        command.Parameters.AddWithValue("@teacherId", teacherId);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@email", email);
        
        await command.ExecuteNonQueryAsync();
    }
}