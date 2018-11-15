using AABC.Domain2.Insurances;
using AABC.Domain2.Providers;
using AABC.Domain2.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AABC.DomainServices.Services.Tests.Unit
{
    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.DomainServices.Services")]
    public class ServiceProviderTests
    {

        private List<Service> _serviceList = new List<Service>();
        private List<ProviderTypeService> _providerTypeServiceList = new List<ProviderTypeService>();
        private List<Domain2.Insurances.Insurance> _insurances = new List<Domain2.Insurances.Insurance>();

        private Mock<Data.V2.CoreContext> _dbMock = new Mock<Data.V2.CoreContext>();
        private ServiceProvider _serviceProvider;

        [TestInitialize]
        public void Initialize() {
            
            _serviceList.Add(new Service() { ID = 1, Code = "A", Type = ServiceTypes.Assessment });
            _serviceList.Add(new Service() { ID = 2, Code = "B", Type = ServiceTypes.Assessment });
            _serviceList.Add(new Service() { ID = 3, Code = "C", Type = ServiceTypes.Care });
            _serviceList.Add(new Service() { ID = 4, Code = "D", Type = ServiceTypes.Care});
            _serviceList.Add(new Service() { ID = 5, Code = "E", Type = ServiceTypes.General});

            _providerTypeServiceList.Add(new ProviderTypeService() { ProviderTypeID = 15, Service = new Service() { ID = 1 } });
            _providerTypeServiceList.Add(new ProviderTypeService() { ProviderTypeID = 15, Service = new Service() { ID = 2 } });
            _providerTypeServiceList.Add(new ProviderTypeService() { ProviderTypeID = 17, Service = new Service() { ID = 3 } });
            _providerTypeServiceList.Add(new ProviderTypeService() { ProviderTypeID = 17, Service = new Service() { ID = 4 } });
            _providerTypeServiceList.Add(new ProviderTypeService() { ProviderTypeID = 19, Service = new Service() { ID = 5 } });

            var insuranceA = new Domain2.Insurances.Insurance();
            var insuranceB = new Domain2.Insurances.Insurance();

            insuranceA.Services.Add(new InsuranceService() {
                Service = _serviceList.Where(x => x.ID == 1).Single()
            });
            insuranceA.Services.Add(new InsuranceService() {
                Service = _serviceList.Where(x => x.ID == 2).Single(),
                EffectiveDate = new DateTime(2000, 1, 1),
                DefectiveDate = new DateTime(2010, 1, 1)
            });
            insuranceA.Services.Add(new InsuranceService() {
                Service = _serviceList.Where(x => x.ID == 3).Single(),
                EffectiveDate = new DateTime(2010, 1, 1),
                DefectiveDate = new DateTime(2020, 1, 1)
            });

            insuranceB.Services.Add(new InsuranceService()
            {
                Service = _serviceList.Where(x => x.ID == 1).Single()
            });
            insuranceB.Services.Add(new InsuranceService()
            {
                Service = _serviceList.Where(x => x.ID == 2).Single(),
                EffectiveDate = new DateTime(2000, 1, 1),
                DefectiveDate = new DateTime(2010, 1, 1)
            });
            insuranceB.Services.Add(new InsuranceService()
            {
                Service = _serviceList.Where(x => x.ID == 3).Single(),
                EffectiveDate = new DateTime(2010, 1, 1),
                DefectiveDate = new DateTime(2020, 1, 1)
            });

            _insurances.Add(insuranceA);
            _insurances.Add(insuranceB);

            _dbMock.Object.Services = GetQueryableMockDbSet(_serviceList.ToArray());
            _dbMock.Object.ProviderTypeServices = GetQueryableMockDbSet(_providerTypeServiceList.ToArray());

            _serviceProvider = new ServiceProvider(_dbMock.Object);

        }


        private static DbSet<T> GetQueryableMockDbSet<T>(params T[] sourceList) where T : class {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            //    dbSet.Setup(x => x.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));

            return dbSet.Object;
        }




        //[TestMethod]
        //public void GetServicesByInsuranceReturnsCurrentWithinDate() {

        //    // insurance A, service 1 = no dates
        //    // insurance A, service 2 = 2000 to 2010
        //    // insurance A, service 3 = 2010 to 2020

        //}

        [TestMethod]
        public void GetLegacyServicesByProviderTypeReturnsCorrectly() {

            var sut = _serviceProvider.GetLegacyServicesByProviderType(Domain2.Providers.ProviderTypeIDs.BoardCertifiedBehavioralAnalyst);

            Assert.IsTrue(sut.Count() == 2);
            Assert.IsTrue(sut.Where(x => x.ID == 1).Count() == 1);
            Assert.IsTrue(sut.Where(x => x.ID == 2).Count() == 1);
        }

        [TestMethod]
        public void GetAllServicesReturnsAllServices() {
            
            var sut = _serviceProvider.GetAllServices();
            Assert.IsTrue(sut.Count == _serviceList.Count);
        }

        [TestMethod]
        public void GetServicesByTypeReturnsCorrectServices() {

            var sut = _serviceProvider.GetServicesByType(ServiceTypes.Assessment);

            Assert.IsTrue(sut.Count == _serviceList.Where(x => x.Type == ServiceTypes.Assessment).Count());
            Assert.IsTrue(sut.Where(x => x.Type == ServiceTypes.Assessment).Count() == _serviceList.Where(x => x.Type == ServiceTypes.Assessment).Count());           
        }
    }
}
