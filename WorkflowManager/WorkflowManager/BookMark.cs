using System.Collections.Generic;

namespace WorkflowManager
{
    public class BookMark:System.Activities.Bookmark
    {
        public BookMark(string bookMarkName, List<string> bookMarkActions):base(bookMarkName)
        {
            BookMarkName = bookMarkName;
            BookMarkActions = bookMarkActions;
        }

        public string BookMarkName { get; set; }
        public List<string> BookMarkActions { get; set; }
    }
}
