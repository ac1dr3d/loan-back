public interface ILoanQueueConsumer
{
    Task<List<int>> GetLoanIdsFromQueueAsync();
}
