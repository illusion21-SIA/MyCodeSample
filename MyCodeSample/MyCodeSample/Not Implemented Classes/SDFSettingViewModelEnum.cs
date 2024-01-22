namespace MyCodeSample.ViewModels
{
    internal class SDFSettingViewModelEnum : SDFSettingViewModel
    {
        private SDFSetting sdfSett;
        private STMainViewModel model;

        public SDFSettingViewModelEnum(SDFSetting sdfSett, STMainViewModel model)
        {
            this.sdfSett = sdfSett;
            this.model = model;
        }
    }
}