namespace ModernVPN.MVVM.ViewModel
{
    internal class SettingsViewModel
    {
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        public SettingsViewModel()
        {
        }
    }
}