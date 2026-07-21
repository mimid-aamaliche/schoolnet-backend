


------Lets Create The DataBase Tables(Teacher table)
--use schoolNet;

---Teacher Table :


create table Teacher
(

    id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    PersonID int NOT NULL unique,
    TeacherStripeID NVARCHAR(MAX) NULL,
    UserName NVARCHAR(50) NOT NULL unique,
    PassWord NVARCHAR(MAX) NOT NULL,
    CV NVARCHAR(MAX) NULL,
    Notes NVARCHAR(MAX) NULL,

    SpecialisationID int NULL,

    CONSTRAINT FK_Teacher_Person FOREIGN KEY (PersonID) REFERENCES Person(id)
);

GO

use schoolNet;

---->ADD NEW TEACHER
create PROCEDURE SP_AddNew_Teacher
    @PersonID int,
    @TeacherStripeID NVARCHAR(MAX),
    @UserName NVARCHAR(50) ,
    @PassWord NVARCHAR(MAX),
    @CV NVARCHAR(MAX) ,
    @Notes NVARCHAR(MAX) ,
    @SpecialisationID int ,
    @InsertedID int OUTPUT
AS
BEGIN

    BEGIN TRY
    INSERT into Teacher
        (PersonID,TeacherStripeID,UserName,PassWord,CV,SpecialisationID,Notes)
    VALUES
        (@PersonID, @TeacherStripeID, @UserName, @PassWord, @CV, @SpecialisationID, @Notes)

    SET @InsertedID = SCOPE_IDENTITY();

    end TRY
    begin catch

        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();

        SET @InsertedID = -1;

    end catch


END


GO




-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------


--->UPDATE Teacher INFORMATION
--CREATE PROCEDURE SP_Update_Teacher

CREATE PROCEDURE SP_Update_Teacher
    @TeacherID int,
    @PersonID int,
    @TeacherStripeID NVARCHAR(MAX),
    @UserName NVARCHAR(50) ,
    @PassWord NVARCHAR(MAX),
    @CV NVARCHAR(MAX) ,
    @Notes NVARCHAR(MAX) ,
    @SpecialisationID int 
AS
BEGIN
    BEGIN TRY
        UPDATE  Teacher
        SET PersonID=@PersonID,
            TeacherStripeID=@TeacherStripeID,
            UserName=@UserName,
            PassWord=@PassWord,
            CV=@CV,
            SpecialisationID=@SpecialisationID,
            Notes=@Notes
            where Teacher.id=@TeacherID;

        return 1;
    END TRY
    BEGIN CATCH

        --some error log logic here
        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();

        return 0;


    END CATCH
END


GO


-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------



















----Find Teacher  Procedures
------------------------------->BY USERNAME AND PASSWORD




-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------



CREATE PROCEDURE SP_Exist_ByUserNameAndPassword_Teacher
    @UserName NVARCHAR(50) ,
    @PassWord NVARCHAR(MAX)
AS
BEGIN
    BEGIN TRY
       IF EXISTS(select 1 from Teacher where Teacher.UserName=@UserName and Teacher.[PassWord]=@PassWord)
        BEGIN
            return 1;
        END
        ELSE
            return 0;
       
    END TRY
    BEGIN CATCH

        --some error log logic here
        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();

        return 0;


    END CATCH
END

GO

-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------



CREATE PROCEDURE SP_Exist_ByUserName_Teacher
    @UserName NVARCHAR(50) 
AS
BEGIN
    BEGIN TRY
       IF EXISTS(select 1 from Teacher where Teacher.UserName=@UserName)
        BEGIN
            return 1;
        END
        ELSE
            return 0;
       
    END TRY
    BEGIN CATCH

        --some error log logic here
        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();

        return 0;


    END CATCH
END


GO

-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------


create PROCEDURE SP_FindByUserNameAndPassword_Teacher
    @UserName NVARCHAR(50) ,
    @PassWord NVARCHAR(MAX),
    @TeacherID int OUTPUT,
    @PersonID int OUTPUT,
    @TeacherStripeID NVARCHAR(MAX) OUTPUT,
    @CV NVARCHAR(MAX) OUTPUT,
    @Notes NVARCHAR(MAX) OUTPUT,
    @SpecialisationID int OUTPUT

AS
BEGIN
    BEGIN TRY
       IF EXISTS(select 1 from Teacher where Teacher.UserName=@UserName and Teacher.[PassWord]=@PassWord)
        BEGIN
            select  @TeacherID=id,
                    @PersonID=PersonID,
                    @TeacherStripeID=TeacherStripeID,
                    @UserName=UserName,
                    @CV=CV,
                    @SpecialisationID=SpecialisationID, 
                    @Notes=Notes

            from Teacher
            where Teacher.UserName=@UserName and Teacher.[PassWord]=@PassWord;
            return 1;
        END
        ELSE
            return 0;
       
    END TRY
    BEGIN CATCH

        --some error log logic here
        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();

        return 0;


    END CATCH
END
GO

-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------










---------> By Id 


create PROCEDURE SP_FindByID_Teacher
     @TeacherID int ,
    @UserName NVARCHAR(50) OUTPUT ,
    @PassWord NVARCHAR(MAX) OUTPUT,
    @PersonID int OUTPUT,
    @TeacherStripeID NVARCHAR(MAX) OUTPUT,
    @CV NVARCHAR(MAX) OUTPUT,
    @Notes NVARCHAR(MAX) OUTPUT,
    @SpecialisationID int OUTPUT

AS
BEGIN
    BEGIN TRY
       IF EXISTS(select 1 from Teacher where Teacher.id=@TeacherID)
        BEGIN
            select  @UserName=UserName,
                    @PassWord=PassWord,
                    @PersonID=PersonID,
                    @TeacherStripeID=TeacherStripeID,
                    @UserName=UserName,
                    @CV=CV,
                    @SpecialisationID=SpecialisationID, 
                    @Notes=Notes

            from Teacher
            where Teacher.id=@TeacherID ;         
            return 1;
        END
        ELSE
            return 0;
       
    END TRY
    BEGIN CATCH

        --some error log logic here
        PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT 'Error Message: ' + ERROR_MESSAGE();

        return 0;


    END CATCH
END
GO




-----> Not Implimented yet !!!!!!