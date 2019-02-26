using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutorial1.UtilWpf;
using System.Collections.ObjectModel;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media;
using System.ComponentModel;

namespace Tutorial1
{
    public class MainWindowVM : NotifyPropertyChangedBase
    {
        private const string DIRTY_MARK = "*";
        /// <summary>セーブファイルのファイルパス(nullなら未保存)</summary>
        private string _filepath;

        #region Title
        /// <summary>Windowのタイトル</summary>
        public string Title
        {
            get { return _title; }
            set
            {
                if(_title == value) { return; }
                _title = value;
                RaisePropertyChanged();
            }
        }
        private string _title = "Unsaved File";
        #endregion

        #region People
        /// <summary>全員のデータ</summary>
        public ObservableCollection<Person> People { get; } = new ObservableCollection<Person>();
        #endregion

        #region SelectedPerson
        /// <summary>選択中の人</summary>
        public Person SelectedPerson
        {
            get { return _selectedPerson; }
            set
            {
                if(_selectedPerson == value) { return; }
                _selectedPerson = value;
                RaisePropertyChanged();
                RemoveSelectedPersonCommand.RaiseCanExecuteChanged();   // 削除コマンドが使用可能かを更新
            }
        }
        private Person _selectedPerson;
        #endregion

        #region IsDirty
        /// <summary>セーブファイルのDirtyフラグ</summary>
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if(_isDirty == value) { return; }
                _isDirty = value;
                RaisePropertyChanged();
                // trueになったらタイトルにマークをつけて、falseなら消す
                if(value) {
                    Title += DIRTY_MARK;
                }
                else {
                    if(Title.Substring(Title.Length - 1, 1) == DIRTY_MARK) {
                        Title = Title.Substring(0, Title.Length - 1);
                    }
                }
            }
        }
        private bool _isDirty;
        #endregion

        #region SaveCommand
        /// <summary>データのセーブコマンド</summary>
        public Command SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new Command(Save)); }
        }
        private Command _saveCommand;
        #endregion

        #region SaveAsCommand
        /// <summary>データの名前を付けてセーブコマンド</summary>
        public Command SaveAsCommand
        {
            get { return _saveAsCommand ?? (_saveAsCommand = new Command(SaveAs)); }
        }
        private Command _saveAsCommand;
        #endregion

        #region LoadCommand
        /// <summary>データの読み込みコマンド</summary>
        public Command LoadCommand
        {
            get { return _loadCommand ?? (_loadCommand = new Command(Load)); }
        }
        private Command _loadCommand;
        #endregion

        #region AddNewPersonCommand
        /// <summary>新規追加コマンド</summary>
        public Command AddNewPersonCommand
        {
            get { return _addNewPersonCommand ?? (_addNewPersonCommand = new Command(AddNewPerson)); }
        }
        private Command _addNewPersonCommand;
        #endregion

        #region RemoveSelectedPersonCommand
        /// <summary>削除コマンド</summary>
        public Command RemoveSelectedPersonCommand
        {
            get { return _removeSelectedPersonCommand ?? (_removeSelectedPersonCommand = new Command(RemoveSelectedPerson, () => SelectedPerson != null)); }
        }
        private Command _removeSelectedPersonCommand;
        #endregion

        public MainWindowVM()
        {
            People.CollectionChanged += (sender, e) => IsDirty = true;

            // -------------------- for Debug sample ---------------------------
#if DEBUG
            People.Add(new Person() { Name = "hoge", Age = 32, Color = Colors.Red });
            People.Add(new Person() { Name = "piyo", Age = 3, Color = Colors.Blue });
            People.Add(new Person() { Name = "meu", Age = 55, Color = Colors.Firebrick });
            People.Add(new Person() { Name = "nyan", Age = 4, Color = Colors.Yellow });
            foreach(var person in People) {
                person.PropertyChanged += (sender, e) => IsDirty = true;
            }
            SelectedPerson = People.FirstOrDefault();
#endif
            // -----------------------------------------------------------------
        }

        #region public Method

        /// <summary>終了時処理、未保存なら保存</summary>
        /// <returns>終了するか(false：終了キャンセル)</returns>
        public bool Close()
        {
            if(IsDirty == false) { return true; }
            var result = MessageBox.Show("変更を保存しますか？", "確認", MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk);
            if(result == MessageBoxResult.Cancel) { return false; }
            if(result == MessageBoxResult.No) { return true; }
            Save();
            return true;
        }

        #endregion

        #region private Method

        /// <summary>削除</summary>
        private void RemoveSelectedPerson()
        {
            var result = MessageBox.Show("削除します。よろしいですか？", "確認", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
            if(result != MessageBoxResult.OK) { return; }
            var index = People.IndexOf(SelectedPerson);
            var next = (index == People.Count - 1) ? index - 1 : index;
            People.RemoveAt(index);
            if(next >= 0) {
                SelectedPerson = People[next];
            }
            else {
                SelectedPerson = null;
            }
        }

        /// <summary>新規追加</summary>
        private void AddNewPerson()
        {
            var newPerson = new Person(){ Name = "New Person", Age = 10, Color = Colors.White, };
            newPerson.PropertyChanged += (sender, e) => IsDirty = true;
            People.Add(newPerson);
            SelectedPerson = newPerson;
        }

        /// <summary>ファイルに全員のデータを保存します</summary>
        private void Save()
        {
            if(_filepath == null) {
                SaveAs();
            }
            else {
                var serializer = new DataSerializer();
                try {
                    serializer.Save(_filepath, People);
                }
                catch(Exception) {
                    MessageBox.Show("ファイルの保存に失敗しました", "確認", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                IsDirty = false;
            }
        }

        /// <summary>名前を付けてファイルに全員のデータを保存します</summary>
        private void SaveAs()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "WPFチュートリアル用名簿データ|*.txt";
            dialog.ShowDialog();
            if(string.IsNullOrEmpty(dialog.FileName)) { return; }   // キャンセルなら抜ける
            var serializer = new DataSerializer();
            try {
                serializer.Save(dialog.FileName, People);
            }
            catch(Exception) {
                MessageBox.Show("ファイルの保存に失敗しました", "確認", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            _filepath = dialog.FileName;
            IsDirty = false;
        }

        /// <summary>ファイルから人物データを読み込みます</summary>
        private void Load()
        {
            if(IsDirty) {
                var result = MessageBox.Show("変更を保存しますか？", "確認", MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk);
                if(result == MessageBoxResult.Cancel) { return; }
                if(result == MessageBoxResult.Yes) {
                    Save();
                }
            }

            var dialog = new OpenFileDialog();
            dialog.Filter = "WPFチュートリアル用名簿データ|*.txt";
            dialog.ShowDialog();
            var filepath = dialog.FileName;
            if(string.IsNullOrEmpty(filepath)) { return; }   // キャンセルなら抜ける
            var serializer = new DataSerializer();
            IList<Person> loadedData;
            try {
                loadedData = serializer.Load(filepath);
            }
            catch(Exception) {
                MessageBox.Show("ファイルの読み込みに失敗しました", "確認", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            People.Clear();
            foreach(var data in loadedData) {
                People.Add(data);
            }
            SelectedPerson = People.FirstOrDefault();
            Title = Path.GetFileName(filepath);
            _filepath = filepath;
            IsDirty = false;
        }
        #endregion
    }
}
