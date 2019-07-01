INSERT INTO dbo.UserApp
(UserGuid,
 CreatedOn,
 CreatedBy)
VALUES 
       (
        '5e588615-0b28-4e8e-85bd-5f0b6375f88f',
        GETDATE(),
        '5e588615-0b28-4e8e-85bd-5f0b6375f88f'
       )
       
INSERT INTO
    dbo.UserAppVersion
    (UserAppId,
     Username,
     Password,
     CreatedOn,
     CreatedBy)
VALUES 
(
 SCOPE_IDENTITY(),
 'test',
 '$2a$13$.uDfn3RtZsVtxgZ.vcRL.OYTg2s23maES42X4HGsYw1G7xZn5hrMS',
 GETDATE(),
 '5e588615-0b28-4e8e-85bd-5f0b6375f88f'
)