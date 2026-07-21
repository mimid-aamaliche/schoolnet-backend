
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
    UploadID nvarchar(200) null,        -- MUX direct upload ID (stored when upload URL is generated)
    Status nvarchar(50) null,           -- 'Uploading' | 'preparing' | 'ready' | 'errored'

    CONSTRAINT FK_Content_Course FOREIGN KEY(CourseID) REFERENCES Course(id)

);
GO

-- ── If the table already exists, just add the column ──────────────────────────
-- ALTER TABLE CourseContent ADD UploadID nvarchar(200) NULL;
-- GO


---------------------------------------------------------------------
---------> Add New 
---------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SP_AddNew_CourseContent
    @Title nvarchar(300),
    @ContentType nvarchar(50),
    @DateOfRealease DATETIME,
    @Description nvarchar(MAX),
    @Notes nvarchar(MAX),
    @CourseID integer,
    @ContentPath nvarchar(200),
    @PlayBackID nvarchar(200),
    @AssetID nvarchar(200),
    @UploadID nvarchar(200),
    @Status nvarchar(50),
    @InsertedID int output
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO CourseContent
            (Title, ContentType, DateOfRealease, Description, Notes, CourseID, ContentPath, PlayBackID, AssetID, UploadID, Status)
        VALUES
            (@Title, @ContentType, @DateOfRealease, @Description, @Notes, @CourseID, @ContentPath, @PlayBackID, @AssetID, @UploadID, @Status);
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

CREATE OR ALTER PROCEDURE SP_Update_CourseContent
    @ContentID int,
    @Title nvarchar(300),
    @ContentType nvarchar(50),
    @DateOfRealease DATETIME,
    @Description nvarchar(MAX),
    @Notes nvarchar(MAX),
    @CourseID integer,
    @ContentPath nvarchar(200),
    @PlayBackID nvarchar(200),
    @AssetID nvarchar(200),
    @UploadID nvarchar(200),
    @Status nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE CourseContent
        SET Title         = @Title,
            ContentType   = @ContentType,
            DateOfRealease= @DateOfRealease,
            Description   = @Description,
            Notes         = @Notes,
            CourseID      = @CourseID,
            ContentPath   = @ContentPath,
            PlayBackID    = @PlayBackID,
            AssetID       = @AssetID,
            UploadID      = @UploadID,
            Status        = @Status
        WHERE id = @ContentID;

        IF @@ROWCOUNT = 0
            RETURN -1;
        ELSE
            RETURN 1;
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

CREATE OR ALTER PROCEDURE SP_FindByID_CourseContent
    @ContentID int,
    @Title nvarchar(300) output,
    @ContentType nvarchar(50) output,
    @DateOfRealease DATETIME output,
    @Description nvarchar(MAX) output,
    @Notes nvarchar(MAX) output,
    @CourseID integer output,
    @ContentPath nvarchar(200) output,
    @PlayBackID nvarchar(200) output,
    @AssetID nvarchar(200) output,
    @UploadID nvarchar(200) output,
    @Status nvarchar(50) output
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM CourseContent WHERE id = @ContentID)
        BEGIN
            SELECT  @Title          = Title,
                    @ContentType    = ContentType,
                    @DateOfRealease = DateOfRealease,
                    @Description    = Description,
                    @Notes          = Notes,
                    @CourseID       = CourseID,
                    @ContentPath    = ContentPath,
                    @PlayBackID     = PlayBackID,
                    @AssetID        = AssetID,
                    @UploadID       = UploadID,
                    @Status         = Status
            FROM CourseContent
            WHERE id = @ContentID;
            RETURN 1;
        END
        ELSE
            RETURN 0;
    END TRY
    BEGIN CATCH
        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();
        RETURN ERROR_NUMBER();
    END CATCH
END
GO


---------------------------------------------------------------------
---------> Find By UploadID  (used by MUX webhook handlers)
---------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SP_FindByUploadID_CourseContent
    @UploadID nvarchar(200),
    @ContentID int output,
    @Title nvarchar(300) output,
    @ContentType nvarchar(50) output,
    @DateOfRealease DATETIME output,
    @Description nvarchar(MAX) output,
    @Notes nvarchar(MAX) output,
    @CourseID integer output,
    @ContentPath nvarchar(200) output,
    @PlayBackID nvarchar(200) output,
    @AssetID nvarchar(200) output,
    @Status nvarchar(50) output
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM CourseContent WHERE UploadID = @UploadID)
        BEGIN
            SELECT  @ContentID      = id,
                    @Title          = Title,
                    @ContentType    = ContentType,
                    @DateOfRealease = DateOfRealease,
                    @Description    = Description,
                    @Notes          = Notes,
                    @CourseID       = CourseID,
                    @ContentPath    = ContentPath,
                    @PlayBackID     = PlayBackID,
                    @AssetID        = AssetID,
                    @Status         = Status
            FROM CourseContent
            WHERE UploadID = @UploadID;
            RETURN 1;
        END
        ELSE
            RETURN 0;
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

CREATE OR ALTER PROCEDURE SP_GetContentOfCourse_CourseContent
    @CourseID int
AS
BEGIN
    BEGIN TRY
        SELECT * FROM CourseContent WHERE CourseID = @CourseID;
    END TRY
    BEGIN CATCH
        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();
    END CATCH
END
GO
