using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;

namespace DFSResolver.APIs
{
    [ServiceContract]
    public class DfsResolverApi
    {
        [WebGet(UriTemplate = "{path}")]
        public IEnumerable<String>  Get(String path)
        {
            Trace.WriteLine(path);
            return GetActiveServersPKI("\\\\ad.ucl.ac.uk\\slms\\home1");
        }

        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int NetDfsGetInfo
            (
            [MarshalAs(UnmanagedType.LPWStr)] string EntryPath,
            [MarshalAs(UnmanagedType.LPWStr)] string ServerName,
            [MarshalAs(UnmanagedType.LPWStr)] string ShareName,
            int Level,
            ref IntPtr Buffer
            );

        public struct DFS_INFO_3
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string EntryPath;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Comment;
            public UInt32 State;
            public UInt32 NumberOfStorages;
            public IntPtr Storages;
        }

        public struct DFS_STORAGE_INFO
        {
            public Int32 State;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string ServerName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string ShareName;
        }

        public static IList<String> GetActiveServersPKI(string sDFSPath)
        {
            IList<String> sServers = new List<String>();
            IntPtr pBuffer = new IntPtr();
            int iResult = NetDfsGetInfo(sDFSPath, null, null, 3, ref pBuffer);
            if (iResult == 0)
            {

                DFS_INFO_3 oDFSInfo = (DFS_INFO_3)Marshal.PtrToStructure(pBuffer, typeof(DFS_INFO_3));
                for (int i = 0; i < oDFSInfo.NumberOfStorages; i++)
                {
                    IntPtr pStorage = new IntPtr(oDFSInfo.Storages.ToInt64() + i * Marshal.SizeOf(typeof(DFS_STORAGE_INFO)));
                    DFS_STORAGE_INFO oStorageInfo = (DFS_STORAGE_INFO)Marshal.PtrToStructure(pStorage, typeof(DFS_STORAGE_INFO));

                    //Get Only Active Hosts
                    if (oStorageInfo.State == 2)
                    {
                        sServers.Add(oStorageInfo.ServerName);
                    }
                }

            }
            return sServers;
        }
    }
}