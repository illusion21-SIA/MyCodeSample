
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
        private ParamGeneral name;
        private ParamGeneral lenght;
        private SDFSettingViewModelEnum secType = null;

        //источник уставок - общий список, а подписчиками 3 коллекции(Direct, Null, Indiv)
        //это нужно, чтобы избежать повторения кода в UI, для всех них можно будет использовать один DataTemplate и 3 ItemsControl
        public SourceList<ParamGeneral> _gParams = new();
        //Коллекция параметров прямой последовательноти
        public ReadOnlyObservableCollection<ParamGeneral> DirectList { get => listDirect; }
        private ReadOnlyObservableCollection<ParamGeneral> listDirect;
        //Коллекция параметров нулевой последовательноти
        public ReadOnlyObservableCollection<ParamGeneral> NulltList { get => listNull; }
        private ReadOnlyObservableCollection<ParamGeneral> listNull;
         
        //команды добавления/удаления участка
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

                //подписка на изменения коллекций, чтобы изменение любого параметра отображалось в UI, и чтобы записывалось в источник
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
        
        public string Number { get; }
        public ParamGeneral Lenght { get => lenght; set => lenght = value; }
        //Имя типа object, т.к отображаться должно как строка, а писаться в ParamGeneral
        public object Name
        {
            get => (name.SpecValue as string);
            set
            {
                value = LineSectionModel.CheckNameLength(name, (value as string).TrimStart());
                name.SpecValue = (value as string);
                this.RaisePropertyChanged(nameof(Name));
                UpdateCaption();
            }
        }


        /// <summary>
        /// Выбранная вкладка(Удельные параметры=0, суммарные=1)
        /// </summary>
        public int? SelectedTabInd
        {
            // свойство берется из кэша, т.к оно должно быть одинаковым для всех секций
            get => (GeneralProperties.Instance.GetProperty(nameof(SectionViewModel), GeneralPropertiesDefines.SelectedParamTabInd) as PropertyInt).Value;
            set
            {
                if ((selectedTabInd == 0 || selectedTabInd == null) && value == 1)
                    Task.Run(() => RecalcSumParams());

                SwitchTabLabels((int)value);
                selectedTabInd = value;
                SetIntProperty(GroupGeneralPropertiesDefines.SelectedParamTabInd, (int)value);//свойство сохраняется и в файл, и в кэш
                this.RaisePropertyChanged(nameof(SelectedTabInd));

            }
        }
        private static void SetIntProperty(string key, int val)
        {
            PropertyInt propSelectedTabInd = new PropertyInt(GeneralPropertiesDefines.SelectedParamTabInd) { IsSaved = true, Value = val };

            GeneralProperties.Instance.WriteProperties(new() { propSelectedTabInd }, FlagUnitedProerties.ReplaceOrAdd);
            GeneralProperties.Instance.AddToCache(nameof(SectionViewModel), new() { propSelectedTabInd });
        }

    }
}

