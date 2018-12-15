using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowManager.BookMarkActivities
{
    public sealed class GenericBooleanBookMark : NativeActivity<bool>
    {
        public GenericBooleanBookMark()
        {
        }

        [RequiredArgument]
        public InArgument<string> BookmarkName { get; set; }

        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }

        protected override void Execute(NativeActivityContext context)
        {
            string name = this.BookmarkName.Get(context);

            context.CreateBookmark(name, new BookmarkCallback(OnResumeBookmark));
        }

        public void OnResumeBookmark(NativeActivityContext context, Bookmark bookmark, object obj)
        {
            // When the Bookmark is resumed, assign its value to  
            // the Result argument.  
            Result.Set(context, (bool)obj);
        }
    }
}
