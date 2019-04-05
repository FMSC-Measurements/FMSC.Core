using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FMSC.Core.Utilities
{
    public static class FileUtils
    {
        public static bool IsFileLocked(string filename, FileAccess file_access = FileAccess.Read)
        {
            try
            {
                new FileStream(filename, FileMode.Open, file_access).Close();
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}
