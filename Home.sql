USE schoolNet;
GO

-- ============================================================
--  GetStudentHomePage
--  Returns 3 result sets consumed by clsSqlHomeData.StudentHomeData()
--
--  RS1 – Profile  : StudentID, FullName, Image, Email, UserName
--  RS2 – Courses  : CourseID, Title, Image, FeesPerMonth,
--                   StartDate, EndDate, IsPaid, TotalContents
--  RS3 – Stats    : EnrolledCourses, PaidSubscriptions,
--                   ActiveSubscriptions
-- ============================================================
CREATE OR ALTER PROCEDURE GetStudentHomePage
    @StudentID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- ── RS1 : Profile ────────────────────────────────────────
    SELECT
        s.id AS StudentID,
        TRIM(CONCAT(
            p.FirstName, ' ',
            p.SecondName,
            CASE WHEN p.ThirdName IS NOT NULL THEN ' ' + p.ThirdName ELSE '' END,
            CASE WHEN p.ForthName IS NOT NULL THEN ' ' + p.ForthName ELSE '' END
        )) AS FullName,
        p.Image,
        p.Email,
        s.UserName
    FROM  Student s
    JOIN  Person  p ON p.id = s.PersonID
    WHERE s.id = @StudentID;

    -- ── RS2 : All subscriptions ──────────────────────────────
    -- Returns paid + unpaid, active + expired so the view can
    -- render the correct Unpaid / Active / Expired badge per row.
    SELECT
        c.id            AS CourseID,
        c.Title,
        c.Image,
        c.FeesPerMonth,
        sub.StartDate,
        sub.EndDate,
        sub.IsPaid,
        (SELECT COUNT(1)
         FROM   CourseContent cc
         WHERE  cc.CourseID = c.id) AS TotalContents
    FROM  Subscription sub
    JOIN  Course        c ON c.id = sub.CourseID
    WHERE sub.StudentID = @StudentID;

    -- ── RS3 : Dashboard stats ────────────────────────────────
    SELECT
        COUNT(*)                                                                   AS EnrolledCourses,
        SUM(CASE WHEN sub.IsPaid = 1 THEN 1 ELSE 0 END)                           AS PaidSubscriptions,
        SUM(CASE WHEN sub.IsPaid = 1
                  AND GETDATE() >= sub.StartDate
                  AND (sub.EndDate IS NULL OR sub.EndDate >= GETDATE())
             THEN 1 ELSE 0 END)                                                    AS ActiveSubscriptions
    FROM  Subscription sub
    WHERE sub.StudentID = @StudentID;
END;
GO


-- ============================================================
--  GetTeacherHomePage
--  Returns 3 result sets consumed by clsSqlHomeData.TeacherHomeData()
--
--  RS1 – Profile  : TeacherID, FullName, Image, Email, UserName
--  RS2 – Courses  : CourseID, Title, Image, FeesPerMonth,
--                   Permission, TotalContents, TotalStudents
--  RS3 – Stats    : TotalCourses, TotalStudents, TotalContent
-- ============================================================
CREATE OR ALTER PROCEDURE GetTeacherHomePage
    @TeacherID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- ── RS1 : Profile ────────────────────────────────────────
    SELECT
        t.id AS TeacherID,
        TRIM(CONCAT(
            p.FirstName, ' ',
            p.SecondName,
            CASE WHEN p.ThirdName IS NOT NULL THEN ' ' + p.ThirdName ELSE '' END,
            CASE WHEN p.ForthName IS NOT NULL THEN ' ' + p.ForthName ELSE '' END
        )) AS FullName,
        p.Image,
        p.Email,
        t.UserName
    FROM  Teacher t
    JOIN  Person  p ON p.id = t.PersonID
    WHERE t.id = @TeacherID;

    -- ── RS2 : Courses the teacher handles ────────────────────
    SELECT
        c.id            AS CourseID,
        c.Title,
        c.Image,
        c.FeesPerMonth,
        ch.Permission,
        (SELECT COUNT(1)
         FROM   CourseContent cc
         WHERE  cc.CourseID = c.id)                   AS TotalContents,
        (SELECT COUNT(DISTINCT sub.StudentID)
         FROM   Subscription sub
         WHERE  sub.CourseID = c.id
           AND  sub.IsPaid   = 1)                     AS TotalStudents
    FROM  CourseHandler ch
    JOIN  Course        c ON c.id = ch.CourseID
    WHERE ch.TeacherID = @TeacherID;

    -- ── RS3 : Dashboard stats ────────────────────────────────
    SELECT
        COUNT(DISTINCT ch.CourseID) AS TotalCourses,
        (SELECT COUNT(DISTINCT sub.StudentID)
         FROM   Subscription  sub
         JOIN   CourseHandler ch2 ON ch2.CourseID = sub.CourseID
         WHERE  ch2.TeacherID = @TeacherID
           AND  sub.IsPaid    = 1)                    AS TotalStudents,
        (SELECT COUNT(1)
         FROM   CourseContent cc
         JOIN   CourseHandler ch3 ON ch3.CourseID = cc.CourseID
         WHERE  ch3.TeacherID = @TeacherID)           AS TotalContent
    FROM  CourseHandler ch
    WHERE ch.TeacherID = @TeacherID;
END;
GO
