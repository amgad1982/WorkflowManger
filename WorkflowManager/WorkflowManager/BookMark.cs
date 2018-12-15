using System.Collections.Generic;

namespace WorkflowManager
{
    public class BookMarkWithActions:System.Activities.Bookmark
    {
        public BookMarkWithActions(string bookMarkName, List<string> bookMarkActions):base(bookMarkName)
        {
            BookMarkActions = bookMarkActions;
        }
        public List<string> BookMarkActions { get; set; }
    }
}
