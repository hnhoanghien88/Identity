# Distributed rate limiting

Rate-limit policies remain in MySQL (`rate_limit_policies`). Runtime counters and
concurrency leases use Redis, so all API instances enforce the same limits.

Supported algorithms:

- `FixedWindow`
- `SlidingWindow`
- `TokenBucket`
- `Concurrency`

For `Concurrency`, `PermitLimit + BurstLimit` is the maximum number of active
requests and `WindowSeconds` is the safety timeout for a lease. The middleware
releases the lease when the request completes; the timeout also prevents leaked
slots when an API instance terminates unexpectedly.

## Local Redis

Start Redis from the API directory:

```powershell
docker compose -f docker-compose.redis.yml up -d
```

The default connection is `localhost:6379`. Override it in deployed environments:

```text
ConnectionStrings__Redis=<redis-connection-string>
```

To run without Redis, select the in-memory implementation:

```text
RateLimiting__Store=InMemory
```

`FailureMode=Open` allows requests when Redis is unavailable and logs the error.
Use `FailureMode=Closed` when protection is more important than availability.
