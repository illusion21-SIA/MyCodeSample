namespace MyCodeSample.ViewModels
{
    public class SDFSettingViewModel
    {
        public SDFSettingViewModel(SDFSetting sdfSett, STMainViewModel model)
        {
            SdfSett = sdfSett;
            Model = model;
        }

        public SDFSetting SdfSett { get; }
        public STMainViewModel Model { get; }
    }
}