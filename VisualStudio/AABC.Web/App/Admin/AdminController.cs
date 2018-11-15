using AABC.Web.App.Account.Models;
using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace AABC.Web.Controllers
{
    public class AdminController : Dymeng.Framework.Web.Mvc.Controllers.ContentBaseController
    {



        App.Account.IUserRepository repository;
        App.Admin.AdminService _service;

        public AdminController()
        {
            this.repository = new App.Account.UserRepository();
            _service = new App.Admin.AdminService();
        }

        public AdminController(App.Account.IUserRepository userRepository)
        {
            this.repository = userRepository;
            _service = new App.Admin.AdminService();
        }





        [HttpGet]
        //[Route("~/Admin/{view}")]      (defined view MapRoutes)
        public ActionResult ManageDirectRoute(string view)
        {

            if (view == "Users")
                return Manage(0);
            if (view == "Provider Portal")
                return Manage(1);
            if (view == "Patient Portal")
                return Manage(2);
            if (view == "DataLists")
                return Manage(3);
            if (view == "Commands")
                return Manage(4);

            return Manage(0);
        }

        [HttpGet]
        public ActionResult Manage(int? tabIndex, string tempPass = null)
        {

            ViewBag.TabIndex = tabIndex;
            if (tempPass != null)
            {
                ViewBag.TempPass = tempPass;
            }

            return GetView("AdminContainer"); // view renders main via GetTab action
        }



        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult GetTab()
        {

            string tabName = Request.Params["tabName"];

            switch (tabName)
            {

                case "Users":
                    return Users();
                case "ProviderPortal":
                    return ProviderPortal();
                case "PatientPortal":
                    return PatientPortal();
                case "DataLists":
                    return DataLists();
                case "Commands":
                    return Commands();
                default:
                    return Users();
            }
        }



        #region Users


        [HttpGet]
        public ActionResult Users()
        {

            ManageUsersVM model = new ManageUsersVM
            {
                User = new UserVM(),
                UserPermissionList = new PermissionListVM(),
                UserOptionList = new OptionListVM()
            };

            model.User.ViewHelper.OfficeStaffList = repository.GetOfficeStaffList();
            model.User.Base = new ViewModelBase(
                PushState,
                "/Admin/Manage",
                "Manage Users");

            if (model.User.ID != null)
            {
                model.UserPermissionList.PermissionList = repository.GetUserPermissionList(model.User.ID);
                model.UserOptionList.OptionList = repository.GetUserOptionList(model.User.ID);
            }

            return GetViewOrGridCallback("Users", "UsersListGrid", model);

        }

        [HttpGet]
        public ActionResult UserChangePassword()
        {

            return PartialView();

        }


        [HttpPost]
        public ActionResult ResetPassword(int userID)
        {

            if (!Global.Default.User().HasPermission(Domain.Admin.Permissions.UserManagement))
            {
                return Content("err");
            }

            if (_service.ResetPassword(userID, out string newPass))
            {
                return Content(newPass);
            }
            else
            {
                return Content("err");
            }
        }




        [HttpPost]
        public ActionResult RegenUserPermissions(int userID)
        {
            Data.Services.WebUserService.RegenPermissions(userID, System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString);
            return Content("ok");
        }

        [HttpPost]
        public ActionResult RegenUserOptions(int userID)
        {
            Data.Services.WebUserService.RegenOptions(userID, System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString);
            return Content("ok");
        }


        [HttpPost]
        public ActionResult UserPermissionIsAllowedUpdate(int permissionId, bool isAllowed)
        {

            repository.UserPermissionIsAllowedUpdate(permissionId, isAllowed);

            return new EmptyResult();
        }

        public ActionResult UserGrid()
        {
            UserListVM model = new UserListVM();
            model = repository.GetUserListItems();
            return View("UsersListGrid", model);
        }

        public ActionResult UserForm(int Id)
        {
            UserVM model;
            if (Id > 0)
            {
                model = repository.GetUser(Id);
                if (model.StaffMemberID.GetValueOrDefault(0) > 0)
                {
                    model.StaffMember = repository.GetOfficeStaffMember(model.StaffMemberID.Value);
                }
            }
            else
            {
                model = new UserVM();
            }
            model.ViewHelper.OfficeStaffList = repository.GetOfficeStaffList();
            return View("UserForm", model);
        }
        [HttpPost]
        public ActionResult UserFormSave(UserVM model)
        {
            if (model.ID.GetValueOrDefault(0) == 0)
            {

                try
                {
                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                }
                catch (Exception)
                {
                    // skip "initializedatabaseconnection can only be called once...
                    // TODO: find out exact exception so we're not burying everything
                }

                string Password = Membership.GeneratePassword(8, 1);
                WebSecurity.CreateUserAndAccount(model.UserName, Password);
                int iUser = WebSecurity.GetUserId(model.UserName);
                model.AspNetUserID = iUser;

                repository.SaveUser(model);
                // Load Permissions and Options for the Registered User
                repository.InsertPermissionsList(model.ID);
                repository.InsertOptionsList(model.ID);
            }
            else
            {
                repository.SaveUser(model);
            }

            return UserForm(model.ID.Value);
        }
        [HttpPost]
        public ActionResult UserCreate(string firstName, string lastName, string userName, string email, int? staffID)
        {

            UserVM model = new UserVM
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email
            };
            if (staffID.HasValue)
            {
                model.StaffMember = new Domain.OfficeStaff.OfficeStaff() { ID = staffID };
            }

            try
            {

                RegisterModel registerUser = new RegisterModel
                {
                    UserName = model.UserName,
                    Password = Domain.Admin.User.GenerateRandomPassword()
                };

                App.Account.AccountController accountController = new App.Account.AccountController
                {
                    ControllerContext = ControllerContext
                };
                accountController.Register(registerUser);

                int aspNetUserId = accountController.GetUserID(model.UserName);

                if ((aspNetUserId) != 0)
                {
                    model.AspNetUserID = aspNetUserId;
                    repository.SaveUser(model);

                    // TODO: Email Temporary Password to User


                    // Load Permissions and Options for the Registered User
                    repository.InsertPermissionsList(model.ID);
                    repository.InsertOptionsList(model.ID);

                    // send this along with the temp password so we can display it onscreen
                    return Manage(0, registerUser.Password);

                }

                return new Dymeng.Framework.Web.Mvc.Controllers.TransferResult("Manage");
            }
            catch (Exception e)
            {
                Dymeng.Framework.Exceptions.Handle(e, Global.GetWebInfo());
                return Content("ERR: " + e.Message);
            }

        }

        [HttpPost]
        public ActionResult UserChangePassword(ChangePasswordModel model)
        {

            if (ModelState.IsValid)
            {
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception e)
                {
                    Dymeng.Framework.Exceptions.Handle(e, Global.GetWebInfo());
                    changePasswordSucceeded = false;
                }
                if (changePasswordSucceeded)
                {
                    return Content("OK");
                }
                else
                {
                    return Content("ERR: The current password is incorrect or the new password is invalid.");
                }

            }

            return View(model);

        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            UserVM user = new UserVM();
            user = repository.GetUser(id);

            App.Account.AccountController accountController = new App.Account.AccountController
            {
                ControllerContext = this.ControllerContext
            };
            accountController.Delete(user.UserName);

            repository.DeleteUser(id);


            return RedirectToAction("Users");
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult UserGridCallback()
        {

            ManageUsersVM model = new ManageUsersVM
            {
                UserList = repository.GetUserListItems()
            };

            return PartialView("UsersListGrid", repository.GetUserListItems());
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult UserPermissionGridCallback(int userId)
        {

            PermissionListVM model = new PermissionListVM
            {
                PermissionList = repository.GetUserPermissionList(userId)
            };

            return PartialView("PermissionsListGrid", model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult UserOptionGridCallback(int userId)
        {

            OptionListVM model = new OptionListVM
            {
                OptionList = repository.GetUserOptionList(userId)
            };

            return PartialView("OptionsListGrid", model);
        }

        #endregion



        #region ProviderPortal

        [HttpGet]
        public ActionResult ProviderPortal()
        {
            return new Dymeng.Framework.Web.Mvc.Controllers.TransferResult("/ProviderPortal/Index");
        }

        #endregion



        #region PatientPortal

        [HttpGet]
        public ActionResult PatientPortal()
        {
            return new Dymeng.Framework.Web.Mvc.Controllers.TransferResult("/PatientPortal/Index");
        }

        #endregion



        #region DataLists

        [HttpGet]
        public ActionResult DataLists()
        {
            return new Dymeng.Framework.Web.Mvc.Controllers.TransferResult("/DataLists/Index");
        }

        #endregion



        #region Commands

        [HttpGet]
        public ActionResult Commands()
        {
            return new Dymeng.Framework.Web.Mvc.Controllers.TransferResult("/Commands/Index");
        }
        #endregion












    }
}
