<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="AllNet.Modules.ReservasExportaciones.Settings" %>

<!-- uncomment the code below to start using the DNN Form pattern to create and update settings -->
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblKey" runat="server" />

        <asp:TextBox ID="txtKey" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label ID="lblCon" runat="server" />
        <asp:DropDownList ID="drpConnectionString" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label ID="lblRol" runat="server" />
        <asp:DropDownList ID="drpRolAgente" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label ID="lblNIT" runat="server" />
        <asp:TextBox ID="txtNIT" runat="server"></asp:TextBox>
    </div>
</fieldset>
