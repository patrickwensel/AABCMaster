namespace AABC.Web.Models.Shared
{

    public enum DialogIcon {
        Information = 0,
        Critical = 1,
        Question =2,
        Ok = 3,
        Warning = 4,
        None = 5,
        Save = 6
    }

    public class DialogConfirmVM
    {

        /************

            NOTES

            Default Buttons not yet implemented

        ***********/

        public enum Choices
        {
            Button1,
            Button2
        }

        public string AnchorElementID { get; set; }         // the ID of the container div in the view calling this
        public string DialogElementID { get; set; }               // the ID that we want this to be called (used to invoke .Show() method from caller, e.g., Dialog.Show())
        public string ResultVariableName { get; set; }      // name of the variable to stuff the result into (must exist, in scope, in caller)
        public string DialogCloseScript { get; set; }       // function name or script (if any) to run as this closes

        public string Button1Caption { get; set; }
        public string Button2Caption { get; set; }

        public string Message { get; set; }
        public string Title { get; set; }

        public DialogIcon Icon { get; set; }
        public Choices Default { get; set; }
        public Choices Result { get; set; }
        
        public string ImageFilename
        {
            get
            {
                string filename = "";

                switch (this.Icon) {
                    case DialogIcon.Critical:
                        filename = "ico-delete-48.png";
                        break;
                    case DialogIcon.Information:
                        filename = "ico-info-48.png";
                        break;
                    case DialogIcon.Ok:
                        filename = "ico-ok-48.png";
                        break;
                    case DialogIcon.Question:
                        filename = "ico-question-48.png";
                        break;
                    case DialogIcon.Warning:
                        filename = "ico-warning-48.png";
                        break;
                    default:
                        filename = "";
                        break;
                }

                return filename;
            }
        }

        public DialogConfirmVM(string message) {
            this.Message = message;
            init();
        }
        
        void init() {
            this.DialogElementID = "DialogConfirm";
            this.AnchorElementID = "DialogConfirmAnchor";
            this.ResultVariableName = "DialogConfirmResult";
            this.DialogCloseScript = "DialogConfirmClosed";

            this.Title = "Confirm";
            this.Button1Caption = "Ok";
            this.Button2Caption = "Cancel";
            this.Default = Choices.Button1;
            this.Icon = DialogIcon.Information;
            
        }

    }
}