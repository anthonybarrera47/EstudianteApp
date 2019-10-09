<%@ Page Title="Consulta de Estudiantes"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="ConsultaEstudiantes.aspx.cs" Inherits="EstudianteApp.Consultas.ConsultaEstudiantes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid">
        <div class="card text-center bg-light mb-3">
            <div class="card-header"><%:Page.Title %></div>
            <div class="card-body">
                <div class="form-horizontal col-md-12" role="form">
                    <div>
                        <div class="row justify-content-between">
                            <div class="input-group mb-3">
                                <div class="input-group-prepend">
                                    <span class="input-group-text" id="FiltroDropDownList">Filtro </span>
                                </div>
                                <div class="col-md-4 col-lg-4 col-xl-4 col-sm-4">
                                    <asp:DropDownList ID="BuscarPorDropDownList" runat="server" CssClass="form-control ">
                                        <asp:ListItem>Todos</asp:ListItem>
                                        <asp:ListItem>EstudianteId</asp:ListItem>
                                        <asp:ListItem>Nombre</asp:ListItem>
                                        <asp:ListItem>Apellido</asp:ListItem>
                                        <asp:ListItem>Puntos Perdidos</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="input-group-prepend">
                                    <span class="input-group-text" id="CriterioLB">Criterio </span>
                                </div>
                                <asp:TextBox ID="FiltroTextBox" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                                <div class="input-group-append" aria-describedby="FiltroTextBox">
                                    <asp:LinkButton ID="BuscarButton" runat="server" CssClass="btn btn-success input-sm" OnClick="BuscarButton_Click"><i class="fa fa-search"></i></asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%--CheckBox--%>
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <asp:CheckBox AutoPostBack="true" Checked="true" OnCheckedChanged="FechaCheckBox_CheckedChanged" ID="FechaCheckBox" runat="server" Text="Filtrar por fecha" />
                        </div>
                    </div>
                    <%--FechaDesde--%>
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="FechaDesde">Desde </span>
                        </div>
                        <div class="input-group-append" aria-describedby="FechaDesdeTextBox">
                            <asp:TextBox ID="FechaDesdeTextBox" TextMode="Date" runat="server" class="form-control input-sm" Visible="true"></asp:TextBox>
                        </div>
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="FechaHasta">Hasta </span>
                        </div>
                        <div class="input-group-append" aria-describedby="FechaHastaTextBox">
                            <asp:TextBox ID="FechaHastaTextBox" TextMode="Date" runat="server" class="form-control input-sm" Visible="true"></asp:TextBox>
                        </div>
                    </div>
                    <%--GRID--%>
                    <asp:ScriptManager runat="server" ID="ScriptManager"></asp:ScriptManager>
                    <div class="row justify-content-center">
                        <asp:UpdatePanel ID="UpdatePanel" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="DatosGridView"
                                    runat="server" AutoGenerateColumns="false" HtmlEncode="False"
                                    CssClass="table table-condensed table-hover table-responsive-md table-responsive-sm table-responsive-lg table-responsive-m"
                                    CellPadding="4" ForeColor="#333333" GridLines="None"
                                    AllowPaging="true" PageSize="6"
                                    OnPageIndexChanging="DatosGridView_PageIndexChanging">

                                    <AlternatingRowStyle BackColor="LightBlue" />
                                    <Columns>
                                        <asp:HyperLinkField ControlStyle-CssClass="btn btn-info"
                                            DataNavigateUrlFields="EstudianteId"
                                            DataNavigateUrlFormatString="~/Registros/RegistroEstudiante.aspx?EstudianteId={0}"
                                            Text="Editar"></asp:HyperLinkField>
                                        <asp:BoundField HeaderText="EstudianteId" DataField="EstudianteId" Visible="false" />
                                        <asp:BoundField HeaderText="Nombre" DataField="Nombre" />
                                        <asp:BoundField HeaderText="Apellido" DataField="Apellido" />
                                        <asp:BoundField HeaderText="NombreCompleto" DataField="NombreCompleto" Visible="false" />
                                        <asp:BoundField HeaderText="Puntos Perdidos" DataField="PuntosPerdidos" />
                                        <asp:BoundField HeaderText="Fecha Registro" DataField="Fecha" DataFormatString="{0:dd-MM-yyyy}" />
                                    </Columns>

                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <RowStyle BackColor="#EFF3FB" />
                                </asp:GridView>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="DatosGridView" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
        
    </div>

</asp:Content>
