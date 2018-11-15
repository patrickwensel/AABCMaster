using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.Api.Mappings.Tests.Unit
{
    [TestClass]
    [TestCategory("AABC.Mobile.Api.Mappings")]
    public class MappingsTests
    {

        [TestMethod]
        public void CaseMappingsMapsFromDomain() {

            var domain = new Domain2.Cases.Case();
            var mapper = new CaseMapping();

            domain.ID = 101;
            
            domain.Patient = new Domain2.Patients.Patient();
            domain.Patient.FirstName = "John";
            domain.Patient.LastName = "Doe";

            var sut = mapper.FromDomain(domain);

            Assert.IsNotNull(sut);
            Assert.IsNotNull(sut.Patient);
            Assert.IsNotNull(sut.ActiveAuthorizations);
            // insurance can be null
            Assert.IsInstanceOfType(sut, typeof(SharedEntities.Entities.Case));
            Assert.AreEqual(101, sut.ID);
            Assert.AreEqual("John", sut.Patient.PatientFirstName);
            Assert.AreEqual("Doe", sut.Patient.PatientLastName);            
        }

    }
}
