

use schoolNet


Create Table CourseContent
(
    id int not null IDENTITY(1,1) PRIMARY KEY ,
    Title nvarchar(300) not null,
    ContentType nvarchar(50) not null,
    DateOfRealease DATETIME not null,
    Description nvarchar(MAX) NULL,
    Notes nvarchar(MAX) null,
    CourseID integer not null,
    ContentPath nvarchar(200) null,
    PlayBackID nvarchar(200) null,
    AssetID nvarchar(200) null,
    Status nvarchar(50) null,

    CONSTRAINT FK_Content_Course FOREIGN KEY(CourseID) REFERENCES Course(id)

);
GO


---------------------------------------------------------------------
---------> Add New 
---------------------------------------------------------------------

Create procedure SP_AddNew_CourseContent
    @Title nvarchar(300),
    @ContentType nvarchar(50) ,
    @DateOfRealease DATETIME,
    @Description nvarchar(MAX) ,
    @Notes nvarchar(MAX) ,
    @CourseID integer,
    @ContentPath nvarchar(200) ,
    @PlayBackID nvarchar(200) ,
    @AssetID nvarchar(200) ,
    @Status nvarchar(50) ,
    @InsertedID int output
as
BEGIN
SET NOCOUNT ON;
    Begin TRY
            INSERT INTO CourseContent
        (Title,ContentType,DateOfRealease,Description,Notes,CourseID,ContentPath,PlayBackID,AssetID,Status)
        VALUES
        (@Title, @ContentType, @DateOfRealease, @Description, @Notes, @CourseID, @ContentPath, @PlayBackID, @AssetID, @Status);
            SET @InsertedID = SCOPE_IDENTITY();
    END TRY

    BEGIN CATCH
            PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
            PRINT 'Error Message: ' + ERROR_MESSAGE();
            SET @InsertedID = NULL;
    END CATCH

END
GO







---------------------------------------------------------------------
---------> Update 
---------------------------------------------------------------------


Create procedure SP_Update_CourseContent
    @ContentID int ,
    @Title nvarchar(300),
    @ContentType nvarchar(50) ,
    @DateOfRealease DATETIME,
    @Description nvarchar(MAX) ,
    @Notes nvarchar(MAX) ,
    @CourseID integer,
    @ContentPath nvarchar(200) ,
    @PlayBackID nvarchar(200) ,
    @AssetID nvarchar(200) ,
    @Status nvarchar(50) ,
    @InsertedID int output
as
BEGIN
SET NOCOUNT ON;
    Begin TRY
            
           
                UPDATE CourseContent
                SET         Title=@Title,
                            ContentType=@ContentType,
                            DateOfRealease=@DateOfRealease,
                            Description=@Description,
                            Notes=@Notes,
                            CourseID=@CourseID,
                            ContentPath=@ContentPath,
                            PlayBackID=@PlayBackID,
                            AssetID=@AssetID,
                            Status=@Status
                WHERE id=@ContentID;
            if @@ROWCOUNT = 0
                return -1;
            ELSE
                return 1;
             


        END TRY

        BEGIN CATCH
            PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
            PRINT 'Error Message: ' + ERROR_MESSAGE();
            RETURN ERROR_NUMBER();
        END CATCH

END
GO



---------------------------------------------------------------------
---------> Find By ID 
---------------------------------------------------------------------



create procedure SP_FindByID_CourseContent
    @ContentID int ,
    @Title nvarchar(300) output,
    @ContentType nvarchar(50) output,
    @DateOfRealease DATETIME output,
    @Description nvarchar(MAX) output,
    @Notes nvarchar(MAX) output,
    @CourseID integer output,
    @ContentPath nvarchar(200) output,
    @PlayBackID nvarchar(200) output,
    @AssetID nvarchar(200) output ,
    @Status nvarchar(50) output
as
BEGIN
SET NOCOUNT ON;
        Begin TRY
            if EXISTS (select 1 from CourseContent where id=@ContentID)
            BEGIN

                select      @Title=Title,
                            @ContentType=ContentType,
                            @DateOfRealease=DateOfRealease,
                            @Description=Description,
                            @Notes=Notes,
                            @CourseID=CourseID,
                            @ContentPath=ContentPath,
                            @PlayBackID=PlayBackID,
                            @AssetID=AssetID,
                            @Status=Status
                from CourseContent
                where id=@ContentID;
                return 1;

            END
            ELSE
                return 0;
        END TRY

        BEGIN CATCH
            PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
            PRINT 'Error Message: ' + ERROR_MESSAGE();
            RETURN ERROR_NUMBER();
        END CATCH

END
GO









---------------------------------------------------------------------
---------> Get Course X Content
---------------------------------------------------------------------

--note here we might use the pagination 
--and we should return only the Assets that are preparing



create PROCEDURE SP_GetContentOfCourse_CourseContent
    @CourseID int 
    AS
    BEGIN
        BEGIN TRY
            SELECT * FROM CourseContent where CourseID=@CourseID;
        END TRY
        BEGIN CATCH
             PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
            PRINT 'Error Message: ' + ERROR_MESSAGE();
        END CATCH
        

    END






---------------------------------------------------------------------
---------> 
---------------------------------------------------------------------

