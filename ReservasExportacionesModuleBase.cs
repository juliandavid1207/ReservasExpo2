/*
' Copyright (c) 2023  allnet.com.co
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using AllNet.Modules.ReservasExportaciones.Components;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;

namespace AllNet.Modules.ReservasExportaciones
{
    public class ReservasExportacionesModuleBase : PortalModuleBase
    {
        private string SecretKey
        {
            get
            {
                if (Settings.Contains("SecretKey"))
                    return Settings["SecretKey"].ToString();
                return "123";
            }
        }
        public string PublicKey
        {
            get
            {
                if (Settings.Contains("SecretKey"))
                    return RC4.Encrypt("123", Settings["SecretKey"].ToString());
                return "123";
            }
        }
        public string ConnectionString
        {
            get
            {
                if (Settings.Contains("ConnectionString"))
                    return RC4.Encrypt(SecretKey, Settings["ConnectionString"].ToString());
                return RC4.Encrypt(SecretKey, "BD_AGP");
            }
        }
        public string NitUser
        {
            get
            {
                return IsUserHost ? "" : UserController.Instance.GetCurrentUserInfo().Profile.GetPropertyValue("NIT").ToString();
            }
        }
        public bool IsAgente
        {
            get
            {
                if (Settings.Contains("RolAgente"))
                    return UserController.Instance.GetCurrentUserInfo().IsInRole(Settings["RolAgente"].ToString());
                return UserController.Instance.GetCurrentUserInfo().IsInRole("AgenteAGP");
            }
        }
        public bool IsUserHost
        {
            get
            {
                return UserController.Instance.GetCurrentUserInfo().IsSuperUser;
            }
        }
    }
}