# SchoolNet — Backend

An e-learning platform API: teachers publish video courses, students pay to subscribe and watch.

Built with **ASP.NET Core 8**, **SQL Server** (raw ADO.NET + stored procedures), **Stripe** for payments and **MUX** for video.

> **Project status:** SchoolNet is a work in progress. It runs end to end for the main flows described below, but a lot of the project is still not done. Treat it as a learning project and a base to build on, not as production software.

The matching React frontend lives in its own repository: **schoolnet-FE**.

---

## Contents

- [What it does](#what-it-does)
- [Architecture](#architecture)
- [Class diagrams](#class-diagrams)
- [Sequence diagrams](#sequence-diagrams)
- [Database](#database)
- [API reference](#api-reference)
- [Getting started](#getting-started)
- [Project structure](#project-structure)

---

## What it does

| Role | Can do |
|---|---|
| **Teacher** | Sign up (a Stripe Connect account is created for them), create courses, add video lessons, upload videos |
| **Student** | Sign up, browse courses, pay for a course through Stripe Checkout, watch the videos of courses they paid for |
| **Anyone** | Browse the course list and see the lesson titles of a course |

---

## Architecture

Three layers, each one a separate C# project. A fourth project, `SharedDTOs`, holds the data shapes every layer shares.

```mermaid
flowchart TD
    FE["React frontend<br/>(schoolnet-FE)"]
    API["OnlineSchool_Back-end<br/>Web API: controllers, middleware, DI"]
    BL["OnlineSchoolBusniss<br/>business layer: cls* classes, Stripe and MUX services"]
    DAL["OnlineSchoolDataAccess<br/>data layer: clsSql* classes, one method per stored procedure"]
    DB[("SQL Server<br/>schoolNet")]
    DTO["SharedDTOs<br/>request and response shapes"]
    STRIPE["Stripe"]
    MUX["MUX"]

    FE -->|"HTTP + auth cookie"| API
    API --> BL
    BL --> DAL
    DAL -->|"stored procedures"| DB
    BL --> STRIPE
    BL --> MUX
    STRIPE -.->|"webhook"| API
    MUX -.->|"webhook"| API
    API -.- DTO
    BL -.- DTO
    DAL -.- DTO
```

**Rule of thumb:** a controller never touches SQL, and the data layer never knows about HTTP.

### Patterns used

| Pattern | Where |
|---|---|
| Active Record | `clsCourse`, `clsStudent`, `clsTeacher`, `clsCourseContent`, … each object has its own `Find()` and `Save()` |
| Static data-access classes | `clsSql*Data` wrap `SqlCommand` calls to stored procedures |
| Options pattern | `clsStripeModel` and `clsMUXSigningServiceModel` are bound from configuration |
| Named `HttpClient` | `"muxService"` is pre-configured with MUX credentials |
| Cookie authentication | `401` / `403` status codes instead of login redirects (suits a SPA) |
| Custom middleware | `muxTraficValidatorMiddleware` verifies MUX webhook signatures |

---

## Class diagrams

### Domain classes (business layer)

The `cls*` classes are the core of the app. A `Student` or `Teacher` *has a* `Person` (the personal details), and a course is linked to its teachers through `CourseHandlers`.

```mermaid
classDiagram
    class clsPerson {
        +int? PersonID
        +string FirstName
        +string SecondName
        +string Email
        +string PhoneNumber
        +DateTime? DateOfBirth
        +FullName() string
        +Find(id)$ clsPerson
        +Save() bool
    }

    class clsStudent {
        +int? StudentID
        +string UserName
        +string Password
        +int? StudentLevelID
        +clsPerson PersonalInfo
        +FindStudent(username, password)$ clsStudent
        +Save() bool
    }

    class clsTeacher {
        +int? TeacherID
        +string UserName
        +string Password
        +string TeacherStripeID
        +int? SpecialisationID
        +clsPerson PersonalInfo
        +FindTeacher(username, password)$ clsTeacher
        +Save() bool
    }

    class clsCourse {
        +int? CourseID
        +string CourseTitle
        +string CourseDescription
        +decimal CourseFeePerMonth
        +int? LevelRequiredID
        +Find(id)$ clsCourse
        +GetCoursesList()$ List~CourseDTO~
        +Save() bool
    }

    class clsCourseHandlers {
        +int? TeacherID
        +int? CourseID
        +string Permission
        +GetCourseOfTeacherX(teacherId)$ List~CourseDTO~
        +GetTeachersOfCourse(courseId)$ List~CourseHandlerDTO~
        +Save() bool
    }

    class clsCourseContent {
        +int? CourseContentID
        +int? CourseID
        +string ContentTitle
        +string UploadID
        +string AssetID
        +string AssetStatus
        +string PlayBackID
        +Find(id)$ clsCourseContent
        +FindByUploadID(uploadId)$ clsCourseContent
        +GetContentOfCourseX(courseId)$ List~ContentDTO~
        +Save() bool
    }

    class clsSubscribtion {
        +int? ID
        +int StudentID
        +int CourseID
        +DateTime StartDate
        +DateTime? EndDate
        +bool? IsPaid
        +StudentHasAccessAsync(studentId, courseId)$ Task~bool~
        +GetStudentCoursesAsync(studentId)$ Task~List~CourseDTO~~
        +SaveAsync() Task~bool~
    }

    clsStudent --> clsPerson : PersonalInfo
    clsTeacher --> clsPerson : PersonalInfo
    clsCourseHandlers --> clsTeacher : teacher
    clsCourseHandlers --> clsCourse : course
    clsCourseContent --> clsCourse : belongs to
    clsSubscribtion --> clsStudent : student
    clsSubscribtion --> clsCourse : course
```

### Services and the layers around them

Controllers depend on `IAuthService` and on the domain and service classes. External providers (Stripe, MUX) are wrapped in their own service classes so controllers stay thin.

```mermaid
classDiagram
    class IAuthService {
        <<interface>>
        +SignInUser(claims) Task
        +UserIsAuthenticated() bool
        +GetUserID() string
        +getRole() string
    }
    class AuthService
    IAuthService <|.. AuthService

    class AuthController
    class CourseController
    class ContentController
    class SubscribtionController
    class HomeController
    class StripeWebHookController
    class MuxWebhookController

    class clsStripeService {
        +Pay(course, student) Task~string~
        +CreateConnectedAccount() Task~string~
        +CreateAccountLink(stripeAccountId) Task~string~
        +HandleCheckOutSessionCompleted(session) Task~bool~
    }

    class clsMUXVideoService {
        +CreateAsset() Task~DirectUploadRecord~
        +GetAssetPlayBackID(assetId) Task~string~
        +CreateAssetPlayBackID(assetId) Task~string~
    }

    class clsMUXSigningService {
        +CreateSignedURL(playbackId) string
    }

    class MUXEventService {
        +HandleMuxEvents(body) Task~bool~
    }

    class HomePageService {
        +StudentHomePageData(id) Task~StudentHomeDto~
        +TeacherHomePageData(id) Task~TeacherHomeDto~
    }

    AuthController --> IAuthService
    AuthController --> clsStripeService
    CourseController --> IAuthService
    ContentController --> IAuthService
    ContentController --> clsMUXVideoService
    ContentController --> clsMUXSigningService
    SubscribtionController --> IAuthService
    SubscribtionController --> clsStripeService
    HomeController --> HomePageService
    StripeWebHookController --> clsStripeService
    MuxWebhookController --> MUXEventService
```

---

## Sequence diagrams

### 1. Login

The server keeps the session in an `HttpOnly` cookie. The cookie carries two claims: `UserID` and `User_Role` (`student` or `teacher`). Every protected endpoint reads them through `IAuthService`.

```mermaid
sequenceDiagram
    actor U as User
    participant FE as Frontend
    participant API as AuthController
    participant BL as clsStudent / clsTeacher
    participant DB as SQL Server

    U->>FE: username, password, role
    FE->>API: POST /api/Auth/login
    API->>BL: FindStudent / FindTeacher(username, password)
    BL->>DB: stored procedure
    DB-->>BL: row or nothing
    alt credentials match
        API-->>FE: 200 + Set-Cookie (UserID, User_Role) + user info
    else no match
        API-->>FE: 401 Unauthorized
    end
```

### 2. Teacher sign-up (with Stripe account)

A teacher needs a Stripe Connect account so they can be paid later, so one is created during sign-up and its id is stored on the teacher.

```mermaid
sequenceDiagram
    participant FE as Frontend
    participant API as AuthController
    participant S as clsStripeService
    participant ST as Stripe
    participant BL as clsTeacher / clsPerson
    participant DB as SQL Server

    FE->>API: POST /api/Auth/TeacherSignUp
    API->>S: CreateConnectedAccount()
    S->>ST: create Express account
    ST-->>S: account id
    API->>BL: PersonalInfo.Save()
    BL->>DB: SP_AddNew_Person
    API->>BL: teacher.Save() (with Stripe id)
    BL->>DB: SP_AddNew_Teacher
    API-->>FE: 200 + user info
```

### 3. Create a course

Only teachers can create courses. The creating teacher automatically becomes the course's `admin` through a `CourseHandler` record.

```mermaid
sequenceDiagram
    participant FE as Frontend
    participant API as CourseController
    participant A as IAuthService
    participant C as clsCourse
    participant H as clsCourseHandlers
    participant DB as SQL Server

    FE->>API: POST /api/Course/Add
    API->>A: role and user id from the cookie
    alt not a teacher
        API-->>FE: 401
    else teacher
        API->>C: new clsCourse(request).Save()
        C->>DB: SP_AddNew_Course
        API->>H: CourseHandler(courseId, teacherId, "admin").Save()
        H->>DB: SP_AddNew_CourseHandler
        API-->>FE: 201 Created + course
    end
```

### 4. Pay for a course (Stripe Checkout)

The student is **not** subscribed when they click pay. The subscription is only created later, when Stripe confirms the payment through a webhook. The course and student ids travel inside the checkout session metadata.

```mermaid
sequenceDiagram
    actor S as Student
    participant FE as Frontend
    participant API as SubscribtionController
    participant SS as clsStripeService
    participant ST as Stripe
    participant WH as StripeWebHookController
    participant DB as SQL Server

    S->>FE: click "Subscribe Now"
    FE->>API: POST /api/Subscribtion/Pay (courseId)
    API->>SS: Pay(course, student)
    SS->>ST: create Checkout Session (metadata: course_id, user_id)
    ST-->>SS: session url
    API-->>FE: { checkout_url }
    FE->>ST: redirect the browser to checkout_url
    S->>ST: enters card, pays
    ST->>WH: webhook checkout.session.completed
    WH->>WH: verify Stripe-Signature
    WH->>SS: HandleCheckOutSessionCompleted(session)
    SS->>DB: add Subscription (start now, end +1 month, paid)
    WH-->>ST: 200 OK
```

### 5. Upload a lesson video (MUX direct upload)

Video files never pass through this API. The API only asks MUX for a one-time upload URL, and the browser sends the file straight to MUX. MUX then tells the API what happened through webhooks.

```mermaid
sequenceDiagram
    actor T as Teacher
    participant FE as Frontend
    participant API as ContentController
    participant M as clsMUXVideoService
    participant MUX as MUX
    participant DB as SQL Server

    T->>FE: fill the lesson form, pick a video
    FE->>API: POST /api/Content/Add
    API->>DB: save lesson record
    API-->>FE: contentId
    FE->>API: POST /api/Content/DirectUpload/{contentId}
    API->>M: CreateAsset()
    M->>MUX: create direct upload
    MUX-->>M: upload_id, upload_url
    API->>DB: save UploadID, status "Uploading"
    API-->>FE: upload_url
    FE->>MUX: upload the video in chunks (UpChunk)
```

### 6. MUX webhooks (processing finished)

The lesson record is matched to the MUX event through the `UploadID` that was saved in the previous step. A middleware verifies that the request really came from MUX before the controller sees it.

```mermaid
sequenceDiagram
    participant MUX as MUX
    participant MW as muxTraficValidatorMiddleware
    participant WH as MuxWebhookController
    participant EV as MUXEventService
    participant DB as SQL Server

    MUX->>MW: POST /api/MuxWebhook/webhook
    MW->>MW: read raw body, check Mux-Signature (HMAC SHA-256)
    alt signature invalid
        MW-->>MUX: 401
    else valid
        MW->>WH: forward request
        WH->>EV: HandleMuxEvents(body)
        alt event video.upload.asset_created
            EV->>DB: find lesson by UploadID, save AssetID and status
        else event video.asset.ready
            EV->>DB: find lesson by UploadID, save status "ready" and PlaybackID
        end
        WH-->>MUX: 200
    end
```

### 7. Watch a lesson (signed playback URL)

Playback URLs are signed with a private key and expire, so a video link can't simply be shared or hotlinked. Whether the viewer may watch is decided on the frontend using the role returned by `GET /api/Content/{courseId}` (`guest`, `new_student`, `subscribed_student`, `teacher`).

```mermaid
sequenceDiagram
    actor S as Student
    participant FE as Frontend
    participant API as ContentController
    participant DB as SQL Server
    participant MUX as MUX
    participant SIG as clsMUXSigningService

    S->>FE: open a lesson
    FE->>API: GET /api/Content/ContentID/{id}
    API->>DB: load lesson
    alt not ready yet
        API-->>FE: 404 "not ready yet"
    else ready
        opt no PlaybackID stored yet
            API->>MUX: get or create playback id
            API->>DB: save PlaybackID
        end
        API->>SIG: CreateSignedURL(playbackId)
        SIG-->>API: signed URL (RS256 JWT)
        API-->>FE: lesson details + signed URL
    end
    FE->>MUX: stream the video with the signed URL
```

---

## Database

SQL Server, accessed with **raw ADO.NET and stored procedures** (no Entity Framework, no migrations). Each `.sql` file at the repo root creates the table and the stored procedures of one entity.

```mermaid
erDiagram
    Person ||--o| Student : "is a"
    Person ||--o| Teacher : "is a"
    Teacher ||--o{ CourseHandler : "manages"
    Course ||--o{ CourseHandler : "managed by"
    Course ||--o{ CourseContent : "has lessons"
    Student ||--o{ Subscription : "subscribes"
    Course ||--o{ Subscription : "has subscribers"

    Person {
        int id PK
        string FirstName
        string SecondName
        string Email
        string PhoneNumber
    }
    Student {
        int id PK
        int PersonID FK
        string UserName
        int CurrentLevelID
    }
    Teacher {
        int id PK
        int PersonID FK
        string UserName
        string TeacherStripeID
        int SpecialisationID
    }
    Course {
        int id PK
        string Title
        decimal FeesPerMonth
        string Description
        int LevelRequiredID
    }
    CourseHandler {
        int id PK
        int CourseID FK
        int TeacherID FK
        string Permission
    }
    CourseContent {
        int id PK
        int CourseID FK
        string Title
        string UploadID
        string AssetID
        string PlayBackID
        string Status
    }
    Subscription {
        int id PK
        int StudentID FK
        int CourseID FK
        date StartDate
        date EndDate
        bool IsPaid
    }
```

A database constraint guarantees exactly one `admin` per course (`UX_OneAdminPerCourse` on `CourseHandler`).

---

## API reference

Routes start with `/api`. "Cookie" means the caller must be logged in.

| Method | Route | Cookie | Purpose |
|---|---|---|---|
| `POST` | `/Auth/login` | no | Log in as student or teacher |
| `POST` | `/Auth/StudentSignUp` | no | Register a student |
| `POST` | `/Auth/TeacherSignUp` | no | Register a teacher (creates the Stripe account) |
| `GET` | `/Home` | yes | Dashboard data for the logged-in student or teacher |
| `GET` | `/Course/All` | no | List all courses |
| `GET` | `/Course/{id}` | no | One course and its teachers |
| `GET` | `/Course/My` | yes | Courses the user teaches or is subscribed to |
| `POST` | `/Course/Add` | yes (teacher) | Create a course |
| `GET` | `/Content/{courseId}` | optional | Lessons of a course, plus the caller's role for that course |
| `GET` | `/Content/ContentID/{id}` | yes | One lesson with its signed video URL |
| `POST` | `/Content/Add` | yes | Create a lesson record |
| `POST` | `/Content/DirectUpload/{id}` | yes | Get a MUX upload URL for a lesson |
| `POST` | `/Subscribtion/UserHasAccess` | yes | Does the student have access to a course? |
| `POST` | `/Subscribtion/Pay` | yes | Start a Stripe Checkout, returns `{ checkout_url }` |
| `POST` | `/StripeConnectedAccount/create-account-link` | yes | Stripe onboarding link for a teacher |
| `POST` | `/StripeWebHook` | no | Stripe webhook receiver |
| `POST` | `/MuxWebhook/webhook` | no | MUX webhook receiver (signature checked) |

With the API running in Development, the full interactive list is at `/swagger`.

---

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local, or the Docker image `mcr.microsoft.com/mssql/server`)
- A [Stripe](https://stripe.com) account (test mode is fine)
- A [MUX](https://mux.com) account with an API access token and a signing key

### 1. Create the database

Create a database called `schoolNet`, then run these scripts in this order:

```
Person.sql
Student.sql
Teacher.sql
Course.sql
CourseHandler.sql
CourseContent.sql
Subscription.sql
```

### 2. Configure secrets

Settings are read from `appsettings.json`, and any value can be overridden with an environment variable or with [.NET user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets). **Never commit real keys.** Use placeholders in `appsettings.json` and put the real values in user secrets:

```bash
cd OnlineSchool_web_Back-end-main
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=schoolNet;User Id=sa;Password=<your-password>;TrustServerCertificate=True"
dotnet user-secrets set "Stripe:privateKey" "sk_test_..."
dotnet user-secrets set "Stripe:WebHookKey" "whsec_..."
dotnet user-secrets set "MUXVideoService:muxTokenID" "..."
dotnet user-secrets set "MUXVideoService:muxTokenSecret" "..."
dotnet user-secrets set "MUXVideoService:WebHookKey" "..."
dotnet user-secrets set "MUXSigningService:KeyID" "..."
dotnet user-secrets set "MUXSigningService:SecretKey" "<base64 private key>"
```

### 3. Run

```bash
cd OnlineSchool_web_Back-end-main
dotnet restore
dotnet run
```

- Swagger UI: `/swagger` on the URL printed in the console
- CORS allows the React dev server at `http://localhost:3000`

### 4. Test the webhooks locally

Stripe and MUX need a public URL to reach your machine. Use the [Stripe CLI](https://stripe.com/docs/stripe-cli) (`stripe listen --forward-to localhost:<port>/api/StripeWebHook`) and a tunnel such as `ngrok` for MUX.

---

## Project structure

```
SchoolNet/
├── SchoolNet.slnx
├── OnlineSchool_web_Back-end-main/     Web API (entry point)
│   ├── Controllers/                    one controller per feature
│   ├── Services/                       IAuthService, HomePageService
│   ├── Middlewares/                    request logging, MUX signature check
│   ├── Program.cs                      DI setup and middleware pipeline
│   └── appsettings.json
├── OnlineSchoolBusniss/                business layer
│   ├── cls*.cs                         domain classes (Course, Student, ...)
│   ├── clsStripeService.cs             payments and Connect accounts
│   ├── clsMUXVideoService.cs           direct upload and playback ids
│   ├── clsMUXSigningService.cs         signed playback URLs
│   └── MUXEventService.cs              MUX webhook handling
├── OnlineSchoolDataAccess/             data layer
│   └── clsSql*Data.cs                  one class per table, one method per stored procedure
├── SharedDTOs/                         request and response objects
└── *.sql                               tables and stored procedures
```

### Naming

The code uses `cls` for classes and `clsSql` for data-access classes. That is the project's house style, so new code follows it.
