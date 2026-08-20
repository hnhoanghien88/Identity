INSERT INTO rate_limit_policies
    (ApplicationId, Name, RoutePattern, HttpMethods, PartitionBy, Algorithm,
     PermitLimit, WindowSeconds, BurstLimit, Priority, Version,
     CreatedBy, CreatedDate, IsActive, IsDeleted)
SELECT
    application.Id, 'Identity API - Google External Login', 'external-login/**', 'GET',
    'IpAddress,Application,Endpoint', 'SlidingWindow', 20, 300, NULL, 190, 1,
    'migration', UTC_TIMESTAMP(6), 1, 0
FROM applications AS application
WHERE application.Code = 'Identity-api'
  AND application.IsDeleted = 0
  AND NOT EXISTS (
      SELECT 1 FROM rate_limit_policies
      WHERE Name = 'Identity API - Google External Login'
  );
