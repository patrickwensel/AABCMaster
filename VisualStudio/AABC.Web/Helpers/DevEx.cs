using DevExpress.Web.Mvc;
using System;

namespace AABC.Web.Helpers
{
    public class DevEx
    {




        /// <summary>
        /// Sets the default combobox selection based on the specified property of the specified model object
        /// </summary>
        /// <param name="sender">Sender of the original event (MVCxComboBox)</param>
        /// <param name="modelObject">Object that will be set as the default</param>
        /// <param name="lookupPropertyName">Name of the property to use for finding in the combo's source list</param>
        public static void ComboPreRenderByField(object sender, object modelObject, string lookupPropertyName) {

            if (modelObject == null) {
                return;
            }

            var cb = sender as MVCxComboBox;
            var prop = modelObject.GetType().GetProperty(lookupPropertyName);
            
            if (prop == null) {
                throw new ArgumentException("lookupPropertyName not found on modelObject");
            }

            cb.SelectedItem = cb.Items.FindByValue(prop.GetValue(modelObject));
            
        }


        public static void ComboPreRenderByValue(object sender, object value) {
            var cb = sender as MVCxComboBox;
            cb.SelectedItem = cb.Items.FindByValue(value);
        }

        /// <summary>
        /// Sets the default combobox selection based on the assumed property "ID" of the specified modelObject
        /// </summary>
        /// <param name="sender">Sender of the original event (MVCxComboBox)</param>
        /// <param name="modelObject">Object to which the ID field is found</param>
        public static void ComboPreRender(object sender, object modelObject) {
            ComboPreRenderByField(sender, modelObject, "ID");
        }
        

    }
}