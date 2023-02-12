namespace BureauhouseApi.Services
{
    public class ElasticsearchService
    {
        private readonly ElasticsearchClient _elasticsearch;

        public ElasticsearchService(ElasticsearchClient elasticsearch)
        {
            _elasticsearch = elasticsearch;
        }

        public async Task PersistTransaction(Transaction transaction)
        {
            try
            {
                transaction.Id = Guid.NewGuid();
                await _elasticsearch.IndexAsync(transaction, request => request.Index("easy-verify"));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
