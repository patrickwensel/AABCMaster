using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.Services
{
    public class CommonLists
    {


        static CommonLists _default = null;

        public static CommonLists Default {
            get
            {
                if (_default == null) {
                    _default = new CommonLists();
                }

                return _default;
            }
        }


        public List<Insurance2ListItem> Insurances2() {

            var insurances = AppService.Current.DataContextV2.Insurances.Where(x => x.Active).OrderBy(x => x.Name).ToList();

            var items = new List<Insurance2ListItem>();

            foreach (var ins in insurances) {
                items.Add(new Insurance2ListItem()
                {
                    ID = ins.ID,
                    Name = ins.Name
                });
            }

            return items;
        }
        
        [Obsolete("Use Insurances2 instead, this works off distincting non-managed insurance text fields")]
        public List<InsuranceListItem> Insurances () {

            var insurances = _data.Patients.Select(x => x.PatientInsuranceCompanyName).Distinct().ToList();

            var items = new List<InsuranceListItem>();

            foreach (var ins in insurances) {
                items.Add(new InsuranceListItem()
                {
                    Name = ins
                });
            }

            return items;
        }




        private Data.Models.CoreEntityModel _data;

        public CommonLists() {
            _data = new Data.Models.CoreEntityModel();
        }
        
        public class InsuranceListItem
        {
            public string Name { get; set; }
        }

        public class Insurance2ListItem
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

    }
}