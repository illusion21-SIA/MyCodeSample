
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace MyCodeSample.ViewModels
{
    /// <summary>
    /// Класс представляющий определенный участок ЛЭП
    /// </summary>
    public class SectionViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private string caption;
        private SDFSettingViewModelEnum secType = null;

        public SourceList<ParamGeneral> _gParams = new();
        //Коллекция параметров прямой последовательноти
        public ReadOnlyObservableCollection<ParamGeneral> DirectList { get => listDirect; }
        private ReadOnlyObservableCollection<ParamGeneral> listDirect;
        //Коллекция параметров нулевой последовательноти
        public ReadOnlyObservableCollection<ParamGeneral> NulltList { get => listNull; }
        private ReadOnlyObservableCollection<ParamGeneral> listNull;
        private ParamGeneral name;
        private ParamGeneral lenght;

        public string Number { get; }
        public ParamGeneral Lenght { get => lenght; set => lenght = value; }
        public ParamGeneral Name { get => name; set => name = value; }

        public ReactiveCommand<Unit, Task> InsertSection { get; }
        public ReactiveCommand<Unit, Unit> DeleteSection { get; }
        public SectionViewModel(TreeNodeGroup parent, IGrouping<string, ParamGeneral> result)
        {
            foreach (var sett in result)
            {
                //приходит список всех уставок линии, отсортированных по имени
                if (sett.ParamName.Equals("Name", StringComparison.Ordinal))
                {
                    name = sett;
                    Number = string.Format("(0:00)", result.Key);
                    //теперь в зависимости от имени опреедляем какие относятся именно к этому участку
                    // Settings = parent.Settings?.Where(x => SettSectionBase.section.Match(x.ID).Groups["number"].Value == Number).ToList();
                }
                else if (sett.ParamName.Equals("Type", StringComparison.Ordinal))
                    secType = sett.Setting as SDFSettingViewModelEnum;
                else if (sett.ParamName.Equals("Length", StringComparison.Ordinal))
                    lenght = sett;
                else _gParams.Add(sett);

                //подписка на изменения коллекций, чтобы изменение любого параметра отображалось в UI
                var cancel = _gParams.Connect().
                Filter(par => ParamsList.DirectNames.Contains(par.ParamName))
                .Bind(out listDirect)
                .DisposeMany()
                .Subscribe();

                var cancel2 = _gParams.Connect().
                Filter(par => ParamsList.NullNames.Contains(par.ParamName))
                .Bind(out listNull)
                .DisposeMany()
                .Subscribe();

                //команды вставки/удаления еще одного участка, привязка к ним в xaml
                InsertSection = ReactiveCommand.Create(async () => await LineSectionModel.InsertSectionOMP(this));
                DeleteSection = ReactiveCommand.Create(() => LineSectionModel.DeleteSectionOMP(this));
                //Line = ((parent is TreeNodeGroupLOCLine parentLine) ? parentLine : (parent as TreeNodeGrouplineParams).Line);
            }
        }
    }
}

