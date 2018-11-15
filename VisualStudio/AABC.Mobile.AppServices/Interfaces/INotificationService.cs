using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Interfaces
{
	/// <summary>
	/// Interface INotificationService
	/// </summary>
	public interface INotificationService
	{
		/// <summary>
		/// Notifies the specified message.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <param name="message">The message.</param>
		void Notify(string title, string message);

		/// <summary>
		/// Removes the notification.
		/// </summary>
		void RemoveNotification();
	}
}
