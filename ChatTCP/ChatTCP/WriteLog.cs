using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server
{
    class WriteLog
    {
        public WriteLog()
        {
            
            Write("___________________________________________");
        }
        public void Write(string text)
        {
            string writePath = @"log.txt";

            
            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, true, Encoding.Default))
                {
                    sw.WriteLine($"{DateTime.Now} {text}");
                }

               
            }
            catch (Exception e)
            {
               
            }
        }
    }
}
