using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClarityServices.System {
    public class Logger {

        public string path;
        public Logger(string path) {
            this.path = path;
        }


        public void write(string msg) { //Async!
            new Thread(new ThreadStart(() => {
                try {
                    if (File.Exists(path)) File.SetAttributes(path, FileAttributes.Normal);
                    StreamWriter sw = new StreamWriter(path, true);
                    sw.WriteLine(Environment.NewLine + DateTime.Now.ToString() + msg);
                } catch (Exception) {
                    //archivo tomado
                }
            })).Start();
        }
    }
}
