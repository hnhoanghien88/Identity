UPDATE users AS user
INNER JOIN external_identities AS identity
    ON identity.UserId = user.Id
   AND identity.Provider = 'Google'
SET user.Code = CONCAT(
        LOWER(LEFT(identity.Provider, 1)),
        '_',
        LOWER(LEFT(SHA2(identity.ProviderSubject, 256), 48))
    ),
    user.UpdatedBy = 'migration',
    user.UpdatedDate = UTC_TIMESTAMP(6);
