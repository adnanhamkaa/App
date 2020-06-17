using App.Utilities.Model;
using App.Web.Models;
using App.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace App.Web.Services.Repositories {
    public class FileManServices : ServiceBase, IFileManServices {

        public FileModel SaveFile(FileModel file) {

            var dest = ConfigurationManager.AppSettings["docstorage"].ToString();

            if (dest.StartsWith("~")) {
                try {
                    dest = HttpContext.Current.Server.MapPath(dest);
                } catch (Exception exc) {
                    try {
                        dest = System.Web.Hosting.HostingEnvironment.MapPath(dest);
                    } catch (Exception excx) {

                    }
                }
            }

            var path = Path.Combine(dest, file.FileName);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            if (File.Exists(path)) {
                File.Delete(path);
            }

            File.WriteAllBytes(path, file.Bytes);

            return new FileModel() {
                FileName = Path.GetFileName(file.FileName),
                Mime = MimeMapping.GetMimeMapping(file.FileName)
            };
        }

        public void WriteText(string content, string path) {

            var dest = ConfigurationManager.AppSettings["docstorage"].ToString();

            if (dest.StartsWith("~")) {
                try {
                    dest = HttpContext.Current.Server.MapPath(dest);
                } catch (Exception exc) {
                    try {
                        dest = System.Web.Hosting.HostingEnvironment.MapPath(dest);
                    } catch (Exception excx) {
                        
                    }
                }
            }

            if (!dest.EndsWith("\\")) dest += "\\";

            var filepath = Path.Combine(dest, path);

            Directory.CreateDirectory(Path.GetDirectoryName(filepath));

            if (File.Exists(filepath)) {
                File.Delete(filepath);
            }

            File.WriteAllText(filepath, content);

        }

        public FileModel SaveFile(FileModel file, string daestPath) {

            var dest = daestPath;

            if (dest.StartsWith("~")) {
                try {
                    dest = HttpContext.Current.Server.MapPath(dest);
                } catch (Exception exc) {
                    try {
                        dest = System.Web.Hosting.HostingEnvironment.MapPath(dest);
                    } catch (Exception excx) {

                    }
                }
            }

            var path = Path.Combine(dest, file.FileName);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            if (File.Exists(path)) {
                File.Delete(path);
            }

            File.WriteAllBytes(path, file.Bytes);

            return new FileModel() {
                FileName = Path.GetFileName(file.FileName),
                Mime = MimeMapping.GetMimeMapping(file.FileName)
            };
        }

        public async Task<FileModel> SaveFileAsync(FileModel file) {

            var dest = ConfigurationManager.AppSettings["docstorage"].ToString();

            if (dest.StartsWith("~")) {
                try {
                    dest = HttpContext.Current.Server.MapPath(dest);
                } catch (Exception exc) {
                    try {
                        dest = System.Web.Hosting.HostingEnvironment.MapPath(dest);
                    } catch (Exception excx) {

                    }
                }
            }

            var path = Path.Combine(dest, file.FileName);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            if (File.Exists(path)) {
                File.Delete(path);
            }

            File.WriteAllBytes(path, file.Bytes);

            return new FileModel() {
                FileName = Path.GetFileName(file.FileName),
                Mime = MimeMapping.GetMimeMapping(file.FileName)
            };
        }

        public FileModel LoadFile(string filename) {
            byte[] data = null;

            var dest = ConfigurationManager.AppSettings["docstorage"].ToString();

            if (dest.StartsWith("~")) {
                try {
                    dest = HttpContext.Current.Server.MapPath(dest);
                } catch (Exception exc) {
                    try {
                        dest = System.Web.Hosting.HostingEnvironment.MapPath(dest);
                    } catch (Exception excx) {

                    }
                }
            }

            var path = Path.Combine(dest, filename);

            if (File.Exists(path)) {

                return new FileModel() {
                    Bytes = File.ReadAllBytes(path),
                    FileName = Path.GetFileName(path),
                    Mime = MimeMapping.GetMimeMapping(path)
                };


            } else {
                return null;
            }
        }

        public async Task<FileModel> LoadFileAsync(string filename) {
            byte[] data = null;

            var dest = ConfigurationManager.AppSettings["docstorage"].ToString();

            if (dest.StartsWith("~")) {
                try {
                    dest = HttpContext.Current.Server.MapPath(dest);
                } catch (Exception exc) {
                    try {
                        dest = System.Web.Hosting.HostingEnvironment.MapPath(dest);
                    } catch (Exception excx) {

                    }
                }
            }

            var path = Path.Combine(dest, filename);

            if (File.Exists(path)) {

                return new FileModel() {
                    Bytes = File.ReadAllBytes(path),
                    FileName = Path.GetFileName(path),
                    Mime = MimeMapping.GetMimeMapping(path)
                };


            } else {
                return null;
            }
        }

        public async Task<FileModel> DeleteFileAsync(string filename) {

            var dest = ConfigurationManager.AppSettings["docstorage"].ToString();

            if (dest.StartsWith("~")) {
                dest = HttpContext.Current.Server.MapPath(dest);
            }

            var path = Path.Combine(dest, filename);

            if (File.Exists(path)) {

                File.Delete(path);

                return new FileModel() {
                    FileName = Path.GetFileName(path),
                    Mime = MimeMapping.GetMimeMapping(path)
                };


            } else {
                return null;
            }
        }

        public string GetDocStoragePath() {
            var dest = ConfigurationManager.AppSettings["docstorage"].ToString();

            if (dest.StartsWith("~")) {
                try {
                    dest = HttpContext.Current.Server.MapPath(dest);
                } catch (Exception exc) {
                    try {
                        dest = System.Web.Hosting.HostingEnvironment.MapPath(dest);
                    } catch (Exception excx) {

                    }
                }
            }


            return dest;
        }


        public string GetDocTemplatePath() {
            var dest = ConfigurationManager.AppSettings["doctemplate"].ToString();

            if (dest.StartsWith("~")) {
                try {
                    dest = HttpContext.Current.Server.MapPath(dest);
                } catch (Exception exc) {
                    try {
                        dest = System.Web.Hosting.HostingEnvironment.MapPath(dest);
                    } catch (Exception excx) {

                    }
                }
            }


            return dest;
        }
        
        public string ReadTemplate(string filename) {
            
            var dest = ConfigurationManager.AppSettings["doctemplate"].ToString();

            if (dest.StartsWith("~")) {
                try {
                    dest = HttpContext.Current.Server.MapPath(dest);
                } catch (Exception exc) {
                    try {
                        dest = System.Web.Hosting.HostingEnvironment.MapPath(dest);
                    } catch (Exception excx) {
                        ErrorLog(excx);
                    }
                }
            }

            var path = Path.Combine(dest, filename);

            if (File.Exists(path)) {

                return File.ReadAllText(path);
                
            } else {
                return null;
            }
        }
        
        public FileModel LoadTemplate(string filename) {

            var dest = ConfigurationManager.AppSettings["doctemplate"].ToString();

            if (dest.StartsWith("~")) {
                try {
                    dest = HttpContext.Current.Server.MapPath(dest);
                } catch (Exception exc) {
                    try {
                        dest = System.Web.Hosting.HostingEnvironment.MapPath(dest);
                    } catch (Exception excx) {
                        ErrorLog(excx);
                    }
                }
            }

            var path = Path.Combine(dest, filename);

            if (File.Exists(path)) {

                return new FileModel() {
                    FileName = Path.GetFileName(filename),
                    Mime = MimeMapping.GetMimeMapping(Path.GetFileName(filename)),
                    Bytes = File.ReadAllBytes(path)
                };

            } else {
                return null;
            }
        }

    }
}