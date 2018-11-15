using AABC.Mobile.AppServices.Entities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Events
{
	public class UserChangedEvent : PubSubEvent<User>
	{
	}
}
