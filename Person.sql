

--create database schoolNet;


------Lets Create The DataBase Tables
use schoolNet;


create table Person
(

    id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    NationalNumber nvarchar(50) NOT NULL unique,
    FirstName nvarchar(50) NOT NULL,
    SecondName nvarchar(50) NOT NULL,
    ThirdName nvarchar(50),
    ForthName nvarchar(50),
    Image nvarchar(50),
    Email nvarchar(50),
    DateOfBirth DATE NOT NULL,
    PhoneNumber nvarchar(50) NOT NULL,
    Addresse nvarchar(50)
);
GO


-------PROCEDURES FOR THE PERSON TABLE


---->ADD NEW PERSON
CREATE PROCEDURE SP_AddNew_Person
    @NationalNumber nvarchar(50),
    @FirstName nvarchar(50) ,
    @SecondName nvarchar(50),
    @ThirdName nvarchar(50),
    @ForthName nvarchar(50),
    @Image nvarchar(50),
    @Email nvarchar(50),
    @DateOfBirth DATE ,
    @PhoneNumber nvarchar(50) ,
    @Addresse nvarchar(50),
    @InsertedID int OUT
AS
BEGIN

    INSERT into Person
        (NationalNumber,FirstName,SecondName,ThirdName,ForthName,Image,Email,DateOfBirth,PhoneNumber,Addresse)
    VALUES(@NationalNumber, @FirstName, @SecondName, @ThirdName, @ForthName, @Image, @Email, @DateOfBirth, @PhoneNumber, @Addresse);

    SET @InsertedID = SCOPE_IDENTITY();
END
GO



--->UPDATE PERSON INFORMATION
--CREATE PROCEDURE SP_Update_Person










---> Person Exist By Id
CREATE PROCEDURE SP_Person_Exist
    @PersonID int
AS
BEGIN

    if EXISTS (SELECT 1
    FROM Person
    WHERE Person.id=@PersonID)
        BEGIN

        RETURN 1

    END
    ELSE
        RETURN 0;


END;
GO


----> FIND PERSON BY ID
CREATE PROCEDURE SP_FindByID_Person
    @PersonID int ,
    @NationalNumber nvarchar(50) OUTPUT,
    @FirstName nvarchar(50) OUTPUT,
    @SecondName nvarchar(50) OUTPUT,
    @ThirdName nvarchar(50) OUTPUT,
    @ForthName nvarchar(50) OUTPUT,
    @Image nvarchar(50) OUTPUT,
    @Email nvarchar(50) OUTPUT,
    @DateOfBirth DATE OUTPUT,
    @PhoneNumber nvarchar(50) OUTPUT,
    @Addresse nvarchar(50) OUTPUT
AS
BEGIN

    if EXISTS (SELECT 1
    FROM Person
    WHERE Person.id=@PersonID)
    BEGIN

        SELECT @NationalNumber=NationalNumber
        , @FirstName=FirstName
        , @SecondName=SecondName
        , @ThirdName=ThirdName
        , @ForthName=ForthName
        , @Image=Image
        , @Email=Email
        , @DateOfBirth=DateOfBirth
        , @PhoneNumber=PhoneNumber
        , @Addresse=Addresse
        FROM Person
        where Person.id=@PersonID;
        return 1;

    END

    ELSE
        return 0;
END


GO






