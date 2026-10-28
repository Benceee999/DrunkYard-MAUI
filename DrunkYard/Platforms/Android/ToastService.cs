using Android.Widget;
using DrunkYard.Services;
using DrunkYard.DataAccess.Models;
using DrunkYard.DataAccess.Services;

namespace DrunkYard.Platforms.Android
{
    public class ToastService : IToastService
    {
        public void Show(string message)
        {
            Toast.MakeText(Platform.CurrentActivity, message, ToastLength.Short)?.Show();
        }
    }
}