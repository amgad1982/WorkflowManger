using System;
using System.Activities;
using System.Activities.Validation;
using System.Activities.XamlIntegration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xaml;
using System.Xml;

namespace WorkflowManager
{
    public class InstanceRepository : IInstanceRepository
    {
        SqlConnection _connection;
        SqlCommand _command;
        ILogger _errorLogger;
        string _workflowStorePath;
        public InstanceRepository(string connectionString,string storePath,ILogger errorLogger)
        {
            this._errorLogger = errorLogger;
            this._workflowStorePath = storePath;
            this._connection = new SqlConnection(connectionString);
            _command = _connection.CreateCommand();
        }


        public WorkflowInstance LoadWorkflowInstance(Guid instanceId)
        {
            _command.Parameters.Clear();
            var sqlstring = "select * from WorkflowInstances where InstanceId=@InstanceId";
            this._command.CommandText = sqlstring;
            this._command.Parameters.AddWithValue("@InstanceId", instanceId);

            try
            {
                WorkflowInstance insttance=new WorkflowInstance("") ;
                _connection.Open();
                using (var reader = _command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        insttance.WorkflowName = reader["WorkflowName"].ToString();
                        insttance.State = (InstanceState)reader["InstanceStatus"];
                        insttance.InstanceId = (Guid)reader["InstanceId"];
                        insttance.SetBookMarks(reader["InstanceNextBookMarks"].ToString().Split(',').ToList());
                    }
                };
                return insttance;
            }
            catch (Exception e)
            {
                this._errorLogger.Log(e.Message, LoggerInfoTypes.Error);
                return null;
            }

        }

        public void SaveWorkflowInstanceState(Guid instanceId,string workflowName, InstanceState instanceStatus, string instanceNextBookMarks)
        {
            _command.Parameters.Clear();
            var sqlstring = "Insert into WorkflowInstances (InstanceId,WorkflowName,InstanceStatus,InstanceNextBookMarks) values (@InstanceId,@WorkflowName,@InstanceStatus,@InstanceNextBookMarks)";
            this._command.CommandText = sqlstring;
            
            this._command.Parameters.AddWithValue("@InstanceId", instanceId);
            this._command.Parameters.AddWithValue("@WorkflowName", workflowName);
            this._command.Parameters.AddWithValue("@InstanceStatus", instanceStatus);
            object p = string.IsNullOrEmpty(instanceNextBookMarks) ? (object)DBNull.Value : instanceNextBookMarks;
            this._command.Parameters.AddWithValue("@InstanceNextBookMarks", p);

            try
            {
                _connection.Open();
                _command.ExecuteNonQuery();
            }catch(Exception e)
            {
                this._errorLogger.Log(e.Message, LoggerInfoTypes.Error);
            }
            finally
            {
                _connection.Close();
            }
        }

        public void UpdateWorkflowInstanceState(Guid id,InstanceState instanceStatus, string instanceNextBookMarks)
        {
            _command.Parameters.Clear();
            var sqlstring = "update  WorkflowInstances set InstanceStatus=@InstanceStatus,InstanceNextBookMarks=@InstanceNextBookMarks where InstanceId=@id ";
            this._command.CommandText = sqlstring;

            this._command.Parameters.AddWithValue("@id", id);
            this._command.Parameters.AddWithValue("@InstanceStatus", instanceStatus);
            object p = string.IsNullOrEmpty(instanceNextBookMarks) ? (object)DBNull.Value : instanceNextBookMarks;
            this._command.Parameters.AddWithValue("@InstanceNextBookMarks", p);

            try
            {
                _connection.Open();
                _command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                this._errorLogger.Log(e.Message, LoggerInfoTypes.Error);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
