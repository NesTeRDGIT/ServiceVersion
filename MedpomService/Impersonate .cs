    using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace MedpomService
{

    /*
    public class Impersonation
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        // Test harness.
        // If you incorporate this code into a DLL, be sure to demand FullTrust.
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public static bool SetUser(string domainName, string userName, string _passWord)
        {
            SafeTokenHandle safeTokenHandle;
          
                const int LOGON32_PROVIDER_DEFAULT = 0;
                //This parameter causes LogonUser to create a primary token.
                const int LOGON32_LOGON_INTERACTIVE = 2;

                // Call LogonUser to obtain a handle to an access token.
                bool returnValue = LogonUser(userName, domainName, _passWord,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    out safeTokenHandle);
                

                //Console.WriteLine("LogonUser called.");

                if (false == returnValue)
                {
                    return false;
                }
                else
                {
                    WindowsImpersonationContext impersonatedUser = WindowsIdentity.Impersonate(safeTokenHandle.DangerousGetHandle());
                    return true;
                }
                          

        }
        public static bool CheckUser(string domainName, string userName, string _passWord)
        {
            SafeTokenHandle safeTokenHandle;            
                
                // Console.Write("Enter the password for {0}: ", userName);

                const int LOGON32_PROVIDER_DEFAULT = 0;
                //This parameter causes LogonUser to create a primary token.
                const int LOGON32_LOGON_INTERACTIVE = 2;

                // Call LogonUser to obtain a handle to an access token.
                bool returnValue = LogonUser(userName, domainName, _passWord,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    out safeTokenHandle);


                //Console.WriteLine("LogonUser called.");

                if (false == returnValue)
                {
                    return false;
                }
                else
                {
                    return true;
                }      
        }

        public static SafeTokenHandle GetToken(string domainName, string userName, string passWord)
        {
            SafeTokenHandle safeTokenHandle;
            const int LOGON32_PROVIDER_DEFAULT = 0;
            //This parameter causes LogonUser to create a primary token.
            const int LOGON32_LOGON_INTERACTIVE = 2;

            bool returnValue = LogonUser(userName, domainName, passWord,
                  LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                  out safeTokenHandle);
            if (returnValue)
                return safeTokenHandle;
            else
                return null;
        }
    }
    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeTokenHandle()
            : base(true)
        {
        }

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }
    */

}
