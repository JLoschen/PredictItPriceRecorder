using System;
using GalaSoft.MvvmLight;

namespace PluginGUI.ViewModels
{
    public class ParameterViewModel : ViewModelBase
    {
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }
        private string _name;

        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }
        private string _description;

        public object Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }
        private object _value;

        public object DefaultValue
        {
            get { return _defaultValue; }
            set { Set(ref _defaultValue, value); }
        }
        private object _defaultValue;

        public Type DataType
        {
            get { return _dataType; }
            set { Set(ref _dataType, value); }
        }
        private Type _dataType;
    }
}