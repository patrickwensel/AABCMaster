using AABC.Data.V2;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.Notes
{
    public class TaskService
    {
        private readonly CaseNoteTasksService CaseNoteTasksService;
        private readonly ReferralNoteTasksService ReferralNoteTasksService;
        private readonly IUserProvider UserProvider;

        public TaskService(CoreContext context, IUserProvider userProvider)
        {
            CaseNoteTasksService = new CaseNoteTasksService(context);
            ReferralNoteTasksService = new ReferralNoteTasksService(context);
            UserProvider = userProvider;
        }

        public IEnumerable<NoteTaskDTO> GetAllIncompleteTasks()
        {
            var user = UserProvider.GetUser();
            var elements = CaseNoteTasksService.GetIncompleteTaskListForCurrentUser(user).ToList();
            elements.AddRange(ReferralNoteTasksService.GetIncompleteTaskListForCurrentUser(user));
            return elements.OrderByDescending(x => x.DueDate);
        }


        public IEnumerable<NoteTaskDTO> GetRecentlyCompletedTasks()
        {
            var user = UserProvider.GetUser();
            var elements = CaseNoteTasksService.GetRecentlyCompletedTaskListForCurrentUser(user).ToList();
            elements.AddRange(ReferralNoteTasksService.GetRecentlyCompletedTaskListForCurrentUser(user));
            return elements.OrderByDescending(x => x.CompletedDate);
        }
    }
}
