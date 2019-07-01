IF OBJECT_ID('UserApp') IS NULL
BEGIN 
    CREATE TABLE dbo.UserApp (
        UserAppId BIGINT NOT NULL CONSTRAINT PK_UserApp PRIMARY KEY IDENTITY(1,1),
        UserGuid UNIQUEIDENTIFIER NOT NULL,
        CreatedOn DATETIME NOT NULL,
        CreatedBy UNIQUEIDENTIFIER NOT NULL
    )
    
    CREATE INDEX IX_UserApp_UserGuid ON dbo.UserApp (UserGuid) WITH(FILLFACTOR=90);
END

IF OBJECT_ID('UserAppVersion') IS NULL
BEGIN 
    CREATE TABLE dbo.UserAppVersion (
        UserAppVersion BIGINT NOT NULL CONSTRAINT PK_UserAppVersion PRIMARY KEY IDENTITY(1, 1),
        UserAppId BIGINT NOT NULL,
        Username VARCHAR(50) NOT NULL,
        Password VARCHAR(100) NOT NULL,
        CreatedOn DATETIME NOT NULL,
        CreatedBy UNIQUEIDENTIFIER NOT NULL
    )
    
    CREATE INDEX IX_UserAppVersion_UserAppId ON dbo.UserAppVersion (UserAppId) WITH(FILLFACTOR=90)
    CREATE INDEX IX_UserAppVersion_Username ON dbo.UserAppVersion (Username) WITH(FILLFACTOR=90)
END