using Lib;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Antibot.Tests
{
    [TestFixture]
    public class Tests
    {

        //[Test]
        public void Test1()
        {
            var x = @"12;""33;2"";4dsfsd;""""f234""".SplitOutsideQuotes(';', false, false, false);
        }

        [Test]
        public async Task Test2()
        {
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            // grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();

            IScheduler scheduler =  await factory.GetScheduler();

            
            // and start it off
            await scheduler.Start();

            // define the job and tie it to HelloJob
            IJobDetail job = JobBuilder
                .Create<HelloJob>()
                .WithIdentity("job1", "group1")
                .Build();

            // trigger the job to run now, and then repeat every 10 seconds
            ITrigger trigger = TriggerBuilder
                .Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(1).RepeatForever())
                .Build();

            // tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);

            // You could also schedule multiple triggers for the same job with
            // await scheduler.ScheduleJob(job, new List<ITrigger>() { trigger1, trigger2 }, replace: true);




            // some sleep to show what's happening
            await Task.Delay(TimeSpan.FromSeconds(25));



            // and last shut down the scheduler when you are ready to close your program
            await scheduler.Shutdown();
        }

        private class ConsoleLogProvider: ILogProvider
        {
            public Logger GetLogger(string name)
            {
                return (level, func, exception, parameters) =>
                {
                    if (level >= LogLevel.Info && func != null)
                    {
                        Debug.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                    }
                    return true;
                };
            }

            public IDisposable OpenNestedContext(string message)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenMappedContext(string key, string value)
            {
                throw new NotImplementedException();
            }
        }

        

        public class HelloJob : IJob
        {
            

            public async Task Execute(IJobExecutionContext context)
            {
                //int count = 0; // (int?)context.Get("count") ?? 0;
                
                Debug.WriteLine($"{context.FireInstanceId} start");
                await Task.Delay(TimeSpan.FromSeconds(10));
                Debug.WriteLine($"{context.FireInstanceId} end");

                //count++;

                //context.Put("count", count);

            }
        }
    }
}