

use schoolNet
create table Subscription
(
    id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    StartDate date not null,
    EndDate date null,
    IsPaid bit null,
    PaymentID int null,
    CourseID int not null,
    StudentID int not null,

    CONSTRAINT FK_Subscription_Student FOREIGN KEY (StudentID) references Student(id),
    CONSTRAINT FK_Subscription_Course FOREIGN KEY (CourseID) references Course(id)
)
GO
----------------------------------------------------------------------
---------> add new subscription
----------------------------------------------------------------------

create PROCEDURE SP_AddNew_Subscription
    @StartDate date ,
    @EndDate date ,
    @IsPaid bit ,
    @PaymentID int ,
    @CourseID int ,
    @StudentID int 
    AS
    BEGIN

        BEGIN TRY

            INSERT INTO Subscription(StartDate,EndDate,IsPaid,PaymentID,CourseID,StudentID)
            VALUES(@StartDate,@EndDate,@IsPaid,@PaymentID,@CourseID,@StudentID)

            return SCOPE_IDENTITY();

        END TRY
        BEGIN CATCH


            PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
            PRINT 'Error Message: ' + ERROR_MESSAGE();


            return NULL;

        END CATCH
    END
    
GO


















----------------------------------------------------------------------
---------> student x has access to course Y
----------------------------------------------------------------------


Create PROCEDURE SP_StudentHasAccessOnCourse_Subscription
    @StudentID int ,
    @CourseID int 
    AS
    BEGIN
        SET NOCOUNT ON;
        BEGIN TRY
            IF EXISTS(SELECT 1 FROM Subscription 
                        where   Subscription.StudentID=@StudentID  
                            and  Subscription.CourseID=@CourseID
                            AND  GETDATE()>=Subscription.StartDate
                            and (Subscription.EndDate is null or Subscription.EndDate<=GETDATE())
                            AND Subscription.IsPaid is not null
                            AND Subscription.IsPaid = 1 )
                BEGIN

                    return 1;

                END
            ELSE
                BEGIN
                    RETURN 0;
                END

        END TRY
        BEGIN CATCH
            PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
            PRINT 'Error Message: ' + ERROR_MESSAGE();
            return -1;

        END CATCH
    END

GO










----------------------------------------------------------------------
---------> get subscriptions to course X 
----------------------------------------------------------------------







--not implimented yet 






----------------------------------------------------------------------
---------> get subscriptions to student x 
----------------------------------------------------------------------
--not implimented yet 











----------------------------------------------------------------------
---------> get Student X Enrolled Courses
----------------------------------------------------------------------


Create PROCEDURE SP_StudentEnrolledCourses_Subscription
    @StudentID int 
    AS
    BEGIN
        SET NOCOUNT ON;
        BEGIN TRY
            SELECT Course.* FROM Subscription inner join Course on Subscription.CourseID = Course.id
                        where   Subscription.StudentID=@StudentID  
                            AND  GETDATE()>=Subscription.StartDate
                            and (Subscription.EndDate is null or Subscription.EndDate >= GETDATE())
                            AND Subscription.IsPaid is not null
                            AND Subscription.IsPaid = 1 ;
            return 1;
        END TRY
        BEGIN CATCH


            PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
            PRINT 'Error Message: ' + ERROR_MESSAGE();


            return -1;

        END CATCH






    END


GO


