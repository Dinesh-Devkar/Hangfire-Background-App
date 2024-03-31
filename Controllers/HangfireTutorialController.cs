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
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public HangfireTutorialController(IBackgroundJobClient backgroundJobClient,IRecurringJobManager recurringJobManager)
        {
            _backgroundJobClient=backgroundJobClient;
            _recurringJobManager=recurringJobManager;
        }

        
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

        [HttpPost]
        [Route("ContinuationJob")]
        public IActionResult ContinuationJob(string userName)
        {
            var jobId= BackgroundJob.Enqueue(()=>SendWelcomeMail(userName));
            var jobTwo=BackgroundJob.ContinueJobWith(jobId,()=>UnSubscribeEmail(userName));
            return Ok("User UnSubscribe Successfully");
        }

        [NonAction]
        public void SendWelcomeMail(string userName)
        {
            //Logic to Mail the user
            Console.WriteLine($"Welcome to our application, {userName}");
        }

        [NonAction]
        public void SendInvoiceEmail(string userName)
        {
            System.Console.WriteLine($"Invoice Sent To {userName} On Email");
        }

        [NonAction]
        public void UnSubscribeEmail(string userName)
        {
            System.Console.WriteLine($"User {userName} UnSubscribe Successfully");
        }



        //Calling Jobs through Hangfire Interface
        [HttpGet("IFireAndForgetJob")]
        public IActionResult IFireAndForgetJob(string userName)
        {
            var jobId= _backgroundJobClient.Enqueue(()=>System.Console.WriteLine($"Welcome {userName}"));
            return Ok($"JobId {jobId} Execute Successfully");
        }


        [HttpGet]
        [Route("IScheduledJob")]
        public IActionResult IScheduledJob(string userName)
        {
            var jobId=_backgroundJobClient.Schedule(()=>SendWelcomeMail(userName),TimeSpan.FromMinutes(1));
            return Ok($"JobId : {jobId} executed successfuly");
        }

        [HttpGet("IRecurringJob")]
        public IActionResult IRecurringJob(string userName)
        {
            _recurringJobManager.AddOrUpdate("",()=>SendInvoiceEmail(userName),Cron.Minutely);
            return Ok($"Recuring Job executed successfully");
        }

        [HttpGet("IContinationJob")]
        public IActionResult IContinationJob(string userName)
        {
            var jobId= _backgroundJobClient.Enqueue(()=>SendInvoiceEmail(userName));
            _backgroundJobClient.ContinueJobWith(jobId,()=>UnSubscribeEmail(userName));
            return Ok("Contination Job Executed Successfully");
        } 


    }
}