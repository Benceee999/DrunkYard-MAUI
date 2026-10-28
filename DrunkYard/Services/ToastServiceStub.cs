namespace DrunkYard.Services
{
    public class ToastServiceStub : IToastService
    {
        public void Show(string message)
        {
            System.Diagnostics.Debug.WriteLine($"TOAST ÜZENET: {message}");
        }
    }
}