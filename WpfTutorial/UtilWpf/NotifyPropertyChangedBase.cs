using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial1.UtilWpf
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        /// <summary>プロパティ変化時イベント(INotifyPropertyChanged実装)</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>プロパティ変更通知イベントを発火します</summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
