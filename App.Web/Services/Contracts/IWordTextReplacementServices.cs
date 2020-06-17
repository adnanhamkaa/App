using App.Utilities.Model;
using App.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Services.Contracts {
    public interface IWordTextReplacementServices {
        
        byte[] GetWordReplacedText(string templatePath, List<WordReplacement> items);
        FileModel SaveDoc(string filename, byte[] data);
        bool DeleteDir(string dirname);
        bool DeleteDoc(string filename);
        FileModel GetDoc(string filename);
        bool IsExists(string filename);
        byte[] GetDocumentByte(string templatePath);
        byte[] GetWordReplacedTextUsingPlaintext(string templatePath, List<WordReplacement> items);
        FileModel WordToPdf(byte[] data, string fileName = null);
    }
}
