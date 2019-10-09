using BLL;
using Entidades;
using Extensores;
using Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EstudianteApp.Consultas
{
    public partial class ConsultaEvaluaciones : System.Web.UI.Page
    {
        static List<Evaluaciones> Lista = new List<Evaluaciones>();
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
            Expression<Func<Evaluaciones, bool>> filtro = x => true;
            RepositorioBase<Evaluaciones> repositorio = new RepositorioBase<Evaluaciones>();
            int id;
            switch (BuscarPorDropDownList.SelectedIndex)
            {
                case 0:
                    filtro = x => true;
                    break;
                case 1://ID
                    FiltroTextBox.TextMode = TextBoxMode.Number;
                    id = (FiltroTextBox.Text).ToInt();
                    filtro = x => x.EvaluacionID == id;
                    break;
                case 2:// nombre
                    FiltroTextBox.TextMode = TextBoxMode.Number;
                    id = (FiltroTextBox.Text).ToInt();
                    filtro = x => x.EstudianteId == id;
                    break;
                case 3:
                    filtro = x => x.TotalPerdido == FiltroTextBox.Text.ToDecimal();
                    break;
            }

            FiltroTextBox.TextMode = TextBoxMode.SingleLine;
            DateTime fechaDesde = FechaDesdeTextBox.Text.ToDatetime();
            DateTime FechaHasta = FechaHastaTextBox.Text.ToDatetime();
            if (FechaCheckBox.Checked)
                Lista = repositorio.GetList(filtro).Where(x => x.Fecha >= fechaDesde && x.Fecha <= FechaHasta).ToList();
            else
                Lista = repositorio.GetList(filtro);
            repositorio.Dispose();
            using(RepositorioBase<Estudiantes> repositorioEstudiantes = new RepositorioBase<Estudiantes>())
            {
                foreach(var item in Lista)
                {
                    item.NombreEstudiante = repositorioEstudiantes.Buscar(item.EstudianteId).NombreCompleto;
                }
            }
            this.BindGrid(Lista);
        }

        private void BindGrid(List<Evaluaciones> lista)
        {
            DatosGridView.DataSource = null;
            DatosGridView.DataSource = lista;
            DatosGridView.DataBind();
        }

        protected void DatosGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            DatosGridView.DataSource = Lista;
            DatosGridView.PageIndex = e.NewPageIndex;
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

        protected void VerDetalleButton_Click(object sender, EventArgs e)
        {
            string titulo = "Detalle de la Evaluación";
            Utils.MostrarModal(this.Page, "ShowPopup", titulo);
            GridViewRow row = (sender as Button).NamingContainer as GridViewRow;
            var Evaluacion = Lista.ElementAt(row.RowIndex);
            DetalleDatosGridView.DataSource = null;
            RepositorioEvaluacion Repositorio = new RepositorioEvaluacion();
            List<DetalleEvaluaciones> Details = Repositorio.Buscar(Evaluacion.EvaluacionID).DetalleEvaluaciones;
            using(RepositorioBase<Categorias> RepositorioCategorias = new RepositorioBase<Categorias>())
            {
                Details.ForEach(x => x.Categoria = RepositorioCategorias.Buscar(x.CategoriaId).Descripcion);
            }
            DetalleDatosGridView.DataSource = Details;
            DetalleDatosGridView.DataBind();
            Repositorio.Dispose();
        }
    }
}