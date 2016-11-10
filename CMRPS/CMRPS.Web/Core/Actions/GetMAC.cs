using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using CMRPS.Web.Models;

namespace CMRPS.Web.Core
{
    public partial class Actions
    {
        /// <summary>
        /// HangFire | Gets the MAC address.
        /// </summary>
        /// <param name="computer"></param>
        /// <returns></returns>
        public static string GetMAC(ComputerModel computer)
        {
            try
            {
                // Parse IP
                IPAddress address = IPAddress.Parse(computer.IP);
                // Get INT
                int intAddress = BitConverter.ToInt32(address.GetAddressBytes(), 0);

                byte[] macAddr = new byte[6];
                int macAddrLen = (int)macAddr.Length;

                if (SendARP(intAddress, 0, macAddr, ref macAddrLen) != 0)
                    return computer.MAC;

                StringBuilder macAddressString = new StringBuilder();
                for (int i = 0; i < macAddr.Length; i++)
                {
                    if (macAddressString.Length > 0)
                        macAddressString.Append(":");

                    macAddressString.AppendFormat("{0:x2}", macAddr[i]);
                }
                return macAddressString.ToString().ToUpper();
            }
            catch (Exception)
            {
                return computer.MAC;
            }
        }
    }
}