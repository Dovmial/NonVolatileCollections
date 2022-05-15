namespace NonVolatileCollections
{
    public interface IQueueSerializeHandler<T>: IDisposable where T : class, IModel
    {
        int CountFromFile { get;}
        Task SerializeAsync(T model);
        Task RemoveFromFileAsync();
        Task RemoveRangeFromFileAsync(int amountModels);
        Task SerializeRangeAsync(IEnumerable<T> models);
        Task ClearAsync();
        Task<IList<T>> DeserializeAsync();
    }
}