using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Entities
{
	[Table("CaseValidationSignatures")]
	public class CaseValidationSignature
	{
		[PrimaryKey]
		[AutoIncrement]
		public int CaseValidationSignatureID { get; set; }

		public int CaseValidationID { get; set; }

		public SignatureType SignatureType { get; set; }

		public byte[] Signature { get; set; }
	}
}
