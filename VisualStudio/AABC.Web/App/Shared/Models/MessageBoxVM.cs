namespace AABC.Web.Models.Shared
{
    public class MessageBoxVM
    {


        public string Message { get; set; }
        public string Title { get; set; }

        public DialogIcon Icon { get; set; }

        public MessageBoxVM() {
            this.Icon = DialogIcon.Information;
        }


        public string ImageFilename {
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
                    case DialogIcon.Save:
                        filename = "ico-save-48.png";
                        break;
                    default:
                        filename = "ico-info-48.png";
                        break;
                }

                return filename;
            }
        }


    }
}