
Caching stores the result of an expensive operation temporarily so subsequent requests can skip repeating it. In web APIs, the expensive operation is almost always a database query.

---

## `IMemoryCache` — the standard in-process cache

Stores data in the server's memory. Built into ASP.NET Core, injected via DI.

### Setup

```csharp
// Program.cs
builder.Services.AddMemoryCache();
```

### Cache-aside pattern — the only pattern you need to know

Check cache → on miss, load from DB → store in cache → return.

```csharp
public async Task<TournamentDto> GetTournamentAsync(long id)
{
    var cacheKey = $"tournament:{id}";

    // 1. Check cache
    if (_cache.TryGetValue(cacheKey, out TournamentDto? cached))
        return cached!;

    // 2. Cache miss — load from DB
    var tournament = await _repo.GetByIdAsync(id)
        ?? throw new NotFoundException("Tournament", id);

    var dto = _mapper.Map<TournamentDto>(tournament);

    // 3. Store with expiration
    var options = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
        .SetSlidingExpiration(TimeSpan.FromMinutes(2));

    _cache.Set(cacheKey, dto, options);
    return dto;
}
```

### Invalidation on write — remove stale entry when data changes

```csharp
public async Task UpdateTournamentAsync(long id, UpdateTournamentRequestDTO request)
{
    // ... update DB ...
    await _unitOfWork.SaveChangesAsync();

    _cache.Remove($"tournament:{id}"); // next read repopulates from DB
}
```

---

## Absolute vs Sliding expiration

- **Absolute** — entry dies at a fixed deadline regardless of how often it's accessed. Prevents stale data from living forever.
- **Sliding** — deadline resets on every access. Frequently used data stays warm; cold data gets cleaned up.
- **Combine both** — sliding keeps hot data alive, absolute is the hard ceiling. This is the recommended pattern.

---

## `IMemoryCache` vs `IDistributedCache` (Redis)

||`IMemoryCache`|`IDistributedCache` / Redis|
|---|---|---|
|Where data lives|App process memory|External process (Redis server)|
|Multi-server safe|❌ No|✅ Yes|
|Survives app restart|❌ No|✅ Yes|
|Setup|Zero|Needs Redis|

**The key limitation of `IMemoryCache`:** each server instance has its own cache. If you run two servers, Server A and Server B have different caches. A write on Server A invalidates its own cache but Server B still serves stale data.

**Interview answer:** _"I use `IMemoryCache` for a single-server app — it's simple and fast. When scaling to multiple instances I'd switch to `IDistributedCache` backed by Redis so all instances share the same cache."_

---

## Common interview questions

- **"What is caching?"** → Temporarily storing the result of an expensive operation (usually a DB query) so repeated requests can skip it.
- **"What is the cache-aside pattern?"** → Check cache first. On miss, load from source, store result in cache, return. On write, remove the stale entry.
- **"Absolute vs sliding expiration?"** → Absolute = hard deadline. Sliding = resets on access. Combine them for a ceiling + liveness policy.
- **"When would `IMemoryCache` fail you?"** → Multiple server instances — each has its own cache, so you get inconsistency. Use distributed cache instead.
- **"How do you keep the cache consistent with the database?"** → Remove the cache entry on every write (`_cache.Remove(key)`). Next read repopulates it fresh.