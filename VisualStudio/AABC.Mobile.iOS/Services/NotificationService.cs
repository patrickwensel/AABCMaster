using System;
using System.Collections.Generic;
using System.Text;
using AABC.Mobile.AppServices.Interfaces;

namespace AABC.Mobile.iOS.Services
{
	/// <summary>
	/// Class NotificationService.
	/// </summary>
	/// <seealso cref="AABC.Mobile.AppServices.Interfaces.INotificationService" />
	class NotificationService : INotificationService
	{
		/// <summary>
		/// Notifies the specified message.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <param name="message">The message.</param>
		public void Notify(string title, string message)
		{
			// do nothing
		}

		/// <summary>
		/// Removes the notification.
		/// </summary>
		public void RemoveNotification()
		{
			// do nothing
		}
	}
}
