using NonVolatileCollections.FileDirectoryHandler;
using Microsoft.EntityFrameworkCore;


namespace NonVolatileCollections.DatabaseHandler
{
    internal class QueueDbContext<TModel> : DbContext where TModel : class, IModel
    {
        private bool _onLogging; // включение логирования
        public DbSet<TModel> TModels { get; set; } = null!;
        public string NameDb { get; private set; }
        public string CollectionPath { get; private set; }

        private FileManager _fileManager = new();
        private readonly StreamWriter? _logStream;
        private readonly string _collectionDirectoryPath;
        private readonly string? _logDirectoryPath;

        public QueueDbContext(string nameDb, bool onLogging = false)
        {
            NameDb = nameDb;
            _onLogging = onLogging;
            try
            {
                _collectionDirectoryPath = Path.Combine(_fileManager.StartWorkPath, "Collections");
                _fileManager.CreateDirectory(_collectionDirectoryPath);

                if(_onLogging)
                {
                    _logDirectoryPath = Path.Combine(_fileManager.StartWorkPath, "Logs");
                    _fileManager.CreateDirectory(_logDirectoryPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            CollectionPath = Path.Combine(_collectionDirectoryPath, $"{NameDb}.db");

            if (_onLogging)
            {
                _logStream = new StreamWriter(
                    path: Path.Combine(_logDirectoryPath, $"{nameDb}DbLog.txt"),
                    append: true);
            }
            _ = Database.EnsureCreatedAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={CollectionPath}");
            if(_onLogging)
                optionsBuilder.LogTo(_logStream!.WriteLine);
        }

        public override void Dispose()
        {
            base.Dispose();
            if(_onLogging)
                _logStream!.Dispose();
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            if (_onLogging) 
                await _logStream!.DisposeAsync();
        }
    }
}
