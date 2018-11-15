using AABC.Domain2.Notes;
using AABC.DomainServices.Notes;
using AABC.Web.App.Models;
using AABC.Web.Repositories;
using Dymeng.Framework.Web.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AABC.Web.App.Notes
{

    public abstract class BaseNotesController<TNoteService, TTaskService, TNote, TNoteTask> : ContentBaseController
        where TNoteService : BaseNotesService<TNote, TNoteTask>
        where TTaskService : BaseTasksService<TNoteTask>
        where TNote : BaseNote
        where TNoteTask : BaseNoteTask
    {
        protected SourceType SourceType { get; private set; }
        private readonly TNoteService NotesService;
        private readonly TTaskService TaskService;
        private readonly IOfficeStaffRepository StaffRepository;

        public BaseNotesController(SourceType sourceType, TNoteService notesService, TTaskService taskService)
        {
            SourceType = sourceType;
            NotesService = notesService;
            TaskService = taskService;
            StaffRepository = new OfficeStaffRepository();
            ViewBag.SourceType = sourceType.ToString();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult NoteEdit(int parentID, int id, bool isModal = false)
        {
            var model = id != 0 ? NotesService.GetNoteEditExisting(id) : NotesService.GetNoteEditNew(parentID);
            ViewBag.IsModal = isModal;
            return PartialView("~/App/Notes/Views/NoteEdit.cshtml", model);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult NoteEditFromTask(int taskId, bool isModal = false)
        {
            var model = NotesService.GetNoteEditExistingByTaskId(taskId);
            ViewBag.IsModal = isModal;
            return PartialView("~/App/Notes/Views/NoteEdit.cshtml", model);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(NoteDTO model)
        {
            var user = Global.Default.User();
            NotesService.SaveNote(model, user.ID.Value);
            return NoteEdit(model.ParentID, 0);
        }


        [HttpGet]
        public ActionResult GetNotes(int parentID)
        {
            return this.CamelCaseJson(NotesService.GetNotes(parentID), JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult NotesSummaryList(int parentID)
        {
            var model = new NotesSummaryListVM
            {
                SummaryItems = NotesService.GetNotes(parentID),
                ParentID = parentID
            };
            return PartialView("~/App/Notes/Views/NotesSummaryList.cshtml", model);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetNewNote(int parentID)
        {
            /*todo compare with NotesSummaryList*/
            var model = new NotesSummaryListVM
            {
                SummaryItems = NotesService.GetNewNotes(parentID),
                ParentID = parentID
            };
            return PartialView("~/App/Notes/Views/NotesSummaryList.cshtml", model);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult FollowupCompletePopup(int noteID)
        {
            var model = new NoteFollowupSaveRequest
            {
                NoteID = noteID
            };
            return PartialView("~/App/Notes/Views/NoteFollowupCompletePopup.cshtml", model);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FollowupComplete(NoteFollowupSaveRequest model)
        {
            var user = Global.Default.User();
            NotesService.SaveNoteFollowup(model, user.ID.Value);
            var noteSummary = NotesService.GetNoteSummary(model.NoteID);
            var retModel = new NotesSummaryListVM()
            {
                SummaryItems = new List<NoteDetailsDTO> { noteSummary }
            };
            return PartialView("~/App/Notes/Views/NotesSummaryList.cshtml", retModel);
        }

        #region Tasks
        public ActionResult TaskEditRow(int taskId)
        {
            var model = taskId > 0 ? TaskService.GetTask(taskId) : new NoteTaskDTO(SourceType);
            model.AssignedToList = StaffRepository.GetOfficeStaffList();
            return PartialView("~/App/Notes/Views/TaskEditPartial.cshtml", model);
        }


        [HttpGet]
        public ActionResult TaskCompleteForm(int taskID)
        {
            var task = TaskService.GetTask(taskID);
            task.AssignedToList = StaffRepository.GetOfficeStaffList();
            return View("~/App/Notes/Views/TaskCompleteForm.cshtml", task);
        }


        [HttpPost]
        public ActionResult TaskCompleteForm(int ID, string Remarks, bool? CreateFollowTask, string Description, int? AssignedTo, DateTime? DueDate, int NoteID)
        {
            TaskService.TaskComplete(ID, Remarks);
            if (CreateFollowTask.GetValueOrDefault(false))
            {
                var task = new NoteTaskDTO(SourceType)
                {
                    AssignedTo = AssignedTo,
                    Description = Description,
                    DueDate = DueDate,
                    NoteID = NoteID
                };
                TaskService.Save(task);
            }
            return Json(new { success = true });
        }
        #endregion
    }
}
