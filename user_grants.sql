USE QuickAid;
ALTER USER qa_user WITH LOGIN = qa_user;

USE QuickAid;
EXEC sp_addrolemember 'db_owner', 'qa_user';


USE QuickAid;
SELECT 
    dp.name AS UserName,
    dp.sid AS UserSID,
    sp.sid AS LoginSID
FROM 
    sys.database_principals dp
LEFT JOIN 
    sys.server_principals sp ON dp.sid = sp.sid
WHERE dp.name = 'qa_user';



USE QuickAid;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO qa_user;


SELECT dp1.name, dp2.name 
FROM sys.database_role_members drm
JOIN sys.database_principals dp1 ON drm.member_principal_id = dp1.principal_id
JOIN sys.database_principals dp2 ON drm.role_principal_id = dp2.principal_id
WHERE dp1.name = 'qa_user';
