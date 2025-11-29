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
            {"ItdosenotWorks", "ItdosenotWorks" },
            {"Coordinator", "Coordinator" },
        

        };
    }

    public enum DeviceStatusEnum
    {
        ItWorksWell,
        DeviceNeedsMaintenance,
        DeviceNeedsCalibration,
        ItdosenotWorks,
        Coordinator,
    }

    public enum ByTypeEnum
    {
        Clinic,
        Hospital
    }
}