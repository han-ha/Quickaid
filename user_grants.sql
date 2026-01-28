USE QuickAid;

-- Nale¿y uzupe³niæ skrypt w³asnym loginem i has³em

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'wlasny_login')
BEGIN
    CREATE LOGIN wlasny_login WITH PASSWORD = 'wlasne_haslo';
END
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'wlasny_login')
BEGIN
    CREATE USER wlasny_login FOR LOGIN wlasny_login;
END
GO

ALTER USER wlasny_login WITH LOGIN = wlasny_login;

EXEC sp_addrolemember 'db_owner', 'wlasny_login';


SELECT 
    dp.name AS UserName,
    dp.sid AS UserSID,
    sp.sid AS LoginSID
FROM 
    sys.database_principals dp
LEFT JOIN 
    sys.server_principals sp ON dp.sid = sp.sid
WHERE dp.name = 'wlasny_login';



USE QuickAid;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO wlasny_login;


SELECT dp1.name, dp2.name 
FROM sys.database_role_members drm
JOIN sys.database_principals dp1 ON drm.member_principal_id = dp1.principal_id
JOIN sys.database_principals dp2 ON drm.role_principal_id = dp2.principal_id
WHERE dp1.name = 'wlasny_login';
