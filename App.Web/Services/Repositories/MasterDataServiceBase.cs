using AutoMapper;
using App.DataAccess;
using App.Web.Models;
using App.Web.Services.Contracts;
using App.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using App.Utilities.Model;

namespace App.Web.Services.Repositories
{
    public class CrudServiceBase<TModel, TForm> : ServiceBase where TModel : ModelBase, new() where TForm : FormModelBase, new() {
        public virtual string[] Includes => new string[] { };
        public virtual string DocTemplate => null;
        public virtual string DocResultName => Path.GetFileName(DocTemplate);

        public IMasterDataServices SetupSvc { get; set; }
        private IWordTextReplacementServices _wordSvc { get; set; }

        public CrudServiceBase (ApplicationDbContext context, IMasterDataServices setupSvc, IWordTextReplacementServices wordSvc){
            this.context = context;
            this.SetupSvc = setupSvc;
            this._wordSvc = wordSvc;
        }

        public virtual List<TForm> GetAll() {
            var result = SetupSvc.GetAll<TModel>(true).ToList();
            return Mapper.Map<List<TModel>, List<TForm>>(result);
        }

        public virtual List<TForm> GetAllWithDetail() {
            var result = SetupSvc.GetAll<TModel>(false,Includes).ToList();
            return Mapper.Map<List<TModel>, List<TForm>>(result);
        }

        public virtual TForm GetById(string id) {   
            var obj = SetupSvc.GetById<TModel>(id, Includes);
            return Mapper.Map<TModel, TForm>(obj);
        }

        public virtual TForm Insert(TForm form, bool commit = true) {

            var result = SetupSvc.Upsert<TModel, TForm>(form,Includes);

            MapRelation(result, form);

            if (commit) context.SaveChanges();

            return Mapper.Map<TForm>(result);
        }

        public List<TModel> ExecuteList<T>(T form) where T : DttRequestWithDate {
            form.search.value = (form.search.value ?? "").ToLower();

            var query = context.Set<TModel>().IncludeAll(Includes).Where(t => t.IsDeleted != true);

            //if (form.StartDate != null) {
            //    query = query.Where(t => t.CreatedDate >= form.StartDate);
            //}

            //if (form.EndDate != null) {
            //    query = query.Where(t => t.CreatedDate <= form.EndDate);
            //}
            //if (form.StartDate.HasValue && form.EndDate.HasValue)
            //{
            //    query = query.Where(t => DbFunctions.TruncateTime(t.CreatedDate) >= DbFunctions.TruncateTime(form.StartDate)
            //    && DbFunctions.TruncateTime(t.CreatedDate) <= DbFunctions.TruncateTime(form.EndDate));
            //}

            IQueryable<TModel> Filterbycreated = null;
            IQueryable<TModel> Filterbyupdated = null;

            var tanggalupdate = query.Where(a => a.UpdatedDate == null);

            if (tanggalupdate != null)
            {
                Filterbycreated = tanggalupdate.Where(
                 t => DbFunctions.TruncateTime(t.CreatedDate) >= DbFunctions.TruncateTime(form.StartDate)
                 && DbFunctions.TruncateTime(t.CreatedDate) <= DbFunctions.TruncateTime(form.EndDate));
            }

            var tanggalupdate2 = query.Where(a => a.UpdatedDate != null);
            if (tanggalupdate2 != null)
            {
                Filterbyupdated = tanggalupdate2.Where(
                t => DbFunctions.TruncateTime(t.UpdatedDate) >= DbFunctions.TruncateTime(form.StartDate)
                && DbFunctions.TruncateTime(t.UpdatedDate) <= DbFunctions.TruncateTime(form.EndDate));
            }

            if (form.StartDate.HasValue && form.EndDate.HasValue)
            {
                query = Filterbycreated.Concat(Filterbyupdated);
            }

            return query.ToList();

        }

        public DttResponseForm<TForm> UseExecutedList<T>(T form) where T : DttRequestWithDate {
            var result = new DttResponseForm<TForm>();
            result.draw = form.draw;

            var data = ExecuteList<T>(form);
            result.recordsTotal = data.Count();
            data = FilterExecutedList<T>(data, form);

            data = GetPaging<TModel>(data, form).ToList();
            
            result.data = Mapper.Map<List<TModel>, List<TForm>>(data);
            
            result.recordsFiltered = result.recordsTotal;

            return result;
        }

        public virtual List<TModel> FilterExecutedList<T>(List<TModel> data,T form) where T : DttRequestWithDate {
            return data;
        }

