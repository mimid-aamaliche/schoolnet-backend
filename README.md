# SchoolNet — Backend

> ASP.NET Core 8 Web API powering the SchoolNet e-learning platform.  
> Handles authentication, course management, video streaming (MUX), and payment processing (Stripe).

---

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Project Structure](#project-structure)
- [Tech Stack & Key Packages](#tech-stack--key-packages)
- [Prerequisites](#prerequisites)
- [Database Setup](#database-setup)
- [Configuration](#configuration)
- [Running Locally](#running-locally)
- [API Reference](#api-reference)
- [Authentication](#authentication)
- [External Integrations](#external-integrations)
- [Data Models](#data-models)
- [Known Issues & Unfinished Features](#known-issues--unfinished-features)

---

## Architecture Overview

The backend follows a **3-tier layered architecture** split across four C# projects that all live in this repository:

```
┌─────────────────────────────────────────────────────────────┐
│              OnlineSchool_Back-end  (Web API)                │
│         Controllers · Middlewares · Services · DI            │
└────────────────────────┬────────────────────────────────────┘
                         │  calls
┌────────────────────────▼────────────────────────────────────┐
│              OnlineSchoolBusniss  (Class Library)            │
│   Domain Objects (cls*)  · Stripe Service · MUX Services     │
└────────────────────────┬────────────────────────────────────┘
                         │  calls
┌────────────────────────▼────────────────────────────────────┐
│           OnlineSchoolDataAccess  (Class Library)            │
│     ADO.NET SqlClient · Stored Procedure Wrappers (clsSql*)  │
└────────────────────────┬────────────────────────────────────┘
                         │  SQL
                ┌────────▼────────┐
                │  SQL Server DB   │
                │   (schoolNet)    │
                └─────────────────┘

     All layers share ──►  SharedDTOs  (Class Library)
```

### Design Patterns

| Pattern | Where Used |
|---|---|
| **Active Record** | `clsCourse`, `clsStudent`, `clsTeacher`, `clsCourseContent` — each object owns its `Find()` / `Save()` |
| **Data Access Facade** | Static `clsSql*Data` classes wrap raw `SqlCommand` calls |
| **Options Pattern** | `clsStripeModel`, `clsMUXSigningServiceModel` bound from `appsettings.json` |
| **Named HttpClient** | `"muxService"` client pre-configured with MUX Basic Auth headers |
| **Cookie Authentication** | ASP.NET Core cookie scheme with SPA-friendly 401/403 responses |
| **Custom Middleware** | `muxTraficValidatorMiddleware` — captures raw request body for MUX webhook signature validation |

---

## Project Structure

```
SchoolNet/                                 ← repo root (backend)
│
├── SchoolNet.slnx                         ← solution file
│
├── OnlineSchool_web_Back-end-main/        ← Web API (entry point)
│   ├── Controllers/
│   │   ├── AuthController.cs             ← login, student & teacher signup
│   │   ├── HomeController.cs             ← dashboard data per role
│   │   ├── CourseController.cs           ← CRUD for courses
│   │   ├── ContentController.cs          ← video content + MUX upload
│   │   ├── SubscribtionController.cs     ← Stripe pay + access checks
│   │   ├── StripeConnectedAccountController.cs  ← teacher Stripe onboarding
│   │   ├── StripeWebHookController.cs    ← Stripe webhook handler
│   │   ├── MuxWebhookController.cs       ← MUX webhook handler
│   │   └── StudentController.cs          ← (stub — not yet implemented)
│   ├── Services/
│   │   ├── HomePageService.cs            ← thin orchestration for home data
│   │   └── LogInService.cs              ← IAuthService implementation
│   ├── Middlewares/
│   │   └── muxTraficValidatorMiddleware.cs
│   ├── Program.cs                        ← DI setup, middleware pipeline
│   └── appsettings.json                  ← connection string + secrets (gitignored for dev)
│
├── OnlineSchoolBusniss/                   ← Business / domain layer
│   ├── clsCourse.cs
│   ├── clsCourseContent.cs
│   ├── clsCourseHandlers.cs
│   ├── clsHome.cs
│   ├── clsPerson.cs
│   ├── clsStudent.cs
│   ├── clsTeacher.cs
│   ├── clsSubscribtion.cs
│   ├── clsStripeService.cs               ← Stripe SDK logic
│   ├── clsMUXVideoService.cs             ← MUX asset upload via REST
│   ├── clsMUXSigningService.cs           ← MUX JWT signed URL generation
│   └── MUXEventService.cs               ← MUX webhook event handler
│
├── OnlineSchoolDataAccess/                ← Data access layer
│   ├── DataAccessConfiguration.cs        ← static connection string holder
│   ├── clsSqlCourseData.cs
│   ├── clsSqlCourseContentData.cs
│   ├── clsSqlCourseHandlersData.cs
│   ├── clsSqlHomeData.cs                 ← calls GetStudentHomePage / GetTeacherHomePage SPs
│   ├── clsSqlPersonData.cs
│   ├── clsSqlStudentData.cs
│   ├── clsSqlTeacherData.cs
│   ├── clsSqlSubscribtionsData.cs
│   ├── clsSqlLevelsData.cs
│   └── clsSqlSpecialisationData.cs
│
├── SharedDTOs/                            ← DTOs shared across all layers
│   ├── AddCourseDTO.cs / CourseDTO.cs / CourseDTO_Detailed.cs
│   ├── AddStudentDTO.cs / AddTeacherDTO.cs / AddPersonDTO.cs
│   ├── AuthRequest.cs / AuthResponse.cs / UserDTO.cs
│   ├── ContentDTO.cs / ContentDTO_Detailed.cs / AddContentDTO.cs
│   ├── StudentHomeDto.cs / TeacherHomeDto.cs
│   ├── StudentStatsDto.cs / TeacherStatsDto.cs
│   ├── StudentCourseDto.cs / TeacherCourseDto.cs
│   ├── ProfileDto.cs
│   ├── PaymentRequest.cs / PaymentResponce.cs
│   └── SubscribtionAuthRequestDTO.cs / SubscribtionAuthResponceDTO.cs
│
└── *.sql                                  ← DB schema + stored procedure scripts
    ├── Person.sql
    ├── Student.sql
    ├── Teacher.sql
    ├── Course.sql
    ├── CourseHandler.sql
    ├── CourseContent.sql
    ├── Subscription.sql
    └── Home.sql                           ← GetStudentHomePage + GetTeacherHomePage SPs
```

---

## Tech Stack & Key Packages

| Package | Version | Purpose |
|---|---|---|
| `Microsoft.NET.Sdk.Web` | .NET 8.0 | Web API host |
| `Swashbuckle.AspNetCore` | 6.6.2 | Swagger / OpenAPI |
| `Stripe.net` | 48.5.0 | Stripe payments SDK |
| `Microsoft.Data.SqlClient` | 6.0.2 | SQL Server ADO.NET driver |
| `Microsoft.Extensions.Options` | 9.0.9 | Typed configuration binding |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local or remote)
- A [Stripe](https://stripe.com) account with API keys
- A [MUX](https://mux.com) account with Token ID/Secret and a Signing Key pair

---

## Database Setup

> The project uses **raw ADO.NET with stored procedures**. There are no EF Core migrations.

Run the SQL scripts in the following order against your SQL Server instance:

```sql
-- 1. Create tables and stored procedures
Person.sql
Student.sql
Teacher.sql
Course.sql
CourseHandler.sql
CourseContent.sql
Subscription.sql
Home.sql          -- ← contains GetStudentHomePage & GetTeacherHomePage
```

Each file creates the table and all associated stored procedures for that entity.

---

## Configuration

Copy `appsettings.json` and fill in your values. The `appsettings.Development.json` file is gitignored for safety.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=schoolNet;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Stripe": {
    "privateKey": "sk_test_...",
    "WebHookKey": "whsec_..."
  },
  "MUXVideoService": {
    "muxTokenID": "YOUR_MUX_TOKEN_ID",
    "muxTokenSecret": "YOUR_MUX_TOKEN_SECRET"
  },
  "MUXSigningService": {
    "keyID": "YOUR_SIGNING_KEY_ID",
    "signingKey": "YOUR_BASE64_PRIVATE_KEY"
  }
}
```

---

## Running Locally

```bash
# From the repo root
cd OnlineSchool_web_Back-end-main
dotnet restore
dotnet run
```

- API base URL: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`
- CORS is pre-configured to accept requests from `http://localhost:3000` (React dev server)

---

## API Reference

### Auth — `/api/Auth`

| Method | Route | Body | Description | Auth |
|---|---|---|---|---|
| `POST` | `/api/Auth/login` | `AuthRequest` | Login as student or teacher. Sets auth cookie. | ❌ |
| `POST` | `/api/Auth/StudentSignUp` | `AddStudentDTO` | Register new student | ❌ |
| `POST` | `/api/Auth/TeacherSignUp` | `AddTeacherDTO` | Register new teacher + create Stripe connected account | ❌ |

### Home — `/api/Home`

| Method | Route | Description | Auth |
|---|---|---|---|
| `GET` | `/api/Home` | Returns role-aware dashboard data. Response shape: `{ role: 1\|2, data: StudentHomeDto\|TeacherHomeDto }` | ✅ |

### Courses — `/api/Course`

| Method | Route | Description | Auth |
|---|---|---|---|
| `GET` | `/api/Course/All` | List all courses | ❌ |
| `GET` | `/api/Course/{id}` | Get course detail + teacher handlers | ❌ |
| `GET` | `/api/Course/My` | Get enrolled (student) or managed (teacher) courses | ✅ |
| `POST` | `/api/Course/Add` | Create new course (teachers only) | ✅ |

### Content — `/api/Content`

| Method | Route | Description | Auth |
|---|---|---|---|
| `GET` | `/api/Content/{courseid}` | Get content list for a course + user role context | Optional |
| `GET` | `/api/Content/ContentID/{ContentId}` | Get content details + MUX signed playback URL | ✅ |
| `POST` | `/api/Content/Add` | Create a content record | ✅ |
| `POST` | `/api/Content/DirectUpload/{ContentID}` | Get MUX direct upload URL for video | ✅ |

### Subscriptions — `/api/Subscribtion`

| Method | Route | Description | Auth |
|---|---|---|---|
| `POST` | `/api/Subscribtion/UserHasAccess` | Body: `courseId (int)`. Check if current user has access | ✅ |
| `POST` | `/api/Subscribtion/Pay` | Body: `courseId (int)`. Create Stripe checkout session, returns `{ checkout_url }` | ✅ |

### Stripe — `/api/StripeConnectedAccount` & `/api/StripeWebHook`

| Method | Route | Description | Auth |
|---|---|---|---|
| `POST` | `/api/StripeConnectedAccount/create-account-link` | Generate Stripe Express onboarding URL for teacher | ✅ |
| `POST` | `/api/StripeWebHook` | Stripe webhook receiver (`checkout.session.completed`) | ❌ |

### MUX — `/api/MuxWebhook`

| Method | Route | Description | Auth |
|---|---|---|---|
| `POST` | `/api/MuxWebhook` | MUX webhook receiver (video status events) | ❌ |

---

## Authentication

Cookie-based authentication using `CookieAuthenticationDefaults`.

- **Login**: `POST /api/Auth/login` → sets an `HttpOnly` cookie valid for **1 day**.
- **Logout**: Not yet implemented (clear the cookie client-side or call `HttpContext.SignOutAsync`).
- **Claims stored in cookie**:
  - `UserID` — the student or teacher primary key
  - `User_Role` — `"student"` or `"teacher"`
- **Unauthorized behavior**: Returns `401` (no login redirect — SPA-friendly).
- **Forbidden behavior**: Returns `403`.

---

## External Integrations

### Stripe

Used for **course subscription payments** and **teacher payout accounts**.

| Feature | Flow |
|---|---|
| Teacher signup | Creates a Stripe Express Connected Account. ID stored in `Teacher.TeacherStripeID`. |
| Teacher onboarding | `POST /api/StripeConnectedAccount/create-account-link` returns Stripe onboarding URL. |
| Student payment | `POST /api/Subscribtion/Pay` creates a Checkout Session. Student is redirected to Stripe-hosted page. |
| Payment confirmation | Stripe fires `checkout.session.completed` webhook → `StripeWebHookController` → creates `Subscription` record in DB. |

### MUX Video

Used for **video upload, processing, and secure playback**.

| Feature | Flow |
|---|---|
| Upload | `POST /api/Content/DirectUpload/{id}` → creates MUX asset → returns direct upload URL → frontend uploads file directly to MUX. |
| Processing | MUX fires webhook when asset is ready → `MuxWebhookController` updates `CourseContent.Status` and `AssetID`. |
| Playback | `GET /api/Content/ContentID/{id}` → fetches `PlaybackID` from MUX → generates RS256 JWT signed URL → returns to frontend. |

---

## Data Models

### Entity Relationships

```
Person ──< Student
Person ──< Teacher
Teacher ──< CourseHandler >── Course
Course ──< CourseContent
Course ──< Subscription >── Student
```

### Key Tables

| Table | Key Columns |
|---|---|
| `Person` | `id`, `FirstName`, `SecondName`, `Email`, `PhoneNumber`, `NationalNumber` |
| `Student` | `id`, `PersonID`, `UserName`, `PassWord`, `CurrentLevelID` |
| `Teacher` | `id`, `PersonID`, `UserName`, `PassWord`, `TeacherStripeID`, `SpecialisationID` |
| `Course` | `id`, `Title`, `FeesPerMonth`, `Description`, `Image`, `LevelRequiredID` |
| `CourseHandler` | `id`, `CourseID`, `TeacherID`, `Permission` (`'admin'` default) |
| `CourseContent` | `id`, `CourseID`, `Title`, `ContentType`, `PlayBackID`, `AssetID`, `Status` |
| `Subscription` | `id`, `StudentID`, `CourseID`, `StartDate`, `EndDate`, `IsPaid`, `PaymentID` |

### Stored Procedures (by domain)

<details>
<summary><strong>Person</strong></summary>

- `SP_AddNew_Person`
- `SP_FindByID_Person`
- `SP_Person_Exist`
</details>

<details>
<summary><strong>Student</strong></summary>

- `SP_AddNew_Student`
- `SP_UpDate_Student`
- `SP_FindByUserNameAndPassword_Student`
- `SP_Student_Exist` / `SP_Student_Exist_ByUserNameAndPass` / `SP_Student_Exist_ByUserName`
</details>

<details>
<summary><strong>Teacher</strong></summary>

- `SP_AddNew_Teacher`
- `SP_Update_Teacher`
- `SP_FindByID_Teacher`
- `SP_FindByUserNameAndPassword_Teacher`
- `SP_Exist_ByUserName_Teacher` / `SP_Exist_ByUserNameAndPassword_Teacher`
</details>

<details>
<summary><strong>Course</strong></summary>

- `SP_AddNew_Course`
- `SP_Update_Course`
- `SP_FindByID_Course`
- `SP_GetList_Course`
</details>

<details>
<summary><strong>CourseContent</strong></summary>

- `SP_AddNew_CourseContent`
- `SP_Update_CourseContent`
- `SP_FindByID_CourseContent`
- `SP_GetContentOfCourse_CourseContent`
</details>

<details>
<summary><strong>CourseHandler</strong></summary>

- `SP_AddNew_CourseHandler`
- `SP_Update_CourseHandler`
- `SP_TeacherCourses_CourseHandler`
- `SP_CourseTeachers_CourseHandler`
</details>

<details>
<summary><strong>Subscription</strong></summary>

- `SP_AddNew_Subscription`
- `SP_StudentHasAccessOnCourse_Subscription`
- `SP_StudentEnrolledCourses_Subscription`
</details>

<details>
<summary><strong>Home (Dashboard)</strong></summary>

- `GetStudentHomePage` — returns 3 result sets: Profile · Enrolled Courses · Stats
- `GetTeacherHomePage` — returns 3 result sets: Profile · Managed Courses · Stats
</details>

---

## Known Issues & Unfinished Features

| Feature | Status | Notes |
|---|---|---|
| `StudentController` | ❌ Empty stub | No profile get/update/deactivate endpoints implemented |
| `PUT /api/Course/{id}` | ❌ Missing | Update course endpoint not implemented (frontend edit modal exists) |
| Teacher Account Settings page | ❌ Missing | No frontend page consuming `create-account-link` |
| Stripe success/cancel URLs | ⚠️ Hardcoded | Set to `https://example.com/success` — must be updated before production |
| `_UpdateSubscribtion` async | ⚠️ Sync fallback | `SaveAsync()` update path still calls synchronous update method |
| Password hashing | ⚠️ Plain text | Passwords stored as plain strings — add BCrypt or ASP.NET Identity hashing |
