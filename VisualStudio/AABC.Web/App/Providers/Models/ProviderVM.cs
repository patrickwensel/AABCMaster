using AABC.Domain.General;
using AABC.Domain2.Providers;
using AABC.Web.App.Providers.Models;
using DevExpress.Web;
using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.Models.Providers
{
    public class ProviderVM : Provider
    {
        public IViewModelBase Base { get; set; }
        public WebViewHelper ViewHelper { get; private set; }
        public char? Gender { get; set; }
        public bool Active { get; set; }
        public ProviderTypeVM Type { get; set; }
        public IEnumerable<UploadedFile> ResumeFile { get; set; }
        public string ServiceZips { get; set; }

        public ProviderVM()
        {
            ViewHelper = new WebViewHelper(this);
        }

        public class WebViewHelper : IWebViewHelper
        {
            private readonly ProviderVM _parent;
            public string ReturnErrorMessage { get; set; }
            public bool HasValidationErrors { get; set; }
            public IEnumerable<State> StatesList { get; set; }
            public IEnumerable<ProviderTypesListItemVM> ProviderTypesList { get; set; }
            public IEnumerable<ProviderSubTypesListItemVM> ProviderSubTypesList { get; set; }
            public IDictionary<int, string> ActiveCasesDictionary { get; set; } = new Dictionary<int, string>();  // CaseID, PatientName
            public IDictionary<int, string> InactiveCasesDictionary { get; set; } = new Dictionary<int, string>();    // CaseID, PatientName
            public IEnumerable<KeyValuePair<decimal, DateTime>> RateHistory { get; set; }  // Rate, Effective Date


            public WebViewHelper(ProviderVM parent)
            {
                _parent = parent;
            }


            public IDictionary<int, string> AllCases
            {
                get
                {
                    var result = new Dictionary<int, string>();
                    foreach (var a in ActiveCasesDictionary)
                    {
                        result.Add(a.Key, a.Value);
                    }
                    foreach (var i in InactiveCasesDictionary)
                    {
                        result.Add(i.Key, i.Value);
                    }
                    return result;
                }
            }


            public void SetServiceZips()
            {
                if (_parent.ServiceZipCodes != null)
                {
                    var zipCodes = _parent.ServiceZipCodes.Select(x => x.ZipCode).ToList();
                    _parent.ServiceZips = string.Join(", ", zipCodes);
                }
            }


            public void BindModel() { }


            public bool Validate()
            {
                var validTypes = new string[] { ".pdf", ".txt", ".rtf", ".doc", ".docx" };
                var ret = !(string.IsNullOrEmpty(_parent.FirstName) || string.IsNullOrEmpty(_parent.LastName));
                if (_parent.ResumeFile.Any())
                {
                    var fileName = _parent.ResumeFile.First().FileName;
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        string extension = fileName.Substring(fileName.LastIndexOf('.')).ToLower();
                        if (!validTypes.Contains(extension))
                        {
                            _parent.ResumeFile.First().IsValid = false;
                            ret = false;
                        }
                    }
                }
                HasValidationErrors = !ret;
                return ret;
            }

        }

    }
}