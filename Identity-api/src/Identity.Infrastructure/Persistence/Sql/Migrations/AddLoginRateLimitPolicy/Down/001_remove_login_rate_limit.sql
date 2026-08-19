DELETE FROM rate_limit_policies
WHERE Name = 'Identity API - Login'
  AND CreatedBy = 'migration';