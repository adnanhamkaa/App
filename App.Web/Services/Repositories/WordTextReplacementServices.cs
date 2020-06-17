
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using App.Web.Models;
using App.Web.Services.Contracts;
using App.Web.Utilities;
using OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using App.Utilities.Model;

namespace App.Web.Services.Repositories
{
    public class WordTextReplacementServices : IWordTextReplacementServices {

        IPdfServices _pdfSvc;

        public WordTextReplacementServices(IPdfServices pdfSvc) {
            _pdfSvc = pdfSvc;
        }

        public byte[] GetDocumentByte(string templatePath) {
            var pathdir = ConfigurationManager.AppSettings["doctemplate"].ToString();

            if (pathdir.StartsWith("~")) {
                pathdir = HttpContext.Current.Server.MapPath(pathdir);
            }

            var path = Path.Combine(pathdir, templatePath);

            FileStream fileStream = new FileStream(path, FileMode.Open);
            using (MemoryStream templateStream = new MemoryStream()) {
                //templateStream.Write(templateBytes, 0, (int)templateBytes.Length);
                fileStream.CopyStream(templateStream);
                fileStream.Close();
                return templateStream.ToArray();
            }
        }

        public byte[] GetWordReplacedTextUsingPlaintext(string templatePath, List<WordReplacement> items) {
            var pathdir = ConfigurationManager.AppSettings["doctemplate"].ToString();

            if (pathdir.StartsWith("~")) {
                pathdir = HttpContext.Current.Server.MapPath(pathdir);
            }

            var path = Path.Combine(pathdir, templatePath);

            FileStream fileStream = new FileStream(path, FileMode.Open);
            using (MemoryStream templateStream = new MemoryStream()) {
                //templateStream.Write(templateBytes, 0, (int)templateBytes.Length);
                fileStream.CopyStream(templateStream);
                fileStream.Close();
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateStream, true)) {
                    wordDoc.ChangeDocumentType(WordprocessingDocumentType.Document);
                    
                    SimplifyMarkupSettings settings = new SimplifyMarkupSettings {
                        RemoveProof = true,
                        RemoveRsidInfo = true,
                        NormalizeXml = true,
                        //RemoveContentControls = true,
                        //RemoveMarkupForDocumentComparison = true
                    };

                    MarkupSimplifier.SimplifyMarkup(wordDoc, settings);


                    string docText = null;
                    using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream())) {
                        docText = sr.ReadToEnd();
                    }

                    foreach (var item in items) {
                        if (!string.IsNullOrEmpty(item.TextToReplace)) {
                            Regex regexText = new Regex(item.TextToReplace);
                            docText = regexText.Replace(docText, item.ReplacementText??"");
                        }
                    }


