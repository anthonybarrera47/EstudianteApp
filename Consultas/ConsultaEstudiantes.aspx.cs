﻿using BLL;
using Entidades;
using Extensores;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EstudianteApp.Consultas
{
    public partial class ConsultaEstudiantes : System.Web.UI.Page
    {
        static List<Estudiantes> Lista = new List<Estudiantes>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FechaDesdeTextBox.Text = DateTime.Now.ToFormatDate();
                FechaHastaTextBox.Text = DateTime.Now.ToFormatDate();
            }
        }
        protected void BuscarButton_Click(object sender, EventArgs e)
        {
            Expression<Func<Estudiantes, bool>> filtro = x => true;
            RepositorioBase<Estudiantes> repositorio = new RepositorioBase<Estudiantes>();
            int id;
            switch (BuscarPorDropDownList.SelectedIndex)
            {
                case 0:
                    filtro = x => true;
                    break;
                case 1://ID
                    id = (FiltroTextBox.Text).ToInt();
                    filtro = x => x.EstudianteId == id;
                    break;
                case 2:// nombre
                    filtro = x => x.Nombre.Contains(FiltroTextBox.Text);
                    break;
                case 3:
                    filtro = x => x.Apellido.Contains(FiltroTextBox.Text);
                    break;
            }
            DateTime fechaDesde = FechaDesdeTextBox.Text.ToDatetime();
            DateTime FechaHasta = FechaHastaTextBox.Text.ToDatetime();
            if (FechaCheckBox.Checked)
                Lista = repositorio.GetList(filtro).Where(x => x.Fecha >= fechaDesde && x.Fecha<= FechaHasta).ToList();
            else
                Lista = repositorio.GetList(filtro);
            repositorio.Dispose();
            this.BindGrid(Lista);
        }

        private void BindGrid(List<Estudiantes> lista)
        {
            DatosGridView.DataSource = null;
            DatosGridView.DataSource = lista;
            DatosGridView.DataBind();
        }

        protected void FechaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (FechaCheckBox.Checked)
            {
                FechaDesdeTextBox.Visible = true;
                FechaHastaTextBox.Visible = true;
            }
            else
            {
                FechaDesdeTextBox.Visible = false;
                FechaHastaTextBox.Visible = false;
            }
        }

        protected void DatosGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            DatosGridView.DataSource = Lista;
            DatosGridView.PageIndex = e.NewPageIndex;
            DatosGridView.DataBind();
        }

        protected void ImprimirButton_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Popup", $"ShowReporte('Listado de Estudiantes');", true);

            EstudiantesReportViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
            EstudiantesReportViewer.Reset();
            EstudiantesReportViewer.LocalReport.ReportPath = Server.MapPath(@"~\Reportes\ListadoEstudiantes.rdlc");
            EstudiantesReportViewer.LocalReport.DataSources.Clear();

            EstudiantesReportViewer.LocalReport.DataSources.Add(new ReportDataSource("Estudiantes",
                                                               Lista));
            EstudiantesReportViewer.LocalReport.Refresh();
        }
    }
}