using SummitApi.Models;
using SummitApi.Repositories;

namespace SummitApi;

public interface IPairHandler
{
    Task<PairResponse> Handle(PairRequest request, CancellationToken cancellationToken);
}

public class PairHandler(
    IStudentRepository studentRepository,
    ITeacherRepository teacherRepository,
    IStudentScoreRepository studentScoreRepository,
    IPairRepository pairRepository) : IPairHandler
{
    public async Task<PairResponse> Handle(
        PairRequest pairRequest,
        CancellationToken cancellationToken)
    {
        var teacherResponse = await teacherRepository.GetTeacher(pairRequest.TeacherId, cancellationToken);

        return teacherResponse switch
        {
            RepositoryOperation<Teacher>.Success success => await HandleTeacher(pairRequest, success.Result, cancellationToken),
            RepositoryOperation<Teacher>.Failure => new PairResponse.Failure("TEACHER_NOT_FOUND"),
            RepositoryOperation<Teacher>.Error error => new PairResponse.Error(error.Exception),
        };
    }

    private async Task<PairResponse> HandleTeacher(
        PairRequest pairRequest,
        Teacher teacher,
        CancellationToken cancellationToken)
    {
        var studentResponse = await studentRepository.GetStudent(pairRequest.StudentId, cancellationToken);

        return studentResponse switch
        {
            RepositoryOperation<Student>.Success success => await HandleStudent(teacher, success.Result,
                cancellationToken),
            RepositoryOperation<Student>.Failure => new PairResponse.Failure("STUDENT_NOT_FOUND"),
            RepositoryOperation<Student>.Error error => new PairResponse.Error(error.Exception),
        };
    }
    
    private async Task<PairResponse> HandleStudent(
        Teacher teacher,
        Student student,
        CancellationToken cancellationToken)
    {
        var studentScoreResponse = await studentScoreRepository.GetStudentScoreAsync(student.StudentId, cancellationToken);

        return studentScoreResponse switch
        {
            RepositoryOperation<StudentScoreModel>.Success success => await HandleScore(teacher, student, success.Result, cancellationToken),
            RepositoryOperation<StudentScoreModel>.Failure => new PairResponse.Failure("STUDENT_SCORE_NOT_FOUND"),
            RepositoryOperation<StudentScoreModel>.Error error => new PairResponse.Error(error.Exception),
        };
    }
    
    private async Task<PairResponse> HandleScore(
        Teacher teacher,
        Student student,
        StudentScoreModel studentScore,
        CancellationToken cancellationToken)
    {
        if (studentScore.Score < 50)
        {
            return new PairResponse.Failure("STUDENT_SCORE_NOT_ENOUGH");
        }
        var pairResponse = await pairRepository.Pair(teacher, student, cancellationToken);

        return pairResponse switch
        {
            RepositoryOperation<string>.Success success => new PairResponse.Success(success.Result),
            RepositoryOperation<string>.Failure failure => new PairResponse.Failure(failure.Reason),
            RepositoryOperation<string>.Error error => new PairResponse.Error(error.Exception),
        };
    }
}