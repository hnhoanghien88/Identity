DELETE FROM rate_limit_policies
WHERE Name IN (
    'Identity API - Default',
    'Identity API - Authorization',
    'Identity API - Write Operations'
);
