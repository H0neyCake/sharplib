using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Security.Principal;
using System.DirectoryServices;

namespace sharplib
{
    public class DllEntry
    {
        //[DllExport("RVExtensionVersion", CallingConvention = CallingConvention.Winapi)]

        [DllExport("_RVExtensionVersion@8", CallingConvention = CallingConvention.Winapi)]
        public static void RvExtensionVersion(StringBuilder output, int outputSize)
        {
            output.Append("Cake v1.0");
        }

        //[DllExport("RVExtension", CallingConvention = CallingConvention.Winapi)]

        [DllExport("_RVExtension@12", CallingConvention = CallingConvention.Winapi)]
        public static void RvExtension(StringBuilder output, int outputSize,
           [MarshalAs(UnmanagedType.LPStr)] string function)
        {
            switch (function)
            {
                case "hello":
                    output.Append("Hola, Debugger vol.2");
                    break;
                case "GetCPU_id":
                    output.Append(GetCPUID());
                    break;
                case "GetCPU_name":
                    output.Append(GetCPUName());
                    break;
                case "GetMother_id":
                    output.Append(MotherboardInfo.SerialNumber);
                    break;
                case "GetMother_name":
                    output.Append(MotherboardInfo.Name);
                    break;
                case "GetHDD_Ids":
                    output.Append(GetHDDId());
                    break;
                case "GetHDD_sizes":
                    output.Append(GetHDDSize());
                    break;
                case "GetHDD_names":
                    output.Append(GetHDDNames());
                    break;
                case "GetRam_serialNumber":
                    output.Append(GetRAMSerialNumber());
                    break;
                case "GetRam_PartNumber":
                    output.Append(GetRAMPartNumber());
                    break;
                case "GetRam_name":
                    output.Append(GetRAMPartName());
                    break;
                case "GetRam_capacity":
                    output.Append(GetRAMPartCapacity());
                    break;
                case "GetMac_address":
                    output.Append(GetMacAddress());
                    break;
                case "GetProduct_id":
                    output.Append(GetProductID());
                    break;
                case "GetVRAM_name":
                    output.Append(GetVRAMName());
                    break;
                case "GetBios_id":
                    output.Append(GetBiosSerialNumber());
                    break;
                case "GetBios_ReleaseDate":
                    output.Append(GetBiosReleaseDate());
                    break;
                case "GetBios_Version":
                    output.Append(GetBiosVersion());
                    break;
                case "GetPC_name":
                    output.Append(GetMachineName());
                    break;
                case "Get_SID":
                    output.Append(GetComputerSid());
                    break;
                case "Get_HWID_Hash":
                    output.Append(CreateMD5(GetCPUID() + MotherboardInfo.SerialNumber + GetHDDId()));
                    break;
                case "Get_SWID_Hash":
                    output.Append(CreateMD5(GetProductID() + GetMacAddress() + GetBiosSerialNumber()));
                    break;
            }
        }
        //[DllExport("RVExtensionArgs", CallingConvention = CallingConvention.Winapi)]

