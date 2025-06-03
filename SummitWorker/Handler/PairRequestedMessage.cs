namespace SummitWorker.Handler;

public sealed record PairRequestedMessage(Teacher Teacher, Student Student, string PairCode);
