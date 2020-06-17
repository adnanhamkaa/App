using AutoMapper;
using App.DataAccess;
using App.DataAccess.Model;
using App.Web.Models;
#region Sample
using App.Web.Areas.Samples.Model;
#endregion Sample

namespace App.Web
{
    public class AutoMapperConfig {
        public static void InitMapper() {
            Mapper.Initialize(RegisterMapper);
        }

        private static void RegisterMapper(IMapperConfigurationExpression cfg) {

            cfg.CreateMap<ModelBase, FormModelBase>().ReverseMap()
                .ForMember(t => t.CreatedBy, opt => 
                    opt.Condition((obj,dest) => string.IsNullOrEmpty(dest.CreatedBy))
                )
                .ForMember(t => t.CreatedDate, opt => 
                    opt.Condition((obj, dest) => dest.CreatedDate == null)
                )
                .IncludeAllDerived();

            cfg.CreateMap<Workflow, WorkflowForm>().ReverseMap()
                .ForMember(t => t.Flows, opt => opt.Ignore());
            cfg.CreateMap<Flow, FlowForm>().ReverseMap();
            cfg.CreateMap<Flow, ToDoForm>().ReverseMap();
            cfg.CreateMap<ActivityLog, ActivityLogModel>().ReverseMap();

            
            #region Sample
            cfg.CreateMap<Movie, MovieForm>().ReverseMap()
                .ForMember(t => t.ShowTimes, opt => opt.Ignore());
            cfg.CreateMap<Theatre, TheatreForm>().ReverseMap()
                .ForMember(t => t.ShowTimes, opt => opt.Ignore());
            cfg.CreateMap<ShowTime, ShowTimeForm>().ReverseMap()
                .ForMember(t => t.Movie, opt => opt.Ignore())
                .ForMember(t => t.Theatre, opt => opt.Ignore());
            cfg.CreateMap<Ticket, TicketForm>().ReverseMap()
                .ForMember(t => t.ShowTime, opt => opt.Ignore());
            #endregion Sample
        }
    }
}