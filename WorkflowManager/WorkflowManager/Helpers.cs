using System;
using System.Activities;
using System.Activities.Validation;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using System.Xml;

namespace WorkflowManager
{
    public static class Helpers
    {
        public static Activity LoadWorkflowActiovityFromXaml(string path,ILogger errorLogger)
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
                    errorLogger.Log("An error has occured loading the specified file: " + path, LoggerInfoTypes.Error);
                    return null;
                }


                foreach (ValidationError error in results.Errors)
                {
                    errorLogger.Log(string.Format("{0} Activity: {1}",

                        error.Message,
                        error.Source.DisplayName), LoggerInfoTypes.Error);
                }

                foreach (ValidationError warning in results.Warnings)
                {
                    errorLogger.Log(string.Format("{0} Activity: {1}",
                           warning.Message,
                        warning.Source.DisplayName), LoggerInfoTypes.Warning);
                }

                if (results.Errors.Count > 0)
                {
                    errorLogger.Log("Could not run Workflow: " + path, LoggerInfoTypes.Error);
                    errorLogger.Log("There are validation errors", LoggerInfoTypes.Error);
                    return null;
                }
                return program;

            }
            catch (FileNotFoundException fileNotFoundException)
            {
                errorLogger.Log("Could not read program file." + fileNotFoundException.Message, LoggerInfoTypes.Error);
                fileStream.Close();
                return null;
            }
            catch (IOException ioException)
            {
                errorLogger.Log(ioExceptionPreamble + ioException.Message, LoggerInfoTypes.Error);
                fileStream.Close();
                return null;
            }
        }
        public static string ConvertBookmakListToCommaSeparatedString(this ReadOnlyCollection<Bookmark> bookmarks)
        {
            var coll = new CommaDelimitedStringCollection();
            foreach(Bookmark b in bookmarks)
            {
                coll.Add(b.Name);
            }
            return coll.ToString();
        }
    }
}
