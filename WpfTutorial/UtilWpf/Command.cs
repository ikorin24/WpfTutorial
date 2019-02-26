using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tutorial1.UtilWpf
{
    public class Command : ICommand
    {
        private Action _action;
        private Func<bool> _canexe;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canexe();

        public void Execute(object parameter) => _action();

        /// <summary>特定の処理を実行するコマンドを生成します</summary>
        /// <param name="action">実行する処理</param>
        public Command(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _canexe = () => true;
        }

        /// <summary>特定の処理を実行するコマンドを生成します</summary>
        /// <param name="action">実行する処理</param>
        /// <param name="canexe">実行可能かを判断する式</param>
        public Command(Action action, Func<bool> canexe)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _canexe = canexe ?? throw new ArgumentNullException(nameof(canexe));
        }

        /// <summary>コマンドが実行可能かどうかを再評価します</summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}