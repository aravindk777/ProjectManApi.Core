using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace PM.BL.Common
{
    public static class Converter
    {
        #region Projects - Mapping configuration

        #region Data Model to ViewModel
        public static Models.ViewModels.Project AsViewModel(this Models.DataModels.Project projectData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Models.DataModels.Project, Models.ViewModels.Project>()
                .ForMember(vm => vm.ManagerName, dm => dm.MapFrom(m => m.Manager != null ? m.Manager.FirstName + (!string.IsNullOrEmpty(m.Manager.LastName) ? $" {m.Manager.LastName}" : string.Empty) : string.Empty))
                .ForMember(vm => vm.IsActive, dm => dm.MapFrom(m => !(m.ProjectEnd.HasValue && m.ProjectEnd.Value.CompareTo(System.DateTime.Today) <= 0)))
                .ReverseMap();
            });

            return config.CreateMapper().Map<Models.DataModels.Project, Models.ViewModels.Project>(projectData);
        }

        public static IEnumerable<Models.ViewModels.Project> AsViewModel(this IEnumerable<Models.DataModels.Project> projectData)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Models.DataModels.Project, Models.ViewModels.Project>()
                .ForMember(vm => vm.ManagerName, dm => dm.MapFrom(m => m.Manager != null ? m.Manager.FirstName + (!string.IsNullOrEmpty(m.Manager.LastName) ? $" {m.Manager.LastName}" : string.Empty) : string.Empty))
                .ForMember(vm => vm.IsActive, dm => dm.MapFrom(m => !(m.ProjectEnd.HasValue && m.ProjectEnd.Value.CompareTo(System.DateTime.Today) <= 0)))
                .ReverseMap();
            });

            return config.CreateMapper().Map<IEnumerable<Models.DataModels.Project>, IEnumerable<Models.ViewModels.Project>>(projectData);
        }
        #endregion

        #region View Model to Data Model
        public static Models.DataModels.Project AsDataModel(this Models.ViewModels.Project projectData, Models.DataModels.Project projectDataModel = null)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Models.ViewModels.Project, Models.DataModels.Project>();
            });
            if (projectDataModel == null)
                projectDataModel = config.CreateMapper().Map<Models.ViewModels.Project, Models.DataModels.Project>(projectData);
            else
                projectDataModel = config.CreateMapper().Map(projectData, projectDataModel);
            return projectDataModel;
        }

        public static IEnumerable<Models.DataModels.Project> AsDataModel(this IEnumerable<Models.ViewModels.Project> projectData, IEnumerable<Models.DataModels.Project> projectDataModel = null)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Models.ViewModels.Project, Models.DataModels.Project>();
            });

            if(projectDataModel == null)
                return config.CreateMapper().Map<IEnumerable<Models.ViewModels.Project>, IEnumerable<Models.DataModels.Project>>(projectData);
            else
                return config.CreateMapper().Map(projectData, projectDataModel);
        }        
        #endregion
        #endregion

        #region User - Mapping configuration

        public static Models.ViewModels.User AsViewModel(this Models.DataModels.User userData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Models.DataModels.User, Models.ViewModels.User>()
                .ForMember(vm => vm.Active, d => d.MapFrom(m => !(m.EndDate.HasValue && m.EndDate.Value.CompareTo(System.DateTime.Today) <= 0)))
                .ReverseMap();
            });

            return config.CreateMapper().Map<Models.DataModels.User, Models.ViewModels.User>(userData);
        }

        public static IEnumerable<Models.ViewModels.User> AsViewModel(this IEnumerable<Models.DataModels.User> userData)
        {
            var config = new MapperConfiguration(cfg => {                
                cfg.CreateMap<Models.DataModels.User, Models.ViewModels.User>()
                .ForMember(vm => vm.Active, d => d.MapFrom(m => !m.EndDate.HasValue))
                .ReverseMap();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<IEnumerable<Models.DataModels.User>, IEnumerable<Models.ViewModels.User>>(userData);
        }

        public static Models.DataModels.User AsDataModel(this Models.ViewModels.User userData, bool isCreate = false, Models.DataModels.User dataModel = null)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Models.ViewModels.User, Models.DataModels.User>();
            });

            var mapper = config.CreateMapper();
            if (isCreate)
                return mapper.Map<Models.ViewModels.User, Models.DataModels.User>(userData, 
                    opts => opts.ConfigureMap()
                            .ForMember(m => m.Created, d => d.AddTransform(v => System.DateTime.Now)));

            if (dataModel != null)
                return mapper.Map(userData, dataModel);
            else
                return mapper.Map<Models.DataModels.User>(userData);
        }

        public static IEnumerable<Models.DataModels.User> AsDataModel(this IEnumerable<Models.ViewModels.User> userData, IEnumerable<Models.DataModels.User> dataModel = null)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Models.ViewModels.User, Models.DataModels.User>().ReverseMap();
            });

            if (dataModel == null)
                return config.CreateMapper().Map<IEnumerable<Models.ViewModels.User>, IEnumerable<Models.DataModels.User>>(userData);
            else
                return config.CreateMapper().Map(userData, dataModel);
        }
        #endregion

        #region Tasks

        public static Models.ViewModels.Task AsViewModel(this Models.DataModels.Task taskData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Models.DataModels.Task, Models.ViewModels.Task>()
                .ForMember(vm => vm.OwnerFullName, dm => dm.MapFrom(m => m.TaskOwner != null ? m.TaskOwner.FirstName + (!string.IsNullOrEmpty(m.TaskOwner.LastName) ? $" {m.TaskOwner.LastName}" : string.Empty) : string.Empty))
                .ForMember(vm => vm.ProjectName, dm => dm.MapFrom(m => m.Project != null ? m.Project.ProjectName : string.Empty))
                .ForMember(vm => vm.ParentTaskName, dm => dm.MapFrom(m => m.ParentTask != null ? m.ParentTask.TaskName : string.Empty))
                .ForMember(vm => vm.IsParent, dm => dm.MapFrom(m => m.ParentTask == null ? true: false))
                .ForMember(vm => vm.IsActive, dm => dm.MapFrom(m => !(m.EndDate.HasValue && m.EndDate.Value.CompareTo(System.DateTime.Today) <= 0)))
                .ReverseMap();
            });

            return config.CreateMapper().Map<Models.DataModels.Task, Models.ViewModels.Task>(taskData);
        }

        public static IEnumerable<Models.ViewModels.Task> AsViewModel(this IEnumerable<Models.DataModels.Task> taskData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Models.DataModels.Task, Models.ViewModels.Task>()
                .ForMember(vm => vm.OwnerFullName, dm => dm.MapFrom(m => m.TaskOwner != null ? m.TaskOwner.FirstName + (!string.IsNullOrEmpty(m.TaskOwner.LastName) ? $" {m.TaskOwner.LastName}" : string.Empty) : string.Empty))
                .ForMember(vm => vm.ProjectName, dm => dm.MapFrom(m => m.Project != null ? m.Project.ProjectName : string.Empty))
                .ForMember(vm => vm.ParentTaskName, dm => dm.MapFrom(m => m.ParentTask != null ? m.ParentTask.TaskName : string.Empty))
                .ForMember(vm => vm.IsParent, dm => dm.MapFrom(m => m.ParentTask == null ? true : false))
                .ForMember(vm => vm.IsActive, dm => dm.MapFrom(m => !(m.EndDate.HasValue && m.EndDate.Value.CompareTo(System.DateTime.Today) <= 0)))
                .ReverseMap();
            });

            return config.CreateMapper().Map<IEnumerable<Models.DataModels.Task>, IEnumerable<Models.ViewModels.Task>>(taskData);
        }

        public static Models.DataModels.Task AsDataModel(this Models.ViewModels.Task taskData, Models.DataModels.Task dataModel = null)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Models.ViewModels.Task, Models.DataModels.Task>().ReverseMap();
            });

            if (dataModel == null)
                return config.CreateMapper().Map<Models.ViewModels.Task, Models.DataModels.Task>(taskData);
            else
                return config.CreateMapper().Map(taskData, dataModel);
        }

        public static IEnumerable<Models.DataModels.Task> AsDataModel(this IEnumerable<Models.ViewModels.Task> taskData, IEnumerable<Models.DataModels.Task> dataModel = null)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Models.ViewModels.Task, Models.DataModels.Task>().ReverseMap();
            });
            if (dataModel == null)
                return config.CreateMapper().Map<IEnumerable<Models.ViewModels.Task>, IEnumerable<Models.DataModels.Task>>(taskData);
            else
                return config.CreateMapper().Map(taskData, dataModel);
        }

        #endregion
    }
}