        [DllExport("_RVExtensionArgs@20", CallingConvention = CallingConvention.Winapi)]
        public static int RvExtensionArgs(StringBuilder output, int outputSize,
           [MarshalAs(UnmanagedType.LPStr)] string function,
           [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 4)] string[] args, int argCount)
        {
            string ss = string.Empty;
            if (function == "toMD5")
            {
                string fnc = args[0];
                output.Append(CreateMD5(fnc));
            }
            else if (function == "toSHA256")
            {
                string fnc = args[0];
                output.Append(SHA256(fnc));
            }
            else if (function == "getHash")
            {
                string fnc = args[0];
                string fnc2 = args[1];
                string fnc3 = args[2];

                output.Append(SHA256(fnc + fnc2 + fnc3));
            }
            return 0;
        }
        public static SecurityIdentifier GetComputerSid()
        {
            return new SecurityIdentifier((byte[])new DirectoryEntry(string.Format("WinNT://{0},Computer", Environment.MachineName)).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid;
        }
        private static string GetMachineName()
        {
            return Environment.MachineName;
        }
        public static string GetBiosSerialNumber()
        {
            return identifier("Win32_BIOS", "SerialNumber");
        }
        public static string GetBiosReleaseDate()
        {
            return identifier("Win32_BIOS", "ReleaseDate");
        }
        public static string GetBiosVersion()
        {
            return identifier("Win32_BIOS", "Version");
        }
        public static string GetVRAMName()
        {
            return identifier("Win32_VideoController", "Name");
        }
        public static string GetRAMSerialNumber()
        {
            return identifier("Win32_PhysicalMemory", "SerialNumber");
        }
        public static string GetRAMPartNumber()
        {
            return identifier("Win32_PhysicalMemory", "PartNumber");
        }
        public static string GetRAMPartCapacity()
        {
            return identifier("Win32_PhysicalMemory", "Capacity");
        }
        public static string GetRAMPartName()
        {
            return identifier("Win32_PhysicalMemory", "Name");
        }
        public static string GetHDDId()
        {
            string hddInfo = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            List<string> hddlist = new List<string>();
            foreach (ManagementObject info in searcher.Get())
            {
                hddlist.Add(info["SerialNumber"].ToString());
            }
            string[] hdds = hddlist.ToArray();
            string ss = "[";
            ss += string.Join(", ", hdds);
            ss += "]";
            return ss;
        }
        public static string GetHDDSize()
        {
            string hddInfo = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            List<string> hddlist = new List<string>();
            foreach (ManagementObject info in searcher.Get())
            {
                hddlist.Add(info["Size"].ToString());
            }
            string[] hdds = hddlist.ToArray();
            string ss = "[";
            ss += string.Join(", ", hdds);
            ss += "]";
            return ss;
        }
        public static string GetHDDNames()
        {
            string hddInfo = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            List<string> hddlist = new List<string>();
            foreach (ManagementObject info in searcher.Get())
            {
                hddlist.Add(info["Model"].ToString());
            }
            string[] hdds = hddlist.ToArray();
            string ss = "[";
            ss += string.Join(", ", hdds);
            ss += "]";
            return ss;
        }
        public static string GetProductID()
        {
            RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey windowsNTKey = localMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion");
            object productID = windowsNTKey.GetValue("ProductId");
            return productID.ToString();
        }
        public static string GetCPUID()
        {            
            return identifier("Win32_Processor", "ProcessorId");
        }
        public static string GetCPUName()
        {
            return identifier("Win32_Processor", "Name");
        }

        public static string GetMacAddress()
        {
            String firstMacAddress = NetworkInterface
            .GetAllNetworkInterfaces()
            .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Select(nic => nic.GetPhysicalAddress().ToString())
            .FirstOrDefault();
            return firstMacAddress;
        }

        #region Service
        private static string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            ManagementClass mc = new ManagementClass(wmiClass);
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }
        public static string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        static string SHA256(string input)
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(input));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }
        #endregion

    }
    static public class MotherboardInfo
    {
        private static ManagementObjectSearcher baseboardSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
        private static ManagementObjectSearcher motherboardSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_MotherboardDevice");

        static public string Name
        {
            get
            {
                try
                {
                    foreach (ManagementObject queryObj in baseboardSearcher.Get())
                    {
                        return queryObj["Product"].ToString();
                    }
                    return "";
                }
                catch (Exception e)
                {
                    return "";
                }
            }
        }
        static public string SerialNumber
        {
            get
            {
                try
                {
                    foreach (ManagementObject queryObj in baseboardSearcher.Get())
                    {
                        return queryObj["SerialNumber"].ToString();
                    }
                    return "";
                }
                catch (Exception e)
                {
                    return "";
                }
            }
        }
    }
}
