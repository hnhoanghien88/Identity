INSERT INTO rate_limit_policies
    (ApplicationId, Name, RoutePattern, HttpMethods, PartitionBy, Algorithm,
     PermitLimit, WindowSeconds, BurstLimit, Priority, Version,
     CreatedBy, CreatedDate, IsActive, IsDeleted)
SELECT
    application.Id, seed.Name, seed.RoutePattern, seed.HttpMethods,
    seed.PartitionBy, seed.Algorithm, seed.PermitLimit, seed.WindowSeconds,
    seed.BurstLimit, seed.Priority, 1, 'migration', UTC_TIMESTAMP(6), 1, 0
FROM applications AS application
INNER JOIN (
    SELECT
        'Identity API - Default' AS Name,
        '*' AS RoutePattern,
        NULL AS HttpMethods,
        'User,Application,Endpoint' AS PartitionBy,
        'TokenBucket' AS Algorithm,
        300 AS PermitLimit,
        60 AS WindowSeconds,
        30 AS BurstLimit,
        0 AS Priority
    UNION ALL
    SELECT
        'Identity API - Authorization',
        'authorization',
        'GET',
        'IpAddress,Application,Endpoint',
        'SlidingWindow',
        30,
        60,
        NULL,
        100
    UNION ALL
    SELECT
        'Identity API - Write Operations',
        'api/**',
        'POST,PUT,PATCH,DELETE',
        'User,IpAddress,Application,Endpoint',
        'TokenBucket',
        100,
        60,
        20,
        50
) AS seed
WHERE application.Code = 'Identity'
    AND application.IsDeleted = 0
    AND NOT EXISTS (
        SELECT 1
        FROM rate_limit_policies AS existing
        WHERE existing.Name = seed.Name
    );
