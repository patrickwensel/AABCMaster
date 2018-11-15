using AABC.Mobile.SharedEntities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.SharedEntities.Messages
{
	public class ValidationRequest
	{
		public ValidatedSession RequestedValidatedSession { get; set; }
	}
}
