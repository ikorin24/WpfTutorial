using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tutorial1
{
    public class DataSerializer
    {
        /// <summary>ファイルにデータをセーブします</summary>
        /// <param name="filepath"></param>
        /// <param name="people"></param>
        public void Save(string filepath, IEnumerable<Person> people)
        {
            if(filepath == null) { throw new ArgumentNullException(nameof(filepath)); }
            if(people == null) { throw new ArgumentNullException(nameof(people)); }

            const string TMP_FILE = "____tmp____";
            if(File.Exists(filepath)) {
                File.Move(filepath, TMP_FILE);          // 元のファイルをバックアップ
            }
            try {
                using(var writer = new StreamWriter(filepath, false, Encoding.UTF8)) {
                    foreach(var person in people) {
                        writer.WriteLine(Person.Dump(person));
                    }
                }
            }
            catch(Exception ex) {
                File.Move(TMP_FILE, filepath);          // 失敗したらバックアップ復元
                throw ex;
            }
        }

        /// <summary>ファイルからデータをロードします</summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public IList<Person> Load(string filepath)
        {
            if(filepath == null) { throw new ArgumentNullException(nameof(filepath)); }

            try {
                var people = new List<Person>();
                using(var reader = new StreamReader(filepath, Encoding.UTF8)) {
                    people.Add(Person.Load(reader.ReadLine()));
                }
                return people;
            }
            catch(Exception ex) {
                throw ex;
            }
        }
    }
}
