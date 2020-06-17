using App.DataAccess.Model;
using App.Web.Models;
using App.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Services.Contracts {
    public interface IWorkflowServices : IServiceBase {
        
        Workflow Upsert(WorkflowForm form);
        Workflow GetById(string id);
        List<ToDoForm> GetToDoList(DateTime? date = null);
        FlowForm SetStatus(string Id, string status);
        Workflow Upsert(IEnumerable<Flow> flows, Workflow form = null, bool saveChanges = true);
        string GetEmailTemplate(AppReminderTypes type);
        Workflow Upsert(IEnumerable<Flow> flows, string dataId, AppReminderTypes type);
        Workflow Delete(string dataId, AppReminderTypes type);
        Workflow RestoreDeleteFlow(string dataId, AppReminderTypes type);
        Workflow GetById(string dataId, AppReminderTypes type);
        void FlowStatusDone(string url);
        List<Flow> GetFlowByDataId(string dataId, AppReminderTypes type);
    }
}
