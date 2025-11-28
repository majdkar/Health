using System.Collections.Generic;

namespace SchoolV01.Shared.Constants.Device
{
    public static class DeviceStatus
    {
        public static Dictionary<string, string> Values = new Dictionary<string, string>
        {
            {"ItWorksWell", "ItWorksWell" },
            {"DeviceNeedsMaintenance", "DeviceNeedsMaintenance" },
            {"DeviceNeedsCalibration", "DeviceNeedsCalibration" },
        

        };
    }

    public enum DeviceStatusEnum
    {
        ItWorksWell,
        DeviceNeedsMaintenance,
        DeviceNeedsCalibration,
    }

    public enum ByTypeEnum
    {
        Clinic,
        Hospital
    }
}