using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace uk.co.broadbandtap.homeseer
{
    /// <summary>
    /// </summary>
    [PluginActionId("uk.co.broadbandtap.homeseer.pluginaction")]
    public class HomeSeerAction : PluginBase
    {
        private class PluginSettings
        {

            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings
                {
                    HomeSeerServerIp = String.Empty,
                    DeviceName = String.Empty,
                    DeviceId = String.Empty,
                    OnValue = String.Empty,
                    OffValue = String.Empty
                };
                return instance;
            }

            [FilenameProperty]
            [JsonProperty(PropertyName = "homeSeerServerIp")]
            public string HomeSeerServerIp { get; set; }

            [JsonProperty(PropertyName = "deviceName")]
            public string DeviceName { get; set; }

            [JsonProperty(PropertyName = "deviceId")]
            public string DeviceId { get; set; }

            [JsonProperty(PropertyName = "onValue")]
            public string OnValue { get; set; }

            [JsonProperty(PropertyName = "OffValue")]
            public string OffValue { get; set; }
        }

        #region Private Members
        private const string ADMIN_IMAGE_FILE = @"images\shield.png";
        private Image prefetchedAdminImage = null;
        private PluginSettings settings;
        private bool currentStatus;
        static readonly HttpClient client = new HttpClient();

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="payload"></param>
        public HomeSeerAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                this.settings = PluginSettings.CreateDefaultSettings();
            }
            else
            {
                this.settings = payload.Settings.ToObject<PluginSettings>();
            }
            OnTick();
        }

        /// <summary>
        /// </summary>
        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Destructor called");
        }

        /// <summary>
        /// </summary>
        /// <param name="payload"></param>
        public async override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Key Pressed");
            try
            {
                HttpResponseMessage response = null;
                if (currentStatus)
                {
                    response = await client.GetAsync("http://" + this.settings.HomeSeerServerIp + "/JSON?request=controldevicebyvalue&ref=" + this.settings.DeviceId + "&value=" + this.settings.OffValue);
                }
                else
                {
                    response = await client.GetAsync("http://" + this.settings.HomeSeerServerIp + "/JSON?request=controldevicebyvalue&ref=" + this.settings.DeviceId + "&value=" + this.settings.OnValue);
                }
                
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                var homeseerStatus = JsonConvert.DeserializeObject<HomeseerStatus>(responseBody);
                if ((int)homeseerStatus.Devices[0].value == int.Parse(settings.OffValue))
                {
                    await Connection.SetTitleAsync($"" + settings.DeviceName);
                    await Connection.SetImageAsync(Image.FromFile(@"images\LightOff.png"), true);
                    currentStatus = false;
                }
                else
                {
                    await Connection.SetTitleAsync($"" + settings.DeviceName);
                    await Connection.SetImageAsync(Image.FromFile(@"images\LightOn.png"), true);
                    currentStatus = true;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            // Check if there are any running instances
        }

        /// <summary>
        /// </summary>
        /// <param name="payload"></param>
        public override void KeyReleased(KeyPayload payload) { }

        /// <summary>
        /// </summary>
        public async override void OnTick()
        {
            await HandleStatusIndicator();
        }

        /// <summary>
        /// </summary>
        /// <param name="payload"></param>
        public async override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            await Connection.SetTitleAsync((String)null);
            await SaveSettings();
        }

        /// <summary>
        /// </summary>
        /// <param name="payload"></param>
        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        #region Private Methods

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        private async Task HandleStatusIndicator()
        {

            try
            {
                HttpResponseMessage response = await client.GetAsync("http://" + this.settings.HomeSeerServerIp + "/JSON?request=getstatus&ref=" + this.settings.DeviceId );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                var homeseerStatus = JsonConvert.DeserializeObject<HomeseerStatus>(responseBody);
                if ((int)homeseerStatus.Devices[0].value == int.Parse(settings.OffValue))
                {
                    await Connection.SetTitleAsync($"" + settings.DeviceName);
                    await Connection.SetImageAsync(Image.FromFile(@"images\LightOff.png"), true);
                    currentStatus = false;
                }
                else
                {
                    await Connection.SetTitleAsync($"" + settings.DeviceName);
                    await Connection.SetImageAsync(Image.FromFile(@"images\LightOn.png"), true);
                    currentStatus = true;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            // Check if there are any running instances
        }

        private Image GetAdminImage()
        {
            if (prefetchedAdminImage == null)
            {
                if (File.Exists(ADMIN_IMAGE_FILE))
                {
                    prefetchedAdminImage = Image.FromFile(ADMIN_IMAGE_FILE);
                }
            }

            return prefetchedAdminImage;
        }

        #endregion
    }
}