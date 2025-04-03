using RandomMovie.Enums;

namespace RandomMovie.ViewModels
{
    public class InfoMessagePopUpViewModel
    {
        public string Message { get; set; }
        public bool OkVisible { get; set; } = false;
        public bool YesVisible { get; set; } = false;
        public bool NoVisible { get; set; } = false;
        public bool CloseVisible { get; set; } = false;

        public InfoMessagePopUpViewModel(string message, InfoMessageAnswer infoMessageAnswer) 
        {
            Message = message;
            switch (infoMessageAnswer)
            {
                case InfoMessageAnswer.Ok:
                    OkVisible = true;
                    break;
                case InfoMessageAnswer.YesNo:
                    YesVisible = true;
                    NoVisible = true;
                    break;
                case InfoMessageAnswer.Close:
                    CloseVisible = true;
                    break;
            }
        }
    }
}
