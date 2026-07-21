

------Lets Create The DataBase Tables(Teacher table)
--use schoolNet;

---Teacher Table :

create table Student
(

    id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    PersonID int NOT NULL unique,
    UserName NVARCHAR(50) NOT NULL unique,
    PassWord NVARCHAR(MAX) NOT NULL,
    CurrentLevelID int null ,


    CONSTRAINT FK_Student_Person FOREIGN KEY (PersonID) REFERENCES Person(id)
);

GO



CREATE PROCEDURE SP_AddNew_Student
    @PersonID INT,
    @UserName NVARCHAR(50),
    @PassWord NVARCHAR(MAX),
    @CurrentLevelID INT ,
    @InsertedID INT OUTPUT
AS
BEGIN
    BEGIN TRY
        INSERT INTO Student (PersonID, UserName, PassWord, CurrentLevelID)
        VALUES (@PersonID, @UserName, @PassWord, @CurrentLevelID);
       SET @InsertedID = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
       SET @InsertedID = NULL;
        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();
    END CATCH
END



GO




CREATE PROCEDURE SP_UpDate_Student
    @StudentID INT,
    @PersonID INT,
    @UserName NVARCHAR(50),
    @PassWord NVARCHAR(MAX),
    @CurrentLevelID INT = NULL
AS
BEGIN
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM Student WHERE id = @StudentID)
        BEGIN
            UPDATE Student
            SET PersonID = @PersonID,
                UserName = @UserName,
                PassWord = @PassWord,
                CurrentLevelID = @CurrentLevelID
            WHERE id = @StudentID;

            RETURN 1; 
        END
        ELSE
            RETURN 0;  
    END TRY
    BEGIN CATCH
        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();
        RETURN 0;  -- Failure
    END CATCH
END

GO








CREATE PROCEDURE SP_FindByUserNameAndPassword_Student
    @UserName NVARCHAR(50),
    @PassWord NVARCHAR(MAX),
    @PersonID INT OUTPUT,
    @CurrentLevelID INT OUTPUT,
    @StudentID INT OUTPUT
AS
BEGIN
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM Student WHERE UserName = @UserName AND PassWord = @PassWord)
        BEGIN
            SELECT 
                @PersonID = PersonID,
                @CurrentLevelID = CurrentLevelID,
                @StudentID = id
            FROM Student
            WHERE UserName = @UserName AND PassWord = @PassWord;

            RETURN 1; -- Found
        END
        ELSE
            RETURN 0; -- Not found
    END TRY
    BEGIN CATCH
        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();
        RETURN 0;
    END CATCH
END

GO









CREATE PROCEDURE SP_Student_Exist
    @StudentID INT
AS
BEGIN
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM Student WHERE id = @StudentID)
            RETURN 1; -- found
        ELSE
            RETURN 0; -- does not exist
    END TRY
    BEGIN CATCH
        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();
        RETURN 0;--somthing went wrong 
    END CATCH
END
GO













CREATE PROCEDURE SP_Student_Exist_ByUserNameAndPass
    @UserName NVARCHAR(50),
    @PassWord NVARCHAR(MAX)
AS
BEGIN
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM Student WHERE UserName = @UserName AND PassWord = @PassWord)
            RETURN 1; -- Exists
        ELSE
            RETURN 0; -- Not exists
    END TRY
    BEGIN CATCH
        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();
        RETURN 0;
    END CATCH
END


GO








CREATE PROCEDURE SP_Student_Exist_ByUserName
    @UserName NVARCHAR(50)
AS
BEGIN
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM Student WHERE UserName = @UserName)
            RETURN 1; -- found
        ELSE
            RETURN 0; -- does not exist
    END TRY
    BEGIN CATCH
        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();
        RETURN 0;--problem !!!!!
    END CATCH
END
GO