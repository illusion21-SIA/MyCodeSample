using ReactiveUI;
using System.Drawing;

namespace MyCodeSample.ViewModels
{
    public class ParamGeneral : ReactiveObject
    {
        public ParamGeneral(STMainViewModel model, SDFSetting sdfSett)
        {

            if (model != null)
            {
                if (sdfSett.Data != null)
                {
                    if (sdfSett.Data is TypEnum enumeration)
                    {
                        Data = enumeration;
                        Setting = new SDFSettingViewModelEnum(sdfSett, model);
                    }
                    else
                    {
                        Data = sdfSett.Data;

                        Setting = new SDFSettingViewModel(sdfSett, model);

                        Setting.ModelSET.OnChanged += ModelSET_OnChanged;
                        GetParam += (s) => (model.SelectedNode as SectionViewModel)?.ParamGetSumHandler(s);
                        ParamChanged += (s) => (model.SelectedNode as SectionViewModel)?.ParamSetSpecHandler(s);
                    }
                }
                else Data = null;

            }
            else Setting = null;

            ParamName = SettSectionBase.section.Match(sdfSett.ID).Groups["name"].Value;
        }
        public SDFSettingViewModel Setting { get; private set; }

        public object Data { get; private set; }

        private string label;

        ///<summary>
        ////Имя, которое используется для отображения
        ///</summary>        
        public string ParamName { get; internal set; }


        public delegate void ParamEvent(object sender);
        ///<summary>
        /// Событие - пользователь изменил значение уставки
        ///</summary>
        public ParamEvent? ParamChanged;
        /// <summary>
        /// Событие - нужно пересчитать суммарное значение из удельного
        ///</summary>
        public ParamEvent? GetParam;
        public bool IsChangedValue { get => !(Setting?.ModelSDF.Data.IsChanged(specValue)) ?? true; }
        public bool IsError { get => !(Setting?.ModelSDF.Data.IsCorrectValue(specValue, out string desc)) ?? true; }

        internal object specValue;
        public object SpecValue
        {
            get
            {
                return (Setting?.ModelSET?.GetValue(Setting.ModelSDF.GetFullName()) is string str) ?
                str.Trim('\u0001', '\u0003') :
                Setting?.ModelSET?.GetValue(Setting.ModelSDF.GetFullName());
            }

            set
            {
                //(value is float ? (ParamsList.correctFloatformat.IsMatch(value.ToString()) && Convert.ToSingle(specValue) != Convert. ToSingle
                if (specValue?.ToString() != value.ToString())
                    specValue = value;
                Setting.ModelSET?.SetValue(Setting.ModelSDF.GetFullName(), value);
                this.RaisePropertyChanged(nameof(SpecValue));
                this.RaisePropertyChanged(nameof(IsChangedValue));
                this.RaisePropertyChanged(nameof(IsError));
            }
        }




        public Color IsChangedBackground
        { get => IsChangedValue ? RTColors.BrushChangedCell : Color.Transparent; }

        private void ModelSET_OnChanged(object sender, ChangesDataCollection e)
        {
            if ((e[0].Sender as SetElement).UndoRedo != UndoRedoEnum.None)
                this.RaisePropertyChanged(nameof(SpecValue));
            this.RaisePropertyChanged(nameof(IsChangedValue));
        }

        public void UpdateSumValue()
        {
            this.RaisePropertyChanged(nameof(SumValue));
            this.RaisePropertyChanged(nameof(IsSumChangedValue));
        }
        public void UpdateErrorAndChangeUI()
        {
            this.RaisePropertyChanged(nameof(IsError));
            this.RaisePropertyChanged(nameof(Is Changed Value));
        }

    }
}