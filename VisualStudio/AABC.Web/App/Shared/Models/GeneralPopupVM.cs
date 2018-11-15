namespace AABC.Web.Models.Shared
{
    public class GeneralPopupVM
    {

        public int Width { get; set; }
        public int Height { get; set; }
        public string Title { get; set; }
        public bool AllowDrag { get; set; }

        public GeneralPopupVM() {
            Width = 350;
            Height = 350;
            Title = "";
            AllowDrag = false;
        }

    }


    public class PopupGeneralOptions
    {
        // lowercase as they must match incoming js object property names
        // would rather handle these lowercase than have js client submit uppercase
        public int? width { get; set; }
        public int? height { get; set; }
        public string title { get; set; }
        public bool allowDrag { get; set; }
    }

}