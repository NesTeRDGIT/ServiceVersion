using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ServiceLoaderMedpomData.Annotations;

namespace ClientServiceWPF.Class
{
    public class ProgressItem : INotifyPropertyChanged
    {
        private bool _IsIndeterminate;
        public bool IsIndeterminate
        {
            get => _IsIndeterminate;
            set
            {
                _IsIndeterminate = value;
                RaisePropertyChanged();
            }
        }
        private int _Maximum = 1;
        public int Maximum
        {
            get => _Maximum;
            set
            {
                _Maximum = value;
                RaisePropertyChanged();
            }
        }
        private int _Value;
        public int Value
        {
            get => _Value;
            set
            {
                _Value = value;
                RaisePropertyChanged();
            }
        }
        private string _Text;
        public string Text
        {
            get => _Text;
            set
            {
                _Text = value;
                RaisePropertyChanged();
            }
        }

        public void SetValues(int MaximumP, int ValueP, string TextP)
        {
            this.Maximum = MaximumP;
            this.Value = ValueP;
            this.Text = TextP;
        }
        public void SetTextValue(int ValueP, string TextP)
        {
            this.Value = ValueP;
            this.Text = TextP;
        }

        public void Clear(string TextP)
        {
            this.Maximum = 1;
            this.Value = 0;
            this.Text = TextP;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
