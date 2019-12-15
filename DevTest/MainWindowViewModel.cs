using System;
using System.Collections.Generic;
using System.ComponentModel;
// RequireAttribute and MetadataTypeAttribute
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

namespace DevTest
{
    [POCOViewModel(ImplementINotifyPropertyChanging = false, ImplementIDataErrorInfo = true, InvokeOnPropertyChangedMethodBeforeRaisingINPC = true)]
    [MetadataType(typeof(MetaData))]
    public class MainWindowViewModel : ViewModelBase
    {
        public static MainWindowViewModel Create(string param)
        {
            return ViewModelSource.Create(() => new MainWindowViewModel(param));
        }

        protected MainWindowViewModel( string param)
        {
            Console.WriteLine("enter constructor, param {0}", param);
        }

        // if isBindable is false,  MVVM.POCO will not generate code for this propetry
        [BindableProperty(isBindable: true, OnPropertyChangingMethodName ="Update")]
        // define as virtual for auto implements by MVVM.POCO
        public virtual Brush TextColor { get; set; } = Brushes.Yellow;

        [StringLength(10, MinimumLength = 0, ErrorMessage = "Wrong Lenth of EnterText")]
        public virtual string EnterText { get; set; } = "default";

        [DependsOnProperties("EnterText")]
        public virtual string Message { get => IDataErrorInfoHelper.GetErrorText(this, "EnterText"); }

        private bool usingRed = false;

        [Command(isCommand:true, UseCommandManager = true)]
        // command func for ChangeColorCommand
        public void ChangeColor()
        {
            TextColor = usingRed ? Brushes.Red : Brushes.SkyBlue;
            usingRed = !usingRed;
        }

        public bool CanChangeColor()
        {
            // ImplementIDataErrorInfo is true, so this is IDataErrorInfo in runtime;
            return !IDataErrorInfoHelper.HasErrors(this as IDataErrorInfo, false, 2,
                (property) => property.Name == "EnterText");
        }

        // default func of TextColor changed callback
        public void OnTextColorChanged(Brush oldColor)
        {
            Console.WriteLine("OnTextColorChanged from {0} to {1}", oldColor, TextColor);
        }

        // explict func of TextColor changing callback
        public void Update(Brush newColor)
        {
            Console.WriteLine("Update from {0} to {1}", TextColor, newColor);
        }


        public class MetaData : IMetadataProvider<MainWindowViewModel>
        {
            void IMetadataProvider<MainWindowViewModel>.BuildMetadata(MetadataBuilder<MainWindowViewModel> builder)
            {
                builder.Property(x => x.EnterText).Required(() => "\nPlease enter text !!!");
            }
        }
    }


    public class TestViewModel
    {
        public virtual Brush Color { get; set; }

        public void OnColorChanged(Brush oldColor)
        {
            Console.WriteLine("--recv binding notification");
        }
    }
}
