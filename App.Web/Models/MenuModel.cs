using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.Web.Utilities;
//using JrzAsp.Lib.TypeUtilities;

namespace App.Web.Models {
    public class MenuModel {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string Action { get; set; }
        public string ClassName { get; set; }
        public List<MenuModel> Child { get; set; }
        public MenuType Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsHidden { get; set; }
        public bool ContainsMatch { get; set; }

        public MenuModel() {
            IsHidden = false;
        }

        private const string JSLINK = "javascript:;";

        public static List<MenuModel> GetMenu() {
            var result = new List<MenuModel>{
                new MenuModel() {
                    Title = "Sample",
                    Type=MenuType.Header,
                    Url=JSLINK,
                    Icon= "fa fa-ticket-alt",
                    Child = new List<MenuModel>() {
                        #region Sample
                        new MenuModel() {
                            Title = "Master Data",
                            Type = MenuType.Header,
                            Url = JSLINK,
                            Action = AppActions.Samples_masterdata,
                            Child = new List<MenuModel>() {
                                new MenuModel() {
                                    Title="Movie",
                                    Url="/Samples/Movie",
                                    Child = CreateCrudChildren("Samples","Movie")
                                },
                                new MenuModel() {
                                    Title="Theatre",
                                    Url="/Samples/Theatre",
                                    Child = CreateCrudChildren("Samples","Theatre")
                                },
                                new MenuModel() {
                                    Title="Showtime",
                                    Url="/Samples/Showtime",
                                    Child = CreateCrudChildren("Samples","Showtime")
                                }
                            }
                        },
                        #endregion Sample
                        new MenuModel() {
                            Title = "Ticket",
                            Url = "/Samples/Ticket"
                        }
                    }
                },
                new MenuModel() {
                    Title = "User Management",
                    Type=MenuType.Detail,
                    Url=JSLINK,
                    Icon= "fa fa-users",
                    Child = new List<MenuModel>() {
                        new MenuModel() {
                            Title = "User Management",
                            Type = MenuType.Detail,
                            Url = "/Account/Users",
                            Action = AppActions.usermanagement_registrasi
                        },
                        new MenuModel() {
                            Title = "Add User",
                            Type = MenuType.Detail,
                            Url = "/Account/Register",
                            IsHidden = true,
                            ContainsMatch = true
                        },
                        new MenuModel() {
                            Title = "Roles",
                            Type = MenuType.Detail,
                            Url = "/Account/Roles",
                            Action = AppActions.User_Management_Roles
                        },
                        new MenuModel() {
                            Title = "Add Role",
                            Type = MenuType.Detail,
                            Url = "/Account/RoleEntry",
                            IsHidden = true,
                            ContainsMatch = true,
                            Action = "Admin"
                        },
                        //new MenuModel() {
                        //    Title = "Reset Password",
                        //    Type = MenuType.Detail,
                        //    Url = "/Account/ResetPassword"
                        //},
                    }
                },
                new MenuModel() {
                    Title = "Activity Log",
                    Type=MenuType.Detail,
                    Url = "/ActivityLog",
                    Action = AppActions.Activity_Log,
                    Icon= "fa fa-user-clock"
                }

            };

            string path = HttpContext.Current.Request.Url.AbsolutePath;

            FindActive(result, path);

            var userRoles = WebHelper.GetUserActions();
            var userData = WebHelper.GetUser();
            Auth(result, userRoles);

            return result;
        }

        private static bool Auth(List<MenuModel> menus, List<string> userRoles) {
            if (userRoles == null) userRoles = new List<string>();
            foreach (var item in menus) {
                if (item.IsHidden) continue;

                item.Child = item.Child ?? new List<MenuModel>();

                if (item.Child.Where(t => t.IsHidden == false).ToList().Count > 0) {
                    item.IsHidden = !(Auth(item.Child, userRoles) && (string.IsNullOrEmpty(item.Action) ? true : userRoles.Any(t => item.Action.Contains(t))));
                    foreach (var child in item.Child)
                    {
                        if (!string.IsNullOrEmpty(child.Action))
                        {
                            child.IsHidden = !userRoles.Any(t => child.Action.Contains(t) && !string.IsNullOrEmpty(t));
                        }
                    }
                } else {
                    if (!string.IsNullOrEmpty(item.Action)) {
                        item.IsHidden = !userRoles.Any(t => item.Action.Contains(t) && !string.IsNullOrEmpty(t));
                    }
                }
            }

            return menus.Any(t => !t.IsHidden);

        }

        public static List<MenuModel> CreateCrudChildren(string areaname, string controllername) {
            return new List<MenuModel>() {
                new MenuModel() {
                    Title = "Entry",
                    Url = $"/{areaname}/{controllername}/Entry",
                    IsHidden = true,
                    ContainsMatch = false
                },
                new MenuModel() {
                    Title = "Edit",
                    Url = $"/{areaname}/{controllername}/Entry",
                    IsHidden = true,
                    ContainsMatch = true
                }
            };
        }

        public static List<MenuModel> CreateCrudChildren(string controllername) {
            return new List<MenuModel>() {
                new MenuModel() {
                    Title = "Entry",
                    Url = $"/{controllername}/Entry",
                    IsHidden = true,
                    ContainsMatch = false
                },
                new MenuModel() {
                    Title = "Edit",
                    Url = $"/{controllername}/Entry",
                    IsHidden = true,
                    ContainsMatch = true
                },
                new MenuModel() {
                    Title = "View Detail",
                    Url = $"/{controllername}/Preview",
                    IsHidden = true,
                    ContainsMatch = true
                }
            };
        }

        public static List<MenuModel> GetBreadCrumbs() {
            
            var menus = GetMenu();

            var result = menus.Where(t => t.IsActive).Flatten(t => (t.Child ?? new List<MenuModel>()).Where(e => e.IsActive)).ToList();

            if (result.Count > 0) result.RemoveAt(0);

            return result;
        }

        private static bool FindActive(List<MenuModel> menu, string path) {
            menu = menu ?? new List<MenuModel>();
            path = path.Trim().ToLower();
            for (int i = 0; i < menu.Count; i++) {
                var item = menu[i];

                if (item.Url.Trim().ToLower() == path.Trim()) {
                    menu[i].IsActive = true;
                    return true;
                }

                if (item.ContainsMatch) {
                    if (path.Trim().Contains(item.Url.Trim().ToLower())) {
                        menu[i].IsActive = true;
                        return true;
                    }
                }


                if (item.Child != null) {
                    menu[i].IsActive = FindActive(menu[i].Child, path);
                    if (menu[i].IsActive) {
                        return true;
                    }
                }
            }

            return false;
        }


    }

    public enum MenuType {
        Detail,
        Header

    }
}