        public virtual DttResponseForm<TForm> GetList<T>(T form) where T : DttRequestWithDate {


            //var result = new DttResponseForm<TForm>();
            //result.draw = form.draw;
            //form.search.value = (form.search.value ?? "").ToLower();
            ////form.length = int.MaxValue;

            //var query =  Filter<T>(context.Set<TModel>().IncludeAll(Includes).Where(t => t.IsDeleted != true), form);

            ////if (form.StartDate != null) {
            ////    query = query.Where(t => t.CreatedDate >= form.StartDate);
            ////}

            ////if (form.EndDate != null) {
            ////    query = query.Where(t => t.CreatedDate <= form.EndDate);
            ////}

            //IQueryable<TModel> Filterbycreated = null;
            //IQueryable<TModel> Filterbyupdated = null;

            //var tanggalupdate = query.Where(a => a.UpdatedDate == null);

            //if (tanggalupdate != null)
            //{
            //    Filterbycreated = tanggalupdate.Where(
            //     t => DbFunctions.TruncateTime(t.CreatedDate) >= DbFunctions.TruncateTime(form.StartDate)
            //     && DbFunctions.TruncateTime(t.CreatedDate) <= DbFunctions.TruncateTime(form.EndDate));
            //}

            //var tanggalupdate2 = query.Where(a => a.UpdatedDate != null);
            //if (tanggalupdate2 != null)
            //{
            //    Filterbyupdated = tanggalupdate2.Where(
            //    t => DbFunctions.TruncateTime(t.UpdatedDate) >= DbFunctions.TruncateTime(form.StartDate)
            //    && DbFunctions.TruncateTime(t.UpdatedDate) <= DbFunctions.TruncateTime(form.EndDate));
            //}

            //if (form.StartDate.HasValue && form.EndDate.HasValue)
            //{
            //    query = Filterbycreated.Concat(Filterbyupdated);
            //}

            //var data = GetPaging<TModel>(query, form).ToList();

            //result.recordsTotal = query.Count();
            //result.recordsFiltered = result.recordsTotal;

            //result.data = Mapper.Map<List<TModel>, List<TForm>>(data);

            //return result;
            var result = ProcessDatatableResult<TForm, TModel>(context.Set<TModel>().IncludeAll(Includes).AsQueryable(), form);

            return result;

        }

        public virtual DttResponseForm<TForm> GetDynamicList(DttRequestForm form) {
            
            return ProcessDatatableResult<TForm,TModel>(context.Set<TModel>().IncludeAll(Includes), form);//Filter<T>(context.Set<TModel>().IncludeAll(Includes).Where(t => t.IsDeleted != true), form);
            
        }

        public override void RestoreDelete(string id) {
            RestoreDeleteData<TModel>(id);
        }

        public virtual IQueryable<TModel> Filter<T>(IQueryable<TModel> query,T form) where T : DttRequestWithDate {
            return query;
        }

        public TModel InsertWithoutCommit(TForm form) {

            var result = SetupSvc.Upsert<TModel, TForm>(form, Includes);

            return result;
        }

        public TModel MapRelation(TModel model, TForm form) {

            if (model == null || form == null) return null;

            var formProps = form.GetType().GetProperties();

            foreach (var prop in model.GetType().GetProperties().Where(t => t.PropertyType.IsSubclassOf(typeof(ModelBase)))) {

                if(formProps.Any(t => t.Name == prop.Name)) {

                    var mapProp = formProps.FirstOrDefault(t => t.Name == prop.Name);
                    var idFieldattr = mapProp.GetCustomAttributes(typeof(IdFieldAttribute), true).FirstOrDefault() as IdFieldAttribute;

                    if(idFieldattr != null) {

                        var id = form.GetPropValue(idFieldattr.FieldName);
                        var data = context.Set(prop.PropertyType).Find(id);
                        model.SetPropValue(prop.Name, data);
                    }

                }

            }

            return model;
        }

        public bool CheckIsSameDataExists(TForm form) {

            var data = Mapper.Map<TForm, TModel>(form);

            return IsSameDataExists<TModel,TForm>(context.Set<TModel>().AsQueryable(), form);

        }
        
        public TModel InsertWithoutCommit(TForm form, out bool isEdit) {

            isEdit = form.Id != null;

            var result = SetupSvc.Upsert<TModel, TForm>(form, Includes);

            return result;
        }

        public virtual TForm Delete(string id,bool commit = true) {

            var result = SetupSvc.Delete<TModel>(id);

            if(commit) context.SaveChanges();

            return Mapper.Map<TForm>(result);
        }

        public virtual TForm DeleteData(string id, string userName)
        {
            return new TForm();
        }

        public FileModel GetDoc(string id) {
            var data = GetById(id);
            
            return new FileModel() {
                Bytes = _wordSvc.GetWordReplacedText(DocTemplate, GetReplaceItems(data)),
                FileName = WordFilename(data),
                Mime = FileModel.MIME_DOCX
            };

        }

        public virtual List<WordReplacement> GetReplaceItems(TForm form) {
            return new List<WordReplacement>();
        }
        
        public virtual string WordFilename(TForm T) {
            return DocResultName;
        }

        public DateTime? StringToDatetime(string query,string format = "dd/MM/yyyy") {
            try {
                return DateTime.ParseExact(query, format,null);
            } catch (Exception exc) {

                return null;
            }
        }
        
        public int? StringToInt(string query) {
            try {
                return int.Parse(query);
            } catch (Exception exc) {

                return null;
            }
        }
        
        public decimal? StringToDecimal(string query) {
            try {
                return decimal.Parse(query);
            } catch (Exception exc) {

                return null;
            }
        }
    }
}