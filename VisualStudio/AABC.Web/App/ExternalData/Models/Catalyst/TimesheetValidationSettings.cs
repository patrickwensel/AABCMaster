﻿namespace AABC.Web.Models.ExternalData.Catalyst
{
    public class TimesheetValidationSettings
    {

        public static DevExpress.Web.UploadControlValidationSettings Settings = new DevExpress.Web.UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".xlsx" },
            MaxFileSize = 4194304
        };

    }
}