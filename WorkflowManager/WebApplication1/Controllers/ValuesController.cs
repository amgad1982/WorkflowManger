using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;
using WorkflowManager;

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
        public Guid Post([FromBody]WorkFlowCommand command)
        {
            var argument = new Dictionary<string, object>();
            argument.Add("UserName", command.UserName);
            argument.Add("Password", command.Password);
            argument.Add("Result", "");
            manager = new WorkflowManager.WorkflowManager(wfRepoPath, true, false, this, this);
            if (command.InstanceId == Guid.Empty)
            {
                return manager.NewWorkFlow("Activity1.xaml", argument);
            }
            else
            {
                return manager.LoadWorkFlowWithBookMarkResume(command.InstanceId, "ReEnter", argument).InstanceId;
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

        }
    }
}
