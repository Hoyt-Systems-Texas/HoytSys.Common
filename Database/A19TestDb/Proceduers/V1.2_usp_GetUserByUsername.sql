IF OBJECT_ID('usp_GetUserByUsername') IS NULL
BEGIN 
    EXEC('CREATE PROCEDURE dbo.usp_GetUserByUsername AS SELECT 1');
END
GO
ALTER PROC dbo.usp_GetUserByUsername 
    @username VARCHAR(100)
AS 
BEGIN 
    DECLARE @userAppId AS TABLE(UserAppId BIGINT);
    INSERT INTO @userAppId
    SELECT
        DISTINCT
        UAV.UserAppId
    FROM
        dbo.UserAppVersion UAV
    WHERE
        Uav.Username = @username;
    
    DECLARE @version AS TABLE(UserAppVersionId BIGINT)
    INSERT INTO @version
    SELECT
        MAX(UAV.UserAppVersionId)
    FROM
         dbo.UserAppVersion UAV
        JOIN @userAppId UAI ON UAV.UserAppId = UAI.UserAppId
    GROUP BY
        UAI.UserAppId
    
    SELECT
        UA.UserAppId,
        UA.UserGuid,
        UAV.Username,
        UAV.Password,
        UAV.CreatedOn [LastModified]
    FROM
        dbo.UserApp UA
        JOIN dbo.UserAppVersion UAV ON UA.UserAppId = UAV.UserAppId
        JOIN @version V ON UAV.UserAppVersionId = V.UserAppVersionId
    WHERE
        UAV.Username = @username
END
GO
