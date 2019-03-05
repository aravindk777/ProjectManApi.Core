using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace PM.BL.Common
{
    public static class Converter
    {
        #region Projects - Mapping configuration

        #region Data Model to ViewModel
        public static PM.Models.ViewModels.Project AsViewModel(this PM.Models.DataModels.Project projectData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.DataModels.Project, PM.Models.ViewModels.Project>()
                .ForMember(vm => vm.ManagerName, dm => dm.MapFrom(m => m.Manager != null ? m.Manager.FirstName + (!string.IsNullOrEmpty(m.Manager.LastName) ? $" {m.Manager.LastName}" : string.Empty) : string.Empty))
                .ReverseMap();
            });

            return config.CreateMapper().Map<PM.Models.DataModels.Project, PM.Models.ViewModels.Project>(projectData);
        }

        public static IEnumerable<PM.Models.ViewModels.Project> AsViewModel(this IEnumerable<PM.Models.DataModels.Project> projectData)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PM.Models.DataModels.Project, PM.Models.ViewModels.Project>()
                .ForMember(vm => vm.ManagerName, dm => dm.MapFrom(m => m.Manager != null ? m.Manager.FirstName + (!string.IsNullOrEmpty(m.Manager.LastName) ? $" {m.Manager.LastName}" : string.Empty) : string.Empty))
                .ReverseMap();
            });

            return config.CreateMapper().Map<IEnumerable<PM.Models.DataModels.Project>, IEnumerable<PM.Models.ViewModels.Project>>(projectData);
        }
        #endregion

        #region View Model to Data Model
        public static PM.Models.DataModels.Project AsDataModel(this PM.Models.ViewModels.Project projectData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.ViewModels.Project, PM.Models.DataModels.Project>().ReverseMap();
            });

            return config.CreateMapper().Map<PM.Models.ViewModels.Project, PM.Models.DataModels.Project>(projectData);
        }

        public static IEnumerable<PM.Models.DataModels.Project> AsDataModel(this IEnumerable<PM.Models.ViewModels.Project> projectData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<IEnumerable<PM.Models.ViewModels.Project>, IEnumerable<PM.Models.DataModels.Project>>().ReverseMap();
            });

            return config.CreateMapper().Map<IEnumerable<PM.Models.ViewModels.Project>, IEnumerable<PM.Models.DataModels.Project>>(projectData);
        }
        #endregion
        #endregion

        #region User - Mapping configuration

        public static PM.Models.ViewModels.User AsViewModel(this PM.Models.DataModels.User userData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.DataModels.User, PM.Models.ViewModels.User>()
                .ForMember(vm => vm.Active, d => d.MapFrom(m => !(m.EndDate.HasValue && m.EndDate.Value.CompareTo(System.DateTime.Today) <= 0)))
                .ReverseMap();
            });

            return config.CreateMapper().Map<PM.Models.DataModels.User, PM.Models.ViewModels.User>(userData);
        }

        public static IEnumerable<PM.Models.ViewModels.User> AsViewModel(this IEnumerable<PM.Models.DataModels.User> userData)
        {
            var config = new MapperConfiguration(cfg => {                
                cfg.CreateMap<PM.Models.DataModels.User, PM.Models.ViewModels.User>()
                .ForMember(vm => vm.Active, d => d.MapFrom(m => !m.EndDate.HasValue))
                .ReverseMap();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<IEnumerable<PM.Models.DataModels.User>, IEnumerable<PM.Models.ViewModels.User>>(userData);
        }

        public static PM.Models.DataModels.User AsDataModel(this PM.Models.ViewModels.User userData, bool isCreate = false)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.ViewModels.User, PM.Models.DataModels.User>()
                .ReverseMap();
            });
            var mapper = config.CreateMapper();
            if (isCreate)
                return mapper.Map<PM.Models.ViewModels.User, PM.Models.DataModels.User>(userData, 
                    opts => opts.ConfigureMap()
                            .ForMember(m => m.Created, d => d.AddTransform(v => System.DateTime.Now)));
            return mapper.Map<PM.Models.ViewModels.User, PM.Models.DataModels.User>(userData);
        }

        public static IEnumerable<PM.Models.DataModels.User> AsDataModel(this IEnumerable<PM.Models.ViewModels.User> userData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<IEnumerable<PM.Models.ViewModels.User>, IEnumerable<PM.Models.DataModels.User>>().ReverseMap();
            });

            return config.CreateMapper().Map<IEnumerable<PM.Models.ViewModels.User>, IEnumerable<PM.Models.DataModels.User>>(userData);
        }
        #endregion

        #region Tasks

        public static PM.Models.ViewModels.Task AsViewModel(this PM.Models.DataModels.Task taskData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.DataModels.Task, PM.Models.ViewModels.Task>()
                .ForMember(vm => vm.OwnerFullName, dm => dm.MapFrom(m => m.TaskOwner != null ? m.TaskOwner.FirstName + (!string.IsNullOrEmpty(m.TaskOwner.LastName) ? $" {m.TaskOwner.LastName}" : string.Empty) : string.Empty))
                .ForMember(vm => vm.ProjectName, dm => dm.MapFrom(m => m.Project != null ? m.Project.ProjectName : string.Empty))
                .ForMember(vm => vm.ParentTaskName, dm => dm.MapFrom(m => m.ParentTask != null ? m.ParentTask.TaskName : string.Empty))
                .ReverseMap();
            });

            return config.CreateMapper().Map<PM.Models.DataModels.Task, PM.Models.ViewModels.Task>(taskData);
        }

        public static IEnumerable<PM.Models.ViewModels.Task> AsViewModel(this IEnumerable<PM.Models.DataModels.Task> taskData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.DataModels.Task, PM.Models.ViewModels.Task>()
                .ForMember(vm => vm.OwnerFullName, dm => dm.MapFrom(m => m.TaskOwner != null ? m.TaskOwner.FirstName + (!string.IsNullOrEmpty(m.TaskOwner.LastName) ? $" {m.TaskOwner.LastName}" : string.Empty) : string.Empty))
                .ForMember(vm => vm.ProjectName, dm => dm.MapFrom(m => m.Project != null ? m.Project.ProjectName : string.Empty))
                .ForMember(vm => vm.ParentTaskName, dm => dm.MapFrom(m => m.ParentTask != null ? m.ParentTask.TaskName : string.Empty))
                .ReverseMap();
            });

            return config.CreateMapper().Map<IEnumerable<PM.Models.DataModels.Task>, IEnumerable<PM.Models.ViewModels.Task>>(taskData);
        }

        public static PM.Models.DataModels.Task AsDataModel(this PM.Models.ViewModels.Task taskData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.ViewModels.Task, PM.Models.DataModels.Task>().ReverseMap();
            });

            return config.CreateMapper().Map<PM.Models.ViewModels.Task, PM.Models.DataModels.Task>(taskData);
        }

        public static IEnumerable<PM.Models.DataModels.Task> AsDataModel(this IEnumerable<PM.Models.ViewModels.Task> taskData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<IEnumerable<PM.Models.ViewModels.Task>, IEnumerable<PM.Models.DataModels.Task>>().ReverseMap();
            });

            return config.CreateMapper().Map<IEnumerable<PM.Models.ViewModels.Task>, IEnumerable<PM.Models.DataModels.Task>>(taskData);
        }

        #endregion
    }
}
