




using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OnlineSchoolBusniss;
using SharedDTOs;


namespace OnlineSchool_Back_end.Services
{

    public interface IAuthService
    {
        Task SignInUser(List<Claim> Claims);

        clsStudent AuthenticateStudent(AuthRequest request);
        clsTeacher AuthenticateTeacher(AuthRequest request);
        public bool UserIsAuthenticated();
        public string GetUserID();
        public string getRole();



    }



    public class AuthService : IAuthService
    {




        private readonly IHttpContextAccessor _httpContextAccessor;


        public AuthService(IHttpContextAccessor ctxAccessor)
        {
            _httpContextAccessor = ctxAccessor;

        }


        public async Task SignInUser(List<Claim> Claims)
        {

            var identity = new ClaimsIdentity(Claims, "cookie");
            var user = new ClaimsPrincipal(identity);

            var ctx = _httpContextAccessor.HttpContext;

            if (ctx == null)
            {
                throw new Exception();
            }
            await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);

        }


        public clsStudent AuthenticateStudent(AuthRequest request)
        {

            //find student and return it 
            return clsStudent.FindStudent(request.Username, request.Password);

        }


        public clsTeacher AuthenticateTeacher(AuthRequest request)
        {

            //find teacher and return it 
            return clsTeacher.FindTeacher(request.Username, request.Password);

        }


        public bool UserIsAuthenticated()
        {

            return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

        }

        public string getRole()
        {

            return _httpContextAccessor.HttpContext?.User.FindFirst("User_Role")?.Value;

        }


        public string GetUserID()
        {

            return _httpContextAccessor.HttpContext?.User.FindFirst("UserID")?.Value;

        }
    }

}