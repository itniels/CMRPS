using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using CMRPS.Web.Controllers;
using CMRPS.Web.Enums;
using CMRPS.Web.Hubs;
using CMRPS.Web.Models;
using Hangfire;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace CMRPS.Web.Core
{
    public partial class Jobs
    {
        /// <summary>
        /// HangFire | Executes schduled actions.
        /// </summary>
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [DisableConcurrentExecution(50)]
        public static void Scheduler()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<ScheduledModel> Schedules = context.Schedules.ToList();
            DayOfWeek currentDay = DateTime.Now.DayOfWeek;

            foreach (ScheduledModel schedule in Schedules)
            {
                bool isToday = false;
                bool hasRun = schedule.LastRun.DayOfWeek == DateTime.Now.DayOfWeek;
                bool isTime = false;
                if (schedule.Active)
                {
                    // Is it today
                    switch (currentDay)
                    {
                        case DayOfWeek.Monday:
                            isToday = schedule.DayMonday;
                            break;
                        case DayOfWeek.Tuesday:
                            isToday = schedule.DayTuesday;
                            break;
                        case DayOfWeek.Wednesday:
                            isToday = schedule.DayWednsday;
                            break;
                        case DayOfWeek.Thursday:
                            isToday = schedule.DayThursday;
                            break;
                        case DayOfWeek.Friday:
                            isToday = schedule.DayFriday;
                            break;
                        case DayOfWeek.Saturday:
                            isToday = schedule.DaySaturday;
                            break;
                        case DayOfWeek.Sunday:
                            isToday = schedule.DaySunday;
                            break;
                    }


                    DateTime runTime = DateTime.Today;
                    runTime = runTime.AddHours(schedule.Hour);
                    runTime = runTime.AddMinutes(schedule.Minute);

                    TimeSpan ts = DateTime.Now - runTime;
                    isTime = ts.TotalSeconds >= 0;

                    if (isToday && !hasRun && isTime)
                    {
                        // Execute schedule
                        ScheduleExecute(schedule.Id);
                    }
                }
            }
        }

        /// <summary>
        /// Scheduler (Helper) | Executes the schedule.
        /// </summary>
        /// <param name="id"></param>
        private static void ScheduleExecute(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ScheduledModel schedule = db.Schedules.SingleOrDefault(x => x.Id == id);
            List<int> computerIds = new List<int>();

            if (schedule.Type == ScheduledType.Individual)
            {
                computerIds = JsonConvert.DeserializeObject<List<int>>(schedule.JsonComputerList);
            }
            else if (schedule.Type == ScheduledType.Color)
            {
                computerIds = db.Colors.SingleOrDefault(x => x.Id == schedule.ColorId).Computers.Select(x => x.Id).ToList();
            }
            else if (schedule.Type == ScheduledType.Location)
            {
                computerIds = db.Locations.SingleOrDefault(x => x.Id == schedule.LocationId).Computers.Select(x => x.Id).ToList();
            }
            else if (schedule.Type == ScheduledType.Type)
            {
                computerIds = db.ComputerTypes.SingleOrDefault(x => x.Id == schedule.TypeId).Computers.Select(x => x.Id).ToList();
            }

            foreach (int cid in computerIds)
            {
                try
                {
                    if (schedule.Action == ScheduledAction.Wakeup)
                    {
                        Core.Actions.SchedulerPowerOn(cid);
                    }
                    else if (schedule.Action == ScheduledAction.Reboot)
                    {
                        Core.Actions.SchedulerPowerRecycle(cid);
                    }
                    else if (schedule.Action == ScheduledAction.Shutdown)
                    {
                        Core.Actions.SchedulerPowerOff(cid);
                    }
                }
                catch (Exception) { }

            }

            // Update database
            schedule.LastRun = DateTime.Now;
            db.Schedules.AddOrUpdate(schedule);
            db.SaveChanges();

            // Call SignalR
            var context = GlobalHost.ConnectionManager.GetHubContext<LiveUpdatesHub>();
            context.Clients.All.UpdateSchedules(id, schedule.LastRun);
            TimeSpan ts = DateTime.Now - schedule.LastRun;
            if (ts.TotalDays < 18250)
            {
                string lastRun = schedule.LastRun.ToShortDateString() + " " + schedule.LastRun.ToShortTimeString();
                context.Clients.All.UpdateSchedules(id, lastRun);
            }
            else
            {
                string lastRun = "Never";
                context.Clients.All.UpdateSchedules(id, lastRun);
            }
        }



    }
}