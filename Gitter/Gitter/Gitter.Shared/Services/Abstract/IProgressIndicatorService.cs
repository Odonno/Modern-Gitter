using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace Gitter.Services.Abstract
{
    public interface IProgressIndicatorService
    {
        StatusBarProgressIndicator ProgressIndicator { get; set; }
        bool ShowProgressIndicator { get; }


        Task ShowAsync();
        Task HideAsync();
    }
}
