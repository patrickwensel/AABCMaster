using AABC.Mobile.SharedEntities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Interfaces
{
	public interface IUpdateService
	{
		Task<CurrentVersionResponse> UpdateRequired();

		Task RedirectToUpdate();
	}
}
