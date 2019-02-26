using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Tutorial1.UtilWpf;

namespace Tutorial1
{
    public class Person : NotifyPropertyChangedBase
    {
        #region Name
        /// <summary>名前</summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if(_name == value) { return; }
                _name = value;
                RaisePropertyChanged();
            }
        }
        private string _name;
        #endregion

        #region Age
        /// <summary>年齢</summary>
        public int Age
        {
            get { return _age; }
            set
            {
                if(_age == value){ return; }
                _age = value;
                RaisePropertyChanged();
            }
        }
        private int _age;
        #endregion

        #region Color
        /// <summary>色</summary>
        public Color Color
        {
            get { return _color; }
            set
            {
                if(_color == value) { return; }
                _color = value;
                RaisePropertyChanged();
            }
        }
        private Color _color;
        #endregion

        /// <summary>インスタンスを文字列化します</summary>
        /// <param name="person">対象のインスタンス</param>
        /// <returns>データ文字列</returns>
        public static string Dump(Person person)
        {
            // 各プロパティを文字列化
            return $"{person.Name}\t{person.Age}\t{person.Color.ToString()}";
        }

        /// <summary>文字列化されたデータからインスタンスを生成します</summary>
        /// <param name="str">データ文字列</param>
        /// <returns>生成したインスタンス</returns>
        public static Person Load(string str)
        {
            if(str == null) { throw new ArgumentNullException(nameof(str)); }
            // Dumpで出力された文字列を解析してインスタンス生成
            try {
                var split = str.Split('\t');
                var person = new Person();
                person._name = split[0];
                person._age = int.Parse(split[1]);
                person._color = (Color)ColorConverter.ConvertFromString(split[2]);
                return person;
            }
            catch(Exception ex) {
                throw new FormatException("不正な形式のデータです", ex);
            }
        }
    }
}
