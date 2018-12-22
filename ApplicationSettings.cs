using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using ItpMicroController.Entities;

namespace ItpMicroController
{
    public class ApplicationSettings
    {
        public static string GetSetttingValue(string key)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var configValue = config.AppSettings.Settings[key];
            if (configValue== null)
                return null;

            return configValue.Value;
        }

        public static void SaveSetting(Configuration config, string key, string value)
        {
            var configValue = config.AppSettings.Settings[key];
            if (configValue == null)
                return;

            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
        }

        public static BindingList<Vehicle> GetVehicles()
        {
            var vehicles = DatabaseWrapper.GetVehicles();
            var sortedListInstance = new BindingList<Vehicle>(vehicles.OrderBy(x => x.Name).ToList());
            return sortedListInstance;
        }

        public static BindingList<Country> GetCountries()
        {
            var countries = DatabaseWrapper.GetCountries();
            var sortedListInstance = new BindingList<Country>(countries.OrderBy(x => x.Name).ToList());
            return sortedListInstance;
        }
    }
}
