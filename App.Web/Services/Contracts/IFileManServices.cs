using App.Utilities.Model;
using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Services.Contracts {
    public interface IFileManServices : IServiceBase{
        FileModel SaveFile(FileModel file);
        Task<FileModel> SaveFileAsync(FileModel file);
        FileModel LoadFile(string filename);
        Task<FileModel> LoadFileAsync(string filename);
        Task<FileModel> DeleteFileAsync(string filename);
        string GetDocStoragePath();
        string GetDocTemplatePath();
        void WriteText(string content, string path);
        string ReadTemplate(string filename);
        FileModel LoadTemplate(string filename);
    }
}
