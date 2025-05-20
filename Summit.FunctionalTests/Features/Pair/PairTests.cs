using Summit.FunctionalTests.Helpers;

namespace Summit.FunctionalTests.Features.Pair;

[Collection("AspireAppHostCollection")]
public class PairTests(AspireAppHostFixture fixture)
{
    [Fact]
    public async Task Pair_WhenRequestedForUnknownTeacher_ShouldReturnFailure()
    {
        // Assert
        const string teacherId = "unknown-teacher-id";
        const string studentId = "unknown-student-id";

        var response = await fixture.HttpClient.PairTeacherStudent(teacherId, studentId);

        var failureResponse = await response.ShouldBeFailureResponse();

        Assert.Equal("TEACHER_NOT_FOUND", failureResponse);
    }
    
    [Fact]
    public async Task Pair_WhenRequestedForUnknownStudent_ShouldReturnFailure()
    {
        // Assert
        const string teacherId = "teacher-id-1000";
        const string studentId = "unknown-student-id";

        await fixture.MysqlConnection.InsertTeacher(teacherId, "Teacher Name", "Teacher Email");
        
        var response = await fixture.HttpClient.PairTeacherStudent(teacherId, studentId);

        var failureResponse = await response.ShouldBeFailureResponse();

        Assert.Equal("STUDENT_NOT_FOUND", failureResponse);
    }
    
    [Fact]
    public async Task Pair_WhenRequestedForStudentWithUnknownScore_ShouldReturnFailure()
    {
        // Assert
        const string teacherId = "teacher-id-2000";
        const string studentId = "student-id-1000";

        await fixture.MysqlConnection.InsertTeacher(teacherId, "Teacher Name", "Teacher Email");
        await fixture.AwsConnection.InsertStudent(studentId, "Student Name", "Student Email");

        var response = await fixture.HttpClient.PairTeacherStudent(teacherId, studentId);

        var failureResponse = await response.ShouldBeFailureResponse();

        Assert.Equal("STUDENT_SCORE_NOT_FOUND", failureResponse);
    }
    
    [Fact]
    public async Task Pair_WhenRequestedForStudentWithLowScore_ShouldReturnFailure()
    {
        // Arrange
        const string teacherId = "teacher-id-3000";
        const string studentId = "student-id-2000";

        await fixture.MysqlConnection.InsertTeacher(teacherId, "Teacher Name", "Teacher Email");
        await fixture.AwsConnection.InsertStudent(studentId, "Student Name", "Student Email");
        await fixture.StudentScoreApiClient.InsertStudentScore(studentId, 30);

        // Act
        var response = await fixture.HttpClient.PairTeacherStudent(teacherId, studentId);

        // Assert
        var failureResponse = await response.ShouldBeFailureResponse();
        
        Assert.Equal("STUDENT_SCORE_NOT_ENOUGH", failureResponse);
    }
    
    [Fact]
    public async Task Pair_WhenRequestedForTeacherAndStudentWithHighScore_ShouldReturnAccepted()
    {
        // Arrange
        const string teacherId = "teacher-id-4000";
        const string studentId = "student-id-3000";

        await fixture.MysqlConnection.InsertTeacher(teacherId, "Teacher Name", "Teacher Email");
        await fixture.AwsConnection.InsertStudent(studentId, "Student Name", "Student Email");
        await fixture.StudentScoreApiClient.InsertStudentScore(studentId, 70);

        // Act
        var response = await fixture.HttpClient.PairTeacherStudent(teacherId, studentId);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }
}