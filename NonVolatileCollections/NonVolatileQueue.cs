

namespace NonVolatileCollections
{
    public class NonVolatileQueue<T> : Queue<T>, IDisposable where T : class, IModel
    {
        private readonly IQueueSerializeHandler<T> _serializeHandler;

        private async Task init()
        {
            int amount = _serializeHandler.CountFromFile;
            if (amount > 0)
            {
                var collection = await _serializeHandler.DeserializeAsync();
                for (int i = 0; i < amount; ++i)
                    base.Enqueue(collection[i]);
            }
        }
        public NonVolatileQueue(IQueueSerializeHandler<T> serializer) : base()
        {
            _serializeHandler = serializer;
            _ = init();
            
        }

        public NonVolatileQueue(IQueueSerializeHandler<T> serializer, IEnumerable<T> collection): base()
        {
            _serializeHandler = serializer;
            _ = _serializeHandler.SerializeRangeAsync(collection);
            _ = init();
        }

        public NonVolatileQueue(IQueueSerializeHandler<T> serializer, int capacity) : base(capacity)
        {
            _serializeHandler = serializer;
            _ = init();
        }

        public new async Task Clear()
        {
            await _serializeHandler.ClearAsync();
            base.Clear();
        }
        public new async Task Enqueue(T item)
        {
            await _serializeHandler.SerializeAsync(item);
            base.Enqueue(item);
        }

        public new async Task<T> Dequeue()
        {
            await _serializeHandler.RemoveFromFileAsync();
            return base.Dequeue();
        }

        public new void TryDequeue(out T? item)
        {
            if (base.TryDequeue(out item))
                _ = _serializeHandler.RemoveFromFileAsync();
        }

        public async Task RemoveRangeAsync(int amountItems)
        {
            if (amountItems > Count)
                throw new ArgumentOutOfRangeException(nameof(amountItems));
            await _serializeHandler.RemoveRangeFromFileAsync(amountItems);

            for (int i = 0; i < amountItems; ++i)
                base.Dequeue();
        }
       
        public void Dispose()
        {
            _serializeHandler.Dispose();
        }
    }
}
