using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace uk.co.broadbandtap.homeseer
{
    /// <summary>
    /// Homeseer Device Type class
    /// </summary>
    public class DeviceType
    {

        [JsonProperty("Device_API")]
        public int Device_API { get; set; }

        [JsonProperty("Device_API_Description")]
        public string Device_API_Description { get; set; }

        [JsonProperty("Device_Type")]
        public int Device_Type { get; set; }

        [JsonProperty("Device_Type_Description")]
        public string Device_Type_Description { get; set; }

        [JsonProperty("Device_SubType")]
        public int Device_SubType { get; set; }

        [JsonProperty("Device_SubType_Description")]
        public string Device_SubType_Description { get; set; }
    }

    /// <summary>
    /// Homeseer Device Status
    /// </summary>
    public class Device
    {
        [JsonProperty("ref")]
        public int deviceRef { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("location")]
        public string location { get; set; }

        [JsonProperty("location2")]
        public string location2 { get; set; }

        [JsonProperty("value")]
        public double value { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("device_type_string")]
        public string device_type_string { get; set; }

        [JsonProperty("last_change")]
        public DateTime last_change { get; set; }

        [JsonProperty("relationship")]
        public int relationship { get; set; }

        [JsonProperty("hide_from_view")]
        public bool hide_from_view { get; set; }

        [JsonProperty("associated_devices")]
        public IList<int> associated_devices { get; set; }

        [JsonProperty("device_type")]
        public DeviceType device_type { get; set; }

        [JsonProperty("device_type_values")]
        public object device_type_values { get; set; }

        [JsonProperty("UserNote")]
        public string UserNote { get; set; }

        [JsonProperty("UserAccess")]
        public string UserAccess { get; set; }

        [JsonProperty("status_image")]
        public string status_image { get; set; }

        [JsonProperty("voice_command")]
        public string voice_command { get; set; }

        [JsonProperty("misc")]
        public int misc { get; set; }

        [JsonProperty("interface_name")]
        public string interface_name { get; set; }
    }

    // ReSharper disable once IdentifierTypo
    /// <summary>
    /// base class of Homeseer Status
    /// </summary>
    public class HomeseerStatus
    {

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Version")]
        public string Version { get; set; }

        [JsonProperty("Devices")]
        public IList<Device> Devices { get; set; }
    }


}