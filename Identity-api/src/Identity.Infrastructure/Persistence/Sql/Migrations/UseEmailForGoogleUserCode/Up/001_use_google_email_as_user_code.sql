UPDATE users AS user
INNER JOIN external_identities AS identity
    ON identity.UserId = user.Id
   AND identity.Provider = 'Google'
SET user.Code = TRIM(identity.EmailAtLinkTime),
    user.UpdatedBy = 'migration',
    user.UpdatedDate = UTC_TIMESTAMP(6)
WHERE user.Code <> TRIM(identity.EmailAtLinkTime);
