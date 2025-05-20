namespace SummitWorker.Handler;

public sealed record PairMessage(Teacher Teacher, Student Student, string PairCode);