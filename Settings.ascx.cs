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

using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections;
using System.Configuration;
using System.Web.Configuration;
using System.Web.UI.WebControls;

namespace AllNet.Modules.ReservasExportaciones
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Settings class manages Module Settings
    /// 
    /// Typically your settings control would be used to manage settings for your module.
    /// There are two types of settings, ModuleSettings, and TabModuleSettings.
    /// 
    /// ModuleSettings apply to all "copies" of a module on a site, no matter which page the module is on. 
    /// 
    /// TabModuleSettings apply only to the current module on the current page, if you copy that module to
    /// another page the settings are not transferred.
    /// 
    /// If you happen to save both TabModuleSettings and ModuleSettings, TabModuleSettings overrides ModuleSettings.
    /// 
    /// Below we have some examples of how to access these settings but you will need to uncomment to use.
    /// 
    /// Because the control inherits from ReservasExportacionesSettingsBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Settings : ReservasExportacionesModuleSettingsBase
    {
        #region Base Method Implementations

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            try
            {
                if (Page.IsPostBack == false)
                {
                    LoadConnectionsString(ref drpConnectionString);
                    LoadRolesDnn(ref drpRolAgente);
                    if (Settings.Contains("SecretKey"))
                        txtKey.Text = Settings["SecretKey"].ToString();
                    if (Settings.Contains("ConnectionString"))
                    {
                        drpConnectionString.SelectedValue = Settings["ConnectionString"].ToString();
                    }
                    if (Settings.Contains("RolAgente"))
                    {
                        drpRolAgente.SelectedValue = Settings["RolAgente"].ToString();
                    }
                    //if (Settings.Contains("txtNIT"))
                    //    txtNIT.Text = Settings["txtNIT"].ToString();

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateSettings saves the modified settings to the Database
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void UpdateSettings()
        {
            try
            {
                var modules = new ModuleController();

                //the following are two sample Module Settings, using the text boxes that are commented out in the ASCX file.
                //module settings
                modules.UpdateModuleSetting(ModuleId, "SecretKey", txtKey.Text);
                modules.UpdateModuleSetting(ModuleId, "ConnectionString", drpConnectionString.SelectedValue);
                modules.UpdateModuleSetting(ModuleId, "RolAgente", drpRolAgente.SelectedValue);
                //modules.UpdateModuleSetting(ModuleId, "SetConnection", txtNIT.Text);
                //modules.UpdateTabModuleSetting(TabModuleId, "SetConnection", txtNIT.Text);

                //tab module settings
                //modules.UpdateTabModuleSetting(TabModuleId, "Setting1",  txtSetting1.Text);
                //modules.UpdateTabModuleSetting(TabModuleId, "Setting2",  txtSetting2.Text);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        public void LoadRolesDnn(ref DropDownList lista)
        {
            ListItem lid = new ListItem();
            lid.Value = "";
            lid.Text = "Seleccione";
            lista.Items.Add(lid);
            var roles = RoleController.Instance.GetRoles(PortalId);

            foreach (var rol in roles)
            {
                ListItem li = new ListItem();
                li.Value = rol.RoleName;
                li.Text = rol.RoleName;
                lista.Items.Add(li);
            }
        }
        public void LoadConnectionsString(ref DropDownList lista)
        {
            ListItem lid = new ListItem();
            lid.Value = "";
            lid.Text = "Seleccione";
            lista.Items.Add(lid);
            ConnectionStringsSection connectionStringsSection = WebConfigurationManager.GetSection("connectionStrings") as ConnectionStringsSection;
            ConnectionStringSettingsCollection connectionStrings = connectionStringsSection.ConnectionStrings;
            IEnumerator connectionStringsEnum = connectionStrings.GetEnumerator();
            int i = 0;
            while (connectionStringsEnum.MoveNext())
            {
                ListItem li = new ListItem();
                li.Value = connectionStrings[i].Name;
                li.Text = connectionStrings[i].Name;
                lista.Items.Add(li);
                i += 1;
            }
        }
        #endregion
    }
}