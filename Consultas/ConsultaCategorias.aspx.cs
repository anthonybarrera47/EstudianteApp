using BLL;
using Entidades;
using Extensores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EstudianteApp.Consultas
{
    public partial class ConsultaCategorias : System.Web.UI.Page
    {

        static List<Categorias> Lista = new List<Categorias>();
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
            Expression<Func<Categorias, bool>> filtro = x => true;
            RepositorioBase<Categorias> repositorio = new RepositorioBase<Categorias>();
            int id;
            switch (BuscarPorDropDownList.SelectedIndex)
            {
                case 0:
                    filtro = x => true;
                    break;
                case 1://ID
                    id = (FiltroTextBox.Text).ToInt();
                    filtro = x => x.CategoriaId == id;
                    break;
                case 2:// nombre
                    filtro = x => x.Descripcion.Contains(FiltroTextBox.Text);
                    break;
                case 3:
                    filtro = x => x.PromedioPerdida == FiltroTextBox.Text.ToDecimal();
                    break;
            }
            DateTime fechaDesde = FechaDesdeTextBox.Text.ToDatetime();
            DateTime FechaHasta = FechaHastaTextBox.Text.ToDatetime();
            if (FechaCheckBox.Checked)
                Lista = repositorio.GetList(filtro).Where(x => x.Fecha >= fechaDesde && x.Fecha <= FechaHasta).ToList();
            else
                Lista = repositorio.GetList(filtro);
            repositorio.Dispose();
            this.BindGrid(Lista);
        }

        private void BindGrid(List<Categorias> lista)
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


    }
}