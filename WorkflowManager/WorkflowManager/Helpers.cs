using System;
using System.Activities;
using System.Activities.Hosting;
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
        public static string ConvertBookmakListToCommaSeparatedString(this ReadOnlyCollection<BookmarkInfo> bookmarks)
        {
            var coll = new CommaDelimitedStringCollection();
            //var actioncoll = new CommaDelimitedStringCollection();
            foreach(var b in bookmarks)
            {
                //actioncoll.Clear();
                //foreach (string s in b.BookMarkActions)
                //{
                //    actioncoll.Add(s);
                //}
                //var output = b.Name + (actioncoll.Count > 0 ? "-" + actioncoll.ToString() : "");
                coll.Add(b.BookmarkName);
            }
            return coll.ToString();
        }

        public static string ConvertStringListToCommaSeparatedString(this List<string> list)
        {
            var coll = new CommaDelimitedStringCollection();
            
            foreach (var s in list)
            {
               
                coll.Add(s);
            }
            return coll.ToString();
        }
    }
}
