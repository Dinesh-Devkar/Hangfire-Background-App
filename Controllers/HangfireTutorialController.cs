using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace HangfireWebApiApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HangfireTutorialController : ControllerBase
    {
        [HttpPost]
        [Route("FireAndForgetJob")]
        public IActionResult Welcome(string userName)
        {
            var jobId = BackgroundJob.Enqueue(() => SendWelcomeMail(userName));
            return Ok($"Job Id {jobId} Completed. Welcome Mail Sent!");
        }

        [HttpPost]
        [Route("DelayedJob")]
        public IActionResult DelayedJob(string userName)
        {
            //Here we use datetime object to pass specific date and time
            // var jobId=BackgroundJob.Schedule(()=>SendWelcomeMail(userName),new DateTime(2024,3,31,12,11,00));

            var jobId=BackgroundJob.Schedule(()=>SendWelcomeMail(userName),TimeSpan.FromMinutes(1));

             return Ok($"Job Id {jobId} Completed.Delayed Welcome Mail Sent!");
        }

        [HttpPost]
        [Route("RecuringJob")]
        public IActionResult RecuringJob(string userName)
        {
            RecurringJob.AddOrUpdate(()=>SendInvoiceEmail(userName),Cron.Minutely);
            return Ok("Recuring Job Scheduled Successfully");
        }

        [NonAction]
        public void SendWelcomeMail(string userName)
        {
            //Logic to Mail the user
            Console.WriteLine($"Delayed Welcome to our application, {userName}");
        }

        [NonAction]
        public void SendInvoiceEmail(string userName)
        {
            System.Console.WriteLine($"Invoice Sent To {userName} On Email");
        }



    }
}