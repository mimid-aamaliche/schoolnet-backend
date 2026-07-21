



create table Course
(
    id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Title nvarchar(100) not null,
    FeesPerMonth DECIMAL not null,
    Description nvarchar(MAX) NULL,
    LevelRequiredID INT NULL,
    Image nvarchar(MAX) null


)
GO

use schoolNet;
go



create PROCEDURE SP_AddNew_Course
    @Title nvarchar(100),
    @FeesPerMonth DECIMAL ,
    @Description nvarchar(MAX),
    @LevelRequiredID INT ,
    @Image nvarchar(MAX),
    @InsertedID int OUTPUT
AS
BEGIN
SET NOCOUNT ON;
    BEGIN TRY

        insert into Course(Title,FeesPerMonth,LevelRequiredID,Image,Description)
        Values(@Title, @FeesPerMonth, @LevelRequiredID, @Image, @Description);
        set @InsertedID = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH

    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
    PRINT 'Error Message: ' + ERROR_MESSAGE();

    SET @InsertedID=-1;
    END CATCH


END

GO












create PROCEDURE SP_Update_Course
    @CourseID int ,
    @Title nvarchar(100),
    @FeesPerMonth DECIMAL ,
    @Description nvarchar(MAX),
    @LevelRequiredID INT ,
    @Image nvarchar(MAX)
AS
BEGIN
SET NOCOUNT ON;
    BEGIN TRY

        Update Course
        set Title=@Title,
            FeesPerMonth=@FeesPerMonth,
            LevelRequiredID=@LevelRequiredID,
            Image=@Image,
            Description=@Description 
            where id=@CourseID;

    END TRY
    BEGIN CATCH

    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
    PRINT 'Error Message: ' + ERROR_MESSAGE();




    END CATCH


END;

GO





create PROCEDURE SP_FindByID_Course
    @CourseID int ,
    @Title nvarchar(100) OUTPUT,
    @FeesPerMonth DECIMAL OUTPUT,
    @Description nvarchar(MAX) OUTPUT,
    @LevelRequiredID INT  OUTPUT,
    @Image nvarchar(MAX) OUTPUT
    AS
    BEGIN

     BEGIN TRY
        IF EXISTS(select 1 from Course where Course.id=@CourseID)
        BEGIN
            select  @Title=Title,
                    @FeesPerMonth=FeesPerMonth,
                    @LevelRequiredID=LevelRequiredID,
                    @Image=Image,
                    @Description=Description 
                from Course 
                where Course.id=@CourseID;


            return 1;---course exist in db and the info are on the parametre
        END
        ELSE
            return 0;--- course does not exist in db
       

    END TRY
    BEGIN CATCH

    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    return 0;---an error occured and we can t return the course info 



    END CATCH









    end;

GO


--------Normal List :--------------------------------------------------------------------------------------------------------------
create Procedure SP_GetList_Course
AS
BEGIN


    BEGIN TRY
        select * from Course;    
    END TRY
    BEGIN CATCH

    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
    PRINT 'Error Message: ' + ERROR_MESSAGE();

    END CATCH






END

GO
----------------------------------------------------------------------------------------


