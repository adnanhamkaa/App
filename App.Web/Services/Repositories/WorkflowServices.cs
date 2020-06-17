using AutoMapper;
using DocumentFormat.OpenXml;
using Hangfire;
using App.DataAccess;
using App.DataAccess.Identity;
using App.DataAccess.Model;
using App.Web.Models;
using App.Web.Models.Email;
using App.Web.Services.Contracts;
//using JrzAsp.Lib.TypeUtilities;
using App.Web.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace App.Web.Services.Repositories
{
    public class WorkflowServices : ServiceBase, IWorkflowServices {

        IEmailServices _emailSvc;
        IFileManServices _fileManSvc;
        IAppReminderJobServices _reminderSvc;

        public WorkflowServices(ApplicationDbContext context, IEmailServices emailSvc, IFileManServices fileManSvc, IAppReminderJobServices reminderSvc) {
            this.context = context;
            this._emailSvc = emailSvc;
            this._fileManSvc = fileManSvc;
            this._reminderSvc = reminderSvc;
        }
        
        public Workflow Upsert(WorkflowForm form) {
            var workflow = context.Workflows.Include("Flows").Where(t => form.Id == t.Id).FirstOrDefault();

            if(workflow == null) {
                form.SetCreated();
                workflow = Mapper.Map<WorkflowForm, Workflow>(form, workflow);
                workflow.Init();
                form.Flows = form.Flows ?? new List<FlowForm>();

                workflow.Flows = form.Flows.Select(t => {
                    var flow = Mapper.Map<Flow>(t);
                    flow.DueDate = flow.DueDate?.Date;
                    //flow.Init();
                    return flow;
                }).ToList();


                context.Workflows.Add(workflow);

            } else {
                form.SetUpdated();

                var toDelete = workflow.Flows.ToList();
                
                toDelete.ForEach(t => {
                    context.Flows.Remove(t);
                });
                
                form.Flows = form.Flows ?? new List<FlowForm>();

                form.Flows.ToList().ForEach(t => {
                    var flow = Mapper.Map<Flow>(t);
                    //flow.Init();
                    workflow.Flows.Add(flow);
                });
                
            }


            context.SaveChanges();
            
            return workflow;
        }

        public Workflow Upsert(IEnumerable<Flow> flows,Workflow form  = null, bool saveChanges = true) {
            if (form == null) {
                form = new Workflow();
                form.Init();
                form.SetCreated();
                form.Flows = new List<Flow>();
                
                form.Flows = flows.Select(t => {
                    var flow = Mapper.Map<Flow>(t);
                    flow.DueDate = flow.DueDate?.Date;
                    //flow.Init();
                    return flow;
                }).ToList();


                context.Workflows.Add(form);

            } else {
                form.SetUpdated();
                if (form.Flows != null) {
                    var toDelete = form.Flows.ToList();

                    toDelete.ForEach(t => {
                        context.Flows.Remove(t);
                    });
                }

                form.Flows = flows?.ToList() ?? new List<Flow>();
                
            }
            
            if(saveChanges) context.SaveChanges();

            return form;
        }

        public Workflow GetById(string id) {
            return context.Workflows.Include("Flows").Where(t => t.Id == id).FirstOrDefault();
        }

        public Workflow GetById(string dataId, AppReminderTypes type) {
             var typeStr = type.ToString();

            return context.Workflows
                .Include("Flows")
                .FirstOrDefault(t => t.Type == typeStr && t.DataId == dataId);

        }

        public List<Flow> GetFlowByDataId(string dataId, AppReminderTypes type) {
            var typeStr = type.ToString();

            return context.Workflows
                .Include("Flows")
                .FirstOrDefault(t => t.DataId == dataId)?.Flows?.Where(t => t.Type == typeStr).ToList();

        }

        public List<ToDoForm> GetToDoList(DateTime? date = null) {
            var user = GetCurrentUser();

            if (user == null) return new List<ToDoForm>();

            date = date?.Date ?? DateTime.Today.Date;
            var tomorrow = date?.Date.AddWorkDays(1) ?? DateTime.Today.Date.AddWorkDays(1);
            var data = context.Flows.Where(t =>
            t.IsDeleted != true
            && t.IsDraft == false
            &&
            (System.Data.Entity.DbFunctions.TruncateTime(t.DueDate) == date && t.DueDate < tomorrow)
            ).OrderBy(t => t.DueDate).OrderBy(t => t.Order).OrderBy(t => t.Status == "created" ? 0 : 1).ToList();

            var roleIds = user.Roles.Select(t => t.RoleId).ToList();
            
            var roleManager = new RoleManager<ApplicationRole, string>(new RoleStore<ApplicationRole, string, IdentityUserRole>(context));
            var roles = context.Roles.Include("Actions")
                .Where(t => roleIds.Any(r => r == t.Id))
                .ToList()
                .SelectMany(t => t.Actions)
                .Select(t => t.ActionName)
                .Distinct()
                .ToList(); 



            var finaldata = data.Where(t => roles.Any(r => AppReminderHelper.IsTypeExistInGroup(r, t.RoleRecipient)))
                .OrderBy(t => t.Type != null ? 
                (t.Type.StartsWith("Reminder_Equity") ? (t.Type.Contains("Corporate_Action") ? (t.Keterangan?.Contains("Pengumuman") == true ? 1 : (t.Keterangan?.ToLower().Contains("pembuatan") == true ? 2 : 0)) : 1) 
                : t.Type.StartsWith("Reminder_Fixed_Income") ? 3 
                : t.Type.StartsWith("Reminder_Derivative") ? 4
                : 5) 
                : 6)
                .ThenBy(t => t.Keterangan)
                .ToList();

            return Mapper.Map<List<Flow>, List<ToDoForm>>(finaldata);

        }

        public FlowForm SetStatus(string Id,string status) {
            var data = context.Flows.Where(t => t.Id == Id).FirstOrDefault();

            if(data != null) {
                data.Status = status;
            }

            context.SaveChanges();

            return Mapper.Map<FlowForm>(data);
        }

        public string GetEmailTemplate(AppReminderTypes type) {
            var result = "";
            try {
                result = _fileManSvc.ReadTemplate(@"Email\" + type.ToString());

            } catch (Exception exc) {
                ErrorLog(exc);
            }

            return result;
        }

        public Workflow Upsert(IEnumerable<Flow> flows, string dataId, AppReminderTypes type) {

            var typeStr = type.ToString();

            var tomorrow = Tomorrow;
            var notifBeforecount = context.Flows.Count(t =>
                t.IsDeleted != true
                && t.IsDraft == false
                &&
                (System.Data.Entity.DbFunctions.TruncateTime(t.DueDate) == Today && t.DueDate < tomorrow)
            );



            var form = context.Workflows
                .Include("Flows")
                .FirstOrDefault(t => t.Type == typeStr && t.DataId == dataId);

            if (form == null) {
                form = new Workflow();
                form.Init();
                form.SetCreated();
                form.Flows = new List<Flow>();
                form.DataId = dataId;
                form.Type = typeStr;


                form.Flows = flows?.ToList() ?? new List<Flow>();
                foreach (var item in form.Flows) {
                    item.Type = typeStr;
                    item.Status = WorkflowForm.WF_STATUS_CREATED;
                    item.DisplayGrouping = item.DisplayGrouping ?? item.Type;
                }

                //form.Flows = flows.Select(t => {
                //    var flow = Mapper.Map<Flow>(t);
                //    flow.DueDate = flow.DueDate?.Date;
                //    flow.Status = WorkflowForm.WF_STATUS_CREATED;
                //    flow.Type = typeStr;
                //    //flow.Init();
                //    return flow;
                //}).ToList();


                context.Workflows.Add(form);

            } else {
                form.SetUpdated();
                if (form.Flows != null) {
                    var toDelete = form.Flows.ToList();
                    
                    //try {
                    //    foreach (var item in toDelete) {
                    //        BackgroundJob.Enqueue(() => CancelMail(item));
                    //    }
                    //} catch (Exception exc) {
                    //    ErrorLog(exc);
                    //}

                    toDelete.ForEach(t => {
                        context.Flows.Remove(t);
                    });
                }

                form.Flows = flows?.ToList() ?? new List<Flow>();
                foreach (var item in form.Flows) {
                    item.Type = typeStr;
                    item.Status = WorkflowForm.WF_STATUS_CREATED;
                    item.DisplayGrouping = item.DisplayGrouping ?? item.Type;
                }
            }
            
            context.SaveChanges();

            try {
                var notifAftercount = context.Flows.Count(t =>
                    t.IsDeleted != true
                    && t.IsDraft == false
                    &&
                    (System.Data.Entity.DbFunctions.TruncateTime(t.DueDate) == Today && t.DueDate < tomorrow)
                );

                if (notifAftercount != notifBeforecount)
                {
                    BackgroundJob.Enqueue(() => _reminderSvc.DistributeReminder(false));
                }

            } catch (Exception exc) {
                ErrorLog(exc);
            }

            return form;
        }

        public void CancelMail(Flow flow) {

            string[] emails = flow.Recipients?.Split(',');
            if (flow != null && emails != null && emails.Length > 0) {
                string subject = flow.Subject;
                string body = flow.Body;

                ICSFileModel icsFileModel = new ICSFileModel() {
                    Location = "IDX",
                    Subject = subject,
                    Description = subject,
                    StartDate = flow.DueDate ?? DateTime.MinValue,
                    EndDate = flow.DueDate ?? DateTime.MinValue,
                    IcsGuid = flow.EmailGuid,
                    Sequence = ((flow.EmailSequence ?? 1) + 1).ToString(),
                    Method = "CANCEL"
                };

                _emailSvc.SendEmailICS(emails, subject, body, icsFileModel);
            }
        }

        public void SendMail(string flowId) {

            string[] emails = null;

            var flow = context.Flows.FirstOrDefault(t => t.Id == flowId);

            if (flow?.DueDate == null) return;

            flow.EmailGuid = Guid.NewGuid().ToString();
            flow.EmailSequence = 1;
            string subject = flow.Subject;
            string body = flow.Body;
            
            var roleManager = new RoleManager<ApplicationRole, string>(new RoleStore<ApplicationRole, string, IdentityUserRole>(context));
            var role = roleManager.FindByName(flow.RoleRecipient);

            if (role != null) {
                var users = context.Users.Where(t => t.Roles.Any(r => r.RoleId == role.Id)).ToList();

                if (users != null && users.Count > 0) {

                    emails = users.Select(t => t.UserName).ToArray();
                    
                    ICSFileModel icsFileModel = new ICSFileModel() {
                        Location = "IDX",
                        Subject = subject,
                        Description = subject,
                        StartDate = flow.DueDate?.ChangeTime(8,0,0,0) ?? DateTime.MinValue,
                        EndDate = flow.DueDate?.ChangeTime(16, 0, 0, 0) ?? DateTime.MinValue,
                        IcsGuid = flow.EmailGuid,
                        Sequence = 1.ToString(),
                        Method = "REQUEST"
                    };

                    _emailSvc.SendEmailICS(emails, subject, body, icsFileModel);
                    flow.MailSent = true;

                    context.SaveChanges();
                }
            }
        }
        
        public Workflow Delete(string dataId, AppReminderTypes type) {
            var typeStr = type.ToString();

            var form = context.Workflows
                .Include("Flows")
                .FirstOrDefault(t => t.Type == typeStr && t.DataId == dataId);

            var user = GetCurrentUser();

            if (form != null) {
                form.SetUpdated();
                if (form.Flows != null) {
                    var toDelete = form.Flows.ToList();

                    try {
                        foreach (var item in toDelete) {
                            BackgroundJob.Enqueue(() => CancelMail(item));
                        }
                    } catch (Exception exc) {
                        ErrorLog(exc);
                    }

                    foreach (var item in toDelete) {
                        item.Delete(user?.UserName);
                    }
                    
                }

                form.Delete(user?.UserName);

                context.SaveChanges();

            }

            return form;

        }

        public Workflow RestoreDeleteFlow(string dataId, AppReminderTypes type) {
            var typeStr = type.ToString();

            var form = context.Workflows
                .Include("Flows")
                .FirstOrDefault(t => t.Type == typeStr && t.DataId == dataId);

            var user = GetCurrentUser();

            if (form != null) {
                form.SetUpdated();
                if (form.Flows != null) {
                    var toRestoreDelete = form.Flows.ToList();
                    
                    try {
                        foreach (var item in form.Flows.ToList()) {
                            BackgroundJob.Enqueue(() => SendMail(item.Id));
                        }

                    } catch (Exception exc) {
                        ErrorLog(exc);
                    }

                    foreach (var item in toRestoreDelete) {
                        item.SetUpdated();
                        item.IsDeleted = false;
                    }

                }

                form.IsDeleted = false;
                form.SetUpdated();

                context.SaveChanges();

            }

            return form;

        }

        public void FlowStatusDone(string url)
        {
            var today = DateTime.Now.Date;
            var flow = context.Flows.FirstOrDefault(t => t.Url.Equals(url) && t.DueDate == today);
            if (flow != null)
            {
                flow.SetUpdated();
                flow.Status = WorkflowForm.WF_STATUS_DONE;

                context.SaveChanges();
            }
        }
    }
}