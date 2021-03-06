﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WorkflowManager;
using WorkflowManager.Models;

namespace WebApplication1.Controllers
{
    public class ValuesController : ApiController,ILogger
    {
        WorkflowManager.WorkflowManager manager;
        string wfRepoPath = System.Web.Hosting.HostingEnvironment.MapPath("~/WorkflowStore/");
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public WorkflowCommandResponse Post([FromBody]WorkflowCommand command)
        {
            var argument = new Dictionary<string, object>();

            foreach (var parameter in command.Parameters)
            {
                argument.Add(parameter.Name,parameter.Value);
            }

            manager = new WorkflowManager.WorkflowManager(wfRepoPath, true, false, this, this);
            var wfResponse = new WorkflowCommandResponse();
            if (command.WorkflowInstanceID == Guid.Empty)
            {
                wfResponse.InstanceId= manager.NewWorkFlow("Activity3.xaml", argument);
                wfResponse.NextWorkflowInstanceActions = manager.GetBookMarks(wfResponse.InstanceId);

                return wfResponse;
            }
            else
            {

                var wfInstance = manager.LoadWorkFlowWithBookMarkResume(command.WorkflowInstanceID, command.Action.Name,
                    command.Action.Value);
                wfResponse.InstanceId = wfInstance.InstanceId;
                wfResponse.NextWorkflowInstanceActions = wfInstance.Bookmarks;
                return wfResponse;
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        public void Log(string message, LoggerInfoTypes infoType)
        {
            Console.WriteLine("Error:" + message);
        }
    }
}
