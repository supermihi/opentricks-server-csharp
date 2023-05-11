using System.Collections.Concurrent;
using System.Linq;

namespace Doppelkopf.Server;

public class CachedStoreTableProvider : ITableProvider
{
  private readonly ConcurrentDictionary<TableId, PersistingAndPublishingTable> _cache = new();
  private readonly ITableStore _store;
  private readonly ITableActionListener _listener;

  public CachedStoreTableProvider(ITableStore store, ITableActionListener listener)
  {
    _store = store;
    _listener = listener;
  }

  public async Task<ITable> Get(TableId id)
  {
    if (_cache.TryGetValue(id, out var result))
    {
      return result;
    }
    var stored = await _store.TryGet(id);
    if (stored != null)
    {
      var (meta, table) = stored.Value;
      result = new PersistingAndPublishingTable(meta, table, _store, _listener);
      _cache.TryAdd(id, result);
      return _cache[id];
    }
    throw new KeyNotFoundException();
  }

  public async IAsyncEnumerable<ITable> GetAll() {
    await Task.Yield();
    foreach (var v in _cache.Values) {
      yield return v;
    }
  }

  public Task<ITable> Create(UserId owner)
  {
    var id = TableId.New();
    var meta = TableMeta.Init(id, owner);
    var table = new PersistingAndPublishingTable(meta, null, _store, _listener);
    _cache.TryAdd(id, table);
    return Task.FromResult<ITable>(table);
  }
}
