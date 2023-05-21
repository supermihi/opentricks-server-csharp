using System.Collections.Concurrent;
using Doppelkopf.Server.Model;
using Doppelkopf.Server.Notifications;

namespace Doppelkopf.Server.Storage;

public class CachedStoreTableProvider : ITableProvider {
  private readonly ConcurrentDictionary<TableId, PersistingAndPublishingTable> _cache = new();
  private readonly ITableStore _store;
  private readonly ITableActionListener _listener;

  public CachedStoreTableProvider(ITableStore store, ITableActionListener listener) {
    _store = store;
    _listener = listener;
  }

  public async Task<ITable> Get(TableId id) {
    if (_cache.TryGetValue(id, out var result)) {
      return result;
    }
    var storedTable = await _store.TryGet(id);
    if (storedTable != null) {
      result = new PersistingAndPublishingTable(storedTable, _store, _listener);
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

  public Task<ITable> Create(UserId owner, string name) {
    var id = TableId.New();
    var meta = TableMeta.Init(id, name, owner);
    var table = new PersistingAndPublishingTable(TableData.Init(meta), _store, _listener);
    _cache.TryAdd(id, table);
    return Task.FromResult<ITable>(table);
  }
}