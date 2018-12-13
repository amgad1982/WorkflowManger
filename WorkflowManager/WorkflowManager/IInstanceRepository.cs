using System;
using System.Activities;
using System.Activities.Validation;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using System.Xml;

namespace WorkflowManager
{
    public interface IInstanceRepository
    {
        WorkflowDefinition LoadWorkflowDefinition(string name, double version);
        void SaveWorkflowInstanceState(Guid instanceId,int instanceDefinitionId, InstanceState instanceStatus, string instanceNextBookMarks);
    }

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
        public WorkflowDefinition LoadWorkflowDefinition(string name, double version)
        {
            var sqlstring = "select * from WorkflowDefinitions where WorkflowName=@name and WorkflowVersion=@version";
            this._command.CommandText = sqlstring;
            this._command.Parameters.AddWithValue("@name", name);
            this._command.Parameters.AddWithValue("@version", version);

            try
            {
                var definition = new WorkflowDefinition();
                _connection.Open();
                using (var reader = _command.ExecuteReader(CommandBehavior.CloseConnection)) {
                    definition.Id = (int)reader["Id"];
                    definition.Name = reader["WorkflowName"].ToString();
                    definition.Version = (double)reader["WorkflowVersion"];
                    definition.Activity = LoadWorkflowActiovityFromXaml(_workflowStorePath + definition.Name);
                };
                return definition;
            }
            catch(Exception e)
            {
                this._errorLogger.Log(e.Message, LoggerInfoTypes.Error);
                return null;
            }
            
        }

        public void SaveWorkflowInstanceState(Guid instanceId,int instanceDefinitionId, InstanceState instanceStatus, string instanceNextBookMarks)
        {
            var sqlstring = "Insert into WorkflowInstances (InstanceID,InstanceDefinition,InstanceStatus,InstanceNextBookMarks) values (@InstanceID,@InstanceDefinition,@InstanceStatus,@InstanceNextBookMarks)";
            this._command.CommandText = sqlstring;
            
            this._command.Parameters.AddWithValue("@InstanceID", instanceId);
            this._command.Parameters.AddWithValue("@InstanceDefinition", instanceDefinitionId);
            this._command.Parameters.AddWithValue("@InstanceStatus", instanceStatus);
            this._command.Parameters.AddWithValue("@InstanceNextBookMarks", instanceNextBookMarks);

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

        private Activity LoadWorkflowActiovityFromXaml(string path)
        {
            const int fileBufferSize = 1024;
            const string ioExceptionPreamble = "Could not read program file due to an IO Exception.";
            FileStream fileStream = null;

            try
            {
                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, fileBufferSize, true);
                Activity program = null;

                Exception loadException = null;

                try
                {
                    ActivityXamlServicesSettings settings = new ActivityXamlServicesSettings
                    {
                        CompileExpressions = true
                    };
                    program = ActivityXamlServices.Load(fileStream, settings);
                }
                catch (XmlException xmlException)
                {
                    loadException = xmlException;
                }
                catch (XamlException xamlException)
                {
                    loadException = xamlException;
                }
                catch (ArgumentException argumentException)
                {
                    loadException = argumentException;
                }

                ValidationResults results = null;

                //If this is a Dynamic activity - a XamlException might occur
                try
                {
                    results = ActivityValidationServices.Validate(program);
                }
                catch (XamlException xamlException)
                {
                    loadException = xamlException;
                }

                if (loadException != null)
                {
                    this._errorLogger.Log("An error has occured loading the specified file: " + path,LoggerInfoTypes.Error);
                    return null;
                }


                foreach (ValidationError error in results.Errors)
                {
                    this._errorLogger.Log(string.Format("{0} Activity: {1}",

                        error.Message,
                        error.Source.DisplayName), LoggerInfoTypes.Error);
                }

                foreach (ValidationError warning in results.Warnings)
                {
                    this._errorLogger.Log(string.Format("{0} Activity: {1}",
                           warning.Message,
                        warning.Source.DisplayName), LoggerInfoTypes.Warning);
                }

                if (results.Errors.Count > 0)
                {
                    this._errorLogger.Log("Could not run Workflow: " + path, LoggerInfoTypes.Error);
                    this._errorLogger.Log("There are validation errors", LoggerInfoTypes.Error);
                    return null;
                }
                return program;

            }
            catch (FileNotFoundException fileNotFoundException)
            {
                this._errorLogger.Log("Could not read program file." + fileNotFoundException.Message, LoggerInfoTypes.Error);
                fileStream.Close();
                return null;
            }
            catch (IOException ioException)
            {
                this._errorLogger.Log(ioExceptionPreamble + ioException.Message, LoggerInfoTypes.Error);
                fileStream.Close();
                return null;
            }
        }
    }
}
