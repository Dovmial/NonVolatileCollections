using Microsoft.EntityFrameworkCore;
namespace NonVolatileCollections.DatabaseHandler
{
    public class QueueDBSerializeHandler<T> : IQueueSerializeHandler<T> where T : class, IModel
    {
        private readonly QueueDbContext<T> _dbContext = null!;

        public int CountFromFile
        {
            get => _dbContext!.TModels.Count();
        }

        public QueueDBSerializeHandler(string nameCollection, bool onLogging = false)
        {
            _dbContext = new QueueDbContext<T>(nameCollection);
        }

        public async Task SerializeAsync(T model)
        {
            if (model is null)
                return;
            await _dbContext!.AddAsync(model);
            await _dbContext!.SaveChangesAsync();
        }

        public async Task RemoveFromFileAsync()
        {
            var first = _dbContext.TModels.First();
            if (first is null)
                return;
            _dbContext.Remove(first);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<IList<T>> DeserializeAsync()
        {
            return await _dbContext.TModels.ToListAsync();
        }

        public async Task SerializeRangeAsync(IEnumerable<T> models)
        {
            await _dbContext.AddRangeAsync(models);
            await _dbContext.SaveChangesAsync();
        }

       
        public async Task RemoveRangeFromFileAsync(int amountModels)
        {
            var query = _dbContext.TModels.Take(amountModels);

            _dbContext.RemoveRange(query);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ClearAsync()
        {
            _ = await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM TModels; VACUUM;");
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeletedAsync();
            _dbContext.Dispose();
        }
    }
}
