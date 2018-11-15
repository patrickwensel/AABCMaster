using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Interfaces
{
	public interface ISecureAppStorage
	{
		string AppInstanceId { get; set; }

		string Username { get; set; }

		string HashedPassword { get; set; }
	}
}
