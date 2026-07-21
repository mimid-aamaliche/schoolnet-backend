using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace OnlineSchoolBusniss
{
    public  class clsStripeService
    {

        private readonly clsStripeModel _stripeModel;
        private readonly TokenService _tokenService;
        private readonly CustomerService _customerService;
        private readonly ProductService _productService;
        private readonly AccountService _accountService;
        private readonly AccountLinkService _accountLinkService;

        public clsStripeService( IOptions<clsStripeModel> stripeModel, TokenService tokenService, CustomerService customerService, ProductService productService, AccountService accountService,AccountLinkService accountLinkService)
        {
            _stripeModel = stripeModel.Value;
            _tokenService = tokenService;
            _customerService = customerService;
            _productService = productService;
            _accountService= accountService;
            _accountLinkService = accountLinkService;
        }











        // pay method


        public async Task<string> Pay(clsCourse product, clsStudent customer)
        {

            StripeConfiguration.ApiKey = _stripeModel.privateKey;

            var sessionService=new Stripe.Checkout.SessionService();
            var session = await sessionService.CreateAsync(new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                //Customer = " customer.Id",
                LineItems = new List<SessionLineItemOptions>
                {
                       new SessionLineItemOptions { PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                                                                {
                                                                    Currency = "usd",
                                                                    UnitAmount = (long)(product.CourseFeePerMonth*100), // Amount in cents ($25.00)
                                                                    ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                                                                    {
                                                                        Name = product.CourseTitle // Any name you want
                                                                    }
                                                                }, Quantity = 1 
                       }
                },


                Metadata = new Dictionary<string, string>
                {
                    { "course_id", product.CourseID.ToString() },
                    { "user_id", customer.StudentID.ToString() }
                },


                SuccessUrl = "https://example.com/success",
                CancelUrl = "https://example.com"



            });




            return session.Url;





        }





        //add connected account
        public async Task<string> CreateConnectedAccount()
        {
            
            //flow :
            // when teacher is creating an account in our website 
            //we creat the a connected account for him here
            //and return the stripe account id to be stored in the db

            StripeConfiguration.ApiKey = _stripeModel.privateKey;




            //specifie options
            var Options = new AccountCreateOptions
            {
                Type = "express",
                BusinessType = "individual",
                BusinessProfile = new AccountBusinessProfileOptions
                {
                    Name = "SchoolNet",
                    Url = "https://SchoolNet.com"
                }
            };

            
            //create the account
            var account=await _accountService.CreateAsync(Options);







            //return the account stripe id

            return account.Id;


        }





        //Creat link to an account
        public async Task<string> CreateAccountLink(string UserStripeAccountID)
        {
            StripeConfiguration.ApiKey = _stripeModel.privateKey;


            //hard coded for now
            string RefreshURL = "https://example.com/success";
            string returnURL = "https://example.com/success";

            //specifie options
            var options = new AccountLinkCreateOptions
            {
                Account= UserStripeAccountID,
                RefreshUrl = RefreshURL,
                ReturnUrl = returnURL,
                Type="account_onboarding"

            };

            var accountLink=await _accountLinkService.CreateAsync(options);

            return accountLink.Url;


        }




        //handle checkout.session.completed webhook event
        public async Task<bool> HandleCheckOutSessionCompleted(Stripe.Checkout.Session session)
        {

            //the user has payed the fee amount for that course
            //now add a subscribtion record to our db

            var sessionService = new Stripe.Checkout.SessionService();
            var fullsession = sessionService.Get(session.Id);



            if (!int.TryParse(fullsession.Metadata["course_id"], out int Course_Id_Value))
            {
             
                Console.Error.WriteLine("Invalid or missing course_id in metadata");   
                return false; // or throw a custom exception
            }

            if (!int.TryParse(fullsession.Metadata["user_id"], out int User_Id_Value))
            {
                Console.Error.WriteLine("Invalid or missing user_id in metadata");
                return false;
            }


            //debuging
            Console.WriteLine(Course_Id_Value);
            Console.WriteLine(User_Id_Value);




           
            //-------------NOTE         :
            //for safaty maybe check if user already has a valide subscribtion 
            //if yes pay for the next month after the end date of that subscribtion record




            //create new subscribtion record
            clsSubscribtion SubRecord = new clsSubscribtion()
            {

                _StartDate = DateTime.Now,
                _EndDate = DateTime.Now.AddMonths(1),
                _IsPaid = true,
                _CourseID = Course_Id_Value,
                _StudentID = User_Id_Value
            };



            return SubRecord.Save();


        }









    }
}




// pay flow :
//             1) creat checkout and redirect (by getting the price id from the course table)
//             2) make webhook so that we add  the subscribtion record in case the user payed the amount 



//adding a course flow : 
//             1) add the course into strip env( name , price , ...)
//             2) get the price id and the course id from strip_api and store them with the rest of the info in the db
//             3) 



//adding a teacher flow :( we need to add a connected account )

//adding a student flow (we need to add a customer object to strip)
