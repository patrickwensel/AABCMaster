using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AABC.Mobile.AppServices.Interfaces;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AABC.Mobile.Droid.Services
{
	/// <summary>
	/// Class NotificationService.
	/// </summary>
	/// <seealso cref="AABC.Mobile.AppServices.Interfaces.INotificationService" />
	class NotificationService : INotificationService
	{
		/// <summary>
		/// The context
		/// </summary>
		readonly Context _context;

		/// <summary>
		/// The notification identifier
		/// </summary>
		const int NotificationId = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationService"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		public NotificationService(Context context)
		{
			_context = context;
		}

		/// <summary>
		/// Notifies the specified message.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <param name="message">The message.</param>
		public void Notify(string title, string message)
		{
			// Set up an intent so that tapping the notifications returns to this app:
			Intent intent = new Intent(_context, typeof(MainActivity));

			// Create a PendingIntent; we're only using one PendingIntent (ID = 0):
			const int pendingIntentId = 0;
			PendingIntent pendingIntent = PendingIntent.GetActivity(_context, pendingIntentId, intent, PendingIntentFlags.OneShot);

			// Instantiate the notification builder and enable sound:
			var builder = new Notification.Builder(_context)
				.SetContentIntent(pendingIntent)
				.SetContentTitle(title)
				.SetContentText(message)
				.SetOngoing(true)
				.SetSmallIcon(Resource.Drawable.ic_notification);

			// Build the notification:
			var notification = builder.Build();

			// Get the notification manager:
			var notificationManager = _context.GetSystemService(Context.NotificationService) as NotificationManager;

			// Publish the notification:
			notificationManager?.Notify(NotificationId, notification);
		}

		/// <summary>
		/// Removes the notification.
		/// </summary>
		public void RemoveNotification()
		{
			// Get the notification manager:
			var notificationManager = _context.GetSystemService(Context.NotificationService) as NotificationManager;
			notificationManager?.Cancel(NotificationId);
		}
	}
}