

Create Table CourseHandler(
    id int not null IDENTITY(1,1) PRIMARY KEY ,
    CourseID int not null ,
    TeacherID int not null,
    Permission nvarchar(50) not null default 'admin',

    constraint FK_CourseHandler_Course FOREIGN KEY (CourseID) REFERENCES Course(id),
    constraint FK_CourseHandler_Teacher FOREIGN KEY (TeacherID) REFERENCES Teacher(id)
)


--to make sure we alwayse have one admin per course
Create unique index UX_OneAdminPerCourse on CourseHandler(CourseID) where Permission='admin';
GO





--------------------------------------------------------------------------------------
----------> Add New 
--------------------------------------------------------------------------------------




Create procedure SP_AddNew_CourseHandler
    @CourseID int ,
    @TeacherID int ,
    @Permission nvarchar(50) ,
    @InsertedID int OUTPUT
    AS
    BEGIN
        begin try
            INSERT into CourseHandler(CourseID,TeacherID,Permission)
            Values (@CourseID,@TeacherID,@Permission);
            set @InsertedID=SCOPE_IDENTITY();

        END try 
        begin CATCH
            PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
            PRINT 'Error Message: ' + ERROR_MESSAGE();
            SET @InsertedID = NULL;
        end catch 


    END
    GO








--------------------------------------------------------------------------------------
----------> Update 
--------------------------------------------------------------------------------------




Create procedure SP_Update_CourseHandler
    @RecordID int,
    @CourseID int ,
    @TeacherID int ,
    @Permission nvarchar(50) ,
    @InsertedID int OUTPUT
    AS
    BEGIN
        begin try
            Update CourseHandler
            set CourseID=@CourseID,
                TeacherID=@TeacherID,
                Permission=@Permission
             where id= @RecordID ;
             return 1; 
        END try 
        begin CATCH
            PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
            PRINT 'Error Message: ' + ERROR_MESSAGE();
            return -1;
        end catch 


    END
    GO








--------------------------------------------------------------------------------------
----------> Get Courses of teacher x
--------------------------------------------------------------------------------------


Create procedure SP_TeacherCourses_CourseHandler
    @TeacherID int
    AS
    BEGIN
        begin try
           select Course.* , CourseHandler.Permission as Permission 
           from CourseHandler inner join Course on CourseHandler.CourseID = Course.id
           where CourseHandler.TeacherID=@TeacherID;
        END try 
        begin CATCH
            PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
            PRINT 'Error Message: ' + ERROR_MESSAGE();
        end catch 


    END
    GO

















--------------------------------------------------------------------------------------
----------> Get Teachers Of Course X 
--------------------------------------------------------------------------------------


Create procedure SP_CourseTeachers_CourseHandler
    @CourseID int
    AS
    BEGIN
        begin try

            select CourseHandler.TeacherID , CourseHandler.Permission , CONCAT(Person.FirstName,Person.SecondName) as TeacherName 
            from CourseHandler 
            inner join Teacher On  CourseHandler.TeacherID = Teacher.id 
            inner join Person on Teacher.PersonID = Person.id
            where CourseHandler.CourseID=@CourseID;

        END try 
        begin CATCH
            PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
            PRINT 'Error Message: ' + ERROR_MESSAGE();
        end catch 


    END
    GO