                    using (StreamWriter sw = new StreamWriter(templateStream)) {
                        sw.Write(docText);


                        return templateStream.ToArray();
                    }
                }
            }
        }

        public byte[] GetWordReplacedText(string templatePath, List<WordReplacement> items) {
            var pathdir = ConfigurationManager.AppSettings["doctemplate"].ToString();

            if (pathdir.StartsWith("~")) {
                pathdir = HttpContext.Current.Server.MapPath(pathdir);
            }

            var path = Path.Combine(pathdir, templatePath);

            FileStream fileStream = new FileStream(path, FileMode.Open);
            using (MemoryStream templateStream = new MemoryStream()) {
                //templateStream.Write(templateBytes, 0, (int)templateBytes.Length);
                fileStream.CopyStream(templateStream);
                fileStream.Close();
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateStream, true)) {
                    wordDoc.ChangeDocumentType(WordprocessingDocumentType.Document);

                    SimplifyMarkupSettings settings = new SimplifyMarkupSettings {
                        RemoveComments = true,
                        RemoveContentControls = true,
                        RemoveEndAndFootNotes = true,
                        RemoveFieldCodes = false,
                        RemoveLastRenderedPageBreak = true,
                        RemovePermissions = true,
                        RemoveProof = true,
                        RemoveRsidInfo = true,
                        RemoveSmartTags = true,
                        RemoveSoftHyphens = true,
                        ReplaceTabsWithSpaces = true,
                        RemoveWebHidden = true,
                        RemoveMarkupForDocumentComparison = true
                    };

                    MarkupSimplifier.SimplifyMarkup(wordDoc, settings);

                    var body = wordDoc.MainDocumentPart.Document.Body;
                    var tables = body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>().ToList();
                    var paras = body.Elements<Paragraph>();
                    var runsall = body.Descendants<Run>().ToList();

                    foreach (var item in wordDoc.MainDocumentPart.HeaderParts) {
                        foreach (var run in item.RootElement.Descendants<Run>()) {
                            runsall.Add(run);
                        }
                    }

                    foreach (var item in wordDoc.MainDocumentPart.FooterParts) {
                        foreach (var run in item.RootElement.Descendants<Run>()) {
                            runsall.Add(run);
                        }
                    }

                    for (int i = 0; i < runsall.Count(); i++) {
                        var r = runsall[i];
                        var textsrun = r.Elements<Text>();

                        if ((items.Any(t => !t.MatchWholeText ? r.InnerText?.Trim().Contains(t.TextToReplace) == true : r.InnerText == t.TextToReplace))) {
                            var replace = items.Where(t => !t.MatchWholeText ? r.InnerText?.Trim().Contains(t.TextToReplace) == true : r.InnerText == t.TextToReplace).FirstOrDefault();

                            if (!replace.IsCheckBox) {
                                foreach (var text in textsrun) {
                                    if (text != null) {
                                        if (items.Any(t => !t.MatchWholeText ? text.InnerText?.Trim().Contains(t.TextToReplace) == true : text.InnerText == t.TextToReplace)) {

                                            var wrd = items.FirstOrDefault(it => !it.MatchWholeText ? text.InnerText?.Trim().Contains(it.TextToReplace) == true : text.InnerText == it.TextToReplace);

                                            //while (items.Any(it => !it.MatchWholeText ? text.InnerText?.Trim().Contains(it.TextToReplace) == true : text.InnerText == it.TextToReplace)) { 

                                            //}
                                            if (replace.UseRun) {
                                                try {
                                                    if(replace.Run != null) {
                                                        replace.Run.Append(r.RunProperties.CloneNode(true));
                                                        r.RemoveAllChildren();
                                                        r.Append(replace.Run.CloneNode(true));
                                                    } else {
                                                        r.RemoveAllChildren(); 
                                                    }
                                                } catch (Exception exc) {
                                                    
                                                }

                                            } else {
                                                wrd = items.FirstOrDefault(it => !it.MatchWholeText ? text.InnerText?.Trim().Contains(it.TextToReplace) == true : text.InnerText == it.TextToReplace);
                                                text.Text = wrd.MatchWholeText ? wrd.ReplacementText : text.Text.Replace(wrd.TextToReplace, wrd.ReplacementText);
                                            }
                                        }
                                    }
                                }
                            } else {
                                r.RemoveAllChildren();
                                replace.Checkboxes.ForEach(c => {
                                    r.Append(c.CloneNode(true));
                                });


                                //var text = textsrun.FirstOrDefault();

                                //if (text != null)
                                //{
                                //    text.Text = String.Empty;
                                //}

                                //r.Chil();//.RemoveChild(r);

                            }
                        }

                    }

                    MarkupSimplifier.SimplifyMarkup(wordDoc, settings);
                    wordDoc.Save();

                    return templateStream.ToArray();

                    //foreach (var para in paras) {
                    //    var fieldss = para.Elements<SimpleField>();
                    //    var runs = para.Elements<Run>().ToList();
                        
                    //    for (int i = 0; i < runs.Count; i++) {
                    //        var r = runs[i];
                    //        var texts = r.Elements<Text>();

                    //        if ((items.Any(t => r.InnerText?.Trim().Contains(t.TextToReplace) == true))) {
                    //            var replace = items.Where(t => r.InnerText?.Trim().Contains(t.TextToReplace) == true).FirstOrDefault();

                    //            if (!replace.IsCheckBox) {
                    //                foreach (var text in texts) {
                    //                    if (text != null) {
                    //                        if (items.Any(t => text.Text?.Trim().Contains(t.TextToReplace) == true)) {

                    //                            var wrd = items.FirstOrDefault(it => text.Text.Contains(it.TextToReplace));

                    //                            while(items.Any(t => text.Text?.Trim().Contains(t.TextToReplace) == true)) {
                    //                                wrd = items.FirstOrDefault(it => text.Text.Contains(it.TextToReplace));
                    //                                text.Text = text.Text.Replace(wrd.TextToReplace, wrd.ReplacementText);
                    //                            }




                    //                        }
                    //                    }
                    //                }
                    //            } else {

                    //                replace.Checkboxes.ForEach(c => {
                    //                    r.InsertAfterSelf(c);
                    //                });
                    //                para.RemoveChild(r);
                    //            }
                    //        }

                    //    }
                    //}


                    //foreach (var table in tables) {
                    //    var rows = table.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>();
                    //    foreach (var row in rows) {
                    //        var cells = row.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>();
                    //        foreach (var cell in cells) {
                    //            var ps = cell.Elements<Paragraph>();
                    //            foreach (var p in ps) {

                    //                var runs = p.Elements<Run>().ToList();


                    //                for (int i = 0; i < runs.Count; i++) {
                    //                    var r = runs[i];
                    //                    var texts = r.Elements<Text>();

                    //                    if ((items.Any(t => t.TextToReplace == r.InnerText?.Trim()))) {
                    //                        var replace = items.Where(t => t.TextToReplace == r.InnerText?.Trim()).FirstOrDefault();

                    //                        if (!replace.IsCheckBox) {
                    //                            foreach (var text in texts) {
                    //                                if (text != null) {
                    //                                    if (items.Any(t => t.TextToReplace == text.Text?.Trim()))
                    //                                        text.Text = text.Text.Replace(text.Text, items.Where(t => t.TextToReplace == text.Text?.Trim()).FirstOrDefault().ReplacementText);
                    //                                }
                    //                            }
                    //                        } else {

                    //                            replace.Checkboxes.ForEach(c => {
                    //                                r.InsertAfterSelf(c);
                    //                            });
                    //                            p.RemoveChild(r);
                    //                        }
                    //                    }

                    //                }



                    //            }
                    //        }
                    //    }
                    //}



                    //var allruns = body.Elements<Run>();

                    //foreach (var run in allruns) {
                    //    foreach (var text in run.Elements<Text>()) {
                    //        if (text != null) {
                    //            if (items.Any(t => t.TextToReplace == text.Text))
                    //                text.Text = text.Text.Replace(text.Text, items.Where(t => t.TextToReplace == text.Text).FirstOrDefault().ReplacementText);
                    //        }
                    //    }
                    //}
                    
                }
            }
        }

        public FileModel SaveDoc(string filename,byte[] data) {

            var dest = ConfigurationManager.AppSettings["docstorage"].ToString();
            
            if (dest.StartsWith("~")) {
                dest = HttpContext.Current.Server.MapPath(dest);
            }
            
            var path = Path.Combine(dest, filename);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            if (File.Exists(path)) {
                File.Delete(path);
            }

            File.WriteAllBytes(path, data);

            return new FileModel() {
                FileName = Path.GetFileName(filename),
                Mime = MimeMapping.GetMimeMapping(filename)
            };
        }

        public FileModel GetDoc(string filename) {
            var result = new FileModel() {
                FileName = Path.GetFileName(filename),
                Mime = MimeMapping.GetMimeMapping(filename)
            };

            var dest = ConfigurationManager.AppSettings["docstorage"].ToString();

            if (dest.StartsWith("~")) {
                dest = HttpContext.Current.Server.MapPath(dest);
            }

            var path = Path.Combine(dest, filename);

            if (File.Exists(path)) {

                result.Bytes = File.ReadAllBytes(path);


                return result;
            }

            return null;


        }

        public bool DeleteDoc(string filename) {

            var dest = ConfigurationManager.AppSettings["docstorage"].ToString();

            if (dest.StartsWith("~")) {
                dest = HttpContext.Current.Server.MapPath(dest);
            }

            var path = Path.Combine(dest, filename);
            try {
                File.Delete(filename);
                return true;
            } catch {
                return false;   
            }
            
        }

        public bool DeleteDir(string dirname) {

            var dest = ConfigurationManager.AppSettings["docstorage"].ToString();

            if (dest.StartsWith("~")) {
                dest = HttpContext.Current.Server.MapPath(dest);
            }

            try {
                var path = Path.Combine(dest, dirname);

                Directory.Delete(path, true);
                return true;
            } catch {
                return false;
            }
            
        }

        public bool IsExists(string filename) {

            var dest = ConfigurationManager.AppSettings["docstorage"].ToString();

            if (dest.StartsWith("~")) {
                dest = HttpContext.Current.Server.MapPath(dest);
            }

            var path = Path.Combine(dest, filename);

            return File.Exists(path);
        }

        public FileModel WordToPdf(byte[] data, string fileName = null) {
            byte[] byteArray = data;
            using (MemoryStream memoryStream = new MemoryStream()) {
                memoryStream.Write(byteArray, 0, byteArray.Length);
                using (WordprocessingDocument doc =
                    WordprocessingDocument.Open(memoryStream, true)) {
                    HtmlConverterSettings settings = new HtmlConverterSettings() {
                        //PageTitle = "My Page Title"
                        AdditionalCss = "td { border : 1px solid #000 !important; }"
                    };
                    XElement html = HtmlConverter.ConvertToHtml(doc, settings);

                    SelectPdf.HtmlToPdf htmlToPdf = new SelectPdf.HtmlToPdf();

                    return _pdfSvc.GetPdfFromHtmlString(html.ToStringNewLineOnAttributes(), Path.GetFileNameWithoutExtension(fileName)??"Document"+".pdf");

                    //File.writeal(@"Test.html", html.ToStringNewLineOnAttributes());
                }
            }
        }
    }
}