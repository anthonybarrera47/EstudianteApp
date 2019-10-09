using BLL;
using Entidades;
using Extensores;
using Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EstudianteApp.Registros
{
    public partial class RegistroEvaluaciones : System.Web.UI.Page
    {
        readonly string KeyViewState = "Evaluacion";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FechaTextBox.Text = DateTime.Now.ToFormatDate();
                ViewState[KeyViewState] = new Evaluaciones();
                Limpiar();
                int id = Request.QueryString["EvaluacionID"].ToInt();
                if (id > 0)
                {
                    using (RepositorioEvaluacion repositorio = new RepositorioEvaluacion())
                    {
                        Evaluaciones evaluaciones = repositorio.Buscar(id);
                        if (evaluaciones.EsNulo())
                            Utils.Alerta(this, TipoTitulo.Informacion, TiposMensajes.RegistroNoEncontrado, IconType.info);
                        else
                            LlenarCampos(evaluaciones);
                    }
                }
            }
        }

        #region "Metodos"
        private void LlenarCombo()
        {
            RepositorioBase<Estudiantes> repositorio = new RepositorioBase<Estudiantes>();
            Utils.LlenarCombo<Estudiantes>(EstudianteDropdownList, repositorio.GetList(x => true), "NombreCompleto", "EstudianteId");
            repositorio.Dispose();

            RepositorioBase<Categorias> repositorioCategorias = new RepositorioBase<Categorias>();
            Utils.LlenarCombo<Categorias>(CategoriaDropDownList, repositorioCategorias.GetList(x => true), "Descripcion", "CategoriaId");
            repositorioCategorias.Dispose();
        }
        private void Limpiar()
        {
            EvaluacionIdTextBox.Text = 0.ToString();
            FechaTextBox.Text = DateTime.Now.ToFormatDate();
            ViewState[KeyViewState] = new Evaluaciones();
            ValorTextBox.Text = string.Empty;
            LogradoTextBox.Text = string.Empty;
            LlenarCombo();
            this.BindGrid();
        }
        private void LlenarCampos(Evaluaciones evaluaciones)
        {
            Limpiar();
            EvaluacionIdTextBox.Text = evaluaciones.EvaluacionID.ToString();
            FechaTextBox.Text = evaluaciones.Fecha.ToFormatDate();
            EstudianteDropdownList.SelectedValue = evaluaciones.EstudianteId.ToString();
            TotalPerdidoTextBox.Text = evaluaciones.TotalPerdido.ToString();
            ViewState[KeyViewState] = evaluaciones;
            this.BindGrid();
        }
        private Evaluaciones LlenaClase()
        {
            Evaluaciones evaluaciones = ViewStateEvaluaciones();
            evaluaciones.EvaluacionID = EvaluacionIdTextBox.Text.ToInt();
            evaluaciones.EstudianteId = EstudianteDropdownList.SelectedValue.ToInt();
            evaluaciones.Fecha = FechaTextBox.Text.ToDatetime();
            evaluaciones.TotalPerdido = 0;
            evaluaciones.DetalleEvaluaciones.ForEach(x => evaluaciones.TotalPerdido += x.Perdido);
            return evaluaciones;
        }
        private bool Validar()
        {
            bool paso = true;
            if (EstudianteDropdownList.SelectedValue.ToInt() < 0)
                paso = false;
            if (DetalleGridView.Rows.Count <= 0)
                paso = false;
            return paso;
        }
        private void Calcular()
        {
            Evaluaciones evaluaciones = ViewStateEvaluaciones();
            decimal TotalPerdido = 0;
            evaluaciones.DetalleEvaluaciones.ForEach(x => TotalPerdido += x.Perdido);
            TotalPerdidoTextBox.Text = string.Empty;
            TotalPerdidoTextBox.Text = TotalPerdido.ToString();
        }
        private Evaluaciones ViewStateEvaluaciones()
        {
            return (Evaluaciones)ViewState[KeyViewState];
        }
        private void BindGrid()
        {
            Calcular();
            Evaluaciones evaluaciones = ViewStateEvaluaciones();
            evaluaciones.DetalleEvaluaciones.ForEach(x => x.Categoria = new RepositorioBase<Categorias>().Buscar(x.CategoriaId).Descripcion);
            DetalleGridView.DataSource = evaluaciones.DetalleEvaluaciones;
            DetalleGridView.DataBind();
        }
        private bool ExisteEnLaBaseDeDatos()
        {
            RepositorioEvaluacion repositorio = new RepositorioEvaluacion();
            return !(repositorio.Buscar(EvaluacionIdTextBox.Text.ToInt()).EsNulo());
        }
        #endregion
        #region "Eventos"
        protected void BuscarButton_ServerClick(object sender, EventArgs e)
        {
            RepositorioEvaluacion repositorio = new RepositorioEvaluacion();
            if (!ExisteEnLaBaseDeDatos())
            {
                Utils.ToastSweet(this, IconType.info, TiposMensajes.RegistroNoEncontrado);
            }
            else
            {
                Evaluaciones evaluaciones = repositorio.Buscar(EvaluacionIdTextBox.Text.ToInt());
                if (!evaluaciones.EsNulo())
                    LlenarCampos(evaluaciones);
                else
                    Utils.ToastSweet(this, IconType.info, TiposMensajes.RegistroNoEncontrado);
            }
            repositorio.Dispose();
        }
        protected void NuevoButton_Click(object sender, EventArgs e)
        {
            Limpiar();
        }
        protected void GuadarButton_Click(object sender, EventArgs e)
        {
            if (!Validar())
                return;
            RepositorioEvaluacion repositorio = new RepositorioEvaluacion();
            Evaluaciones evaluaciones = LlenaClase();
            bool paso = false;
            TipoTitulo tipoTitulo = TipoTitulo.OperacionFallida;
            TiposMensajes tiposMensajes = TiposMensajes.RegistroNoGuardado;
            IconType iconType = IconType.error;

            if (evaluaciones.EvaluacionID == 0)
                paso = repositorio.Guardar(evaluaciones);
            else
            {
                if (!ExisteEnLaBaseDeDatos())
                {
                    Utils.ToastSweet(this, IconType.info, TiposMensajes.RegistroNoEncontrado);
                }
                else
                    paso = repositorio.Modificar(evaluaciones);
            }
            if (paso)
            {
                Limpiar();
                tipoTitulo = TipoTitulo.OperacionExitosa;
                tiposMensajes = TiposMensajes.RegistroGuardado;
                iconType = IconType.success;
            }
            Utils.Alerta(this, tipoTitulo, tiposMensajes, iconType);
            repositorio.Dispose();
        }
        protected void EliminarButton_Click(object sender, EventArgs e)
        {
            RepositorioEvaluacion repositorio = new RepositorioEvaluacion();

            if (!ExisteEnLaBaseDeDatos())
            {
                Utils.ToastSweet(this, IconType.info, TiposMensajes.RegistroNoEncontrado);
            }
            else
            {
                if (repositorio.Eliminar(EvaluacionIdTextBox.Text.ToInt()))
                {
                    Limpiar();
                    Utils.ToastSweet(this, IconType.success, TiposMensajes.RegistroEliminado);
                }
                else
                    Utils.ToastSweet(this, IconType.info, TiposMensajes.RegistroNoEncontrado);
            }
            repositorio.Dispose();
        }
        protected void AgregarDetalle_Click(object sender, EventArgs e)
        {
            Evaluaciones evaluaciones = ViewStateEvaluaciones();
            decimal Valor = ValorTextBox.Text.ToDecimal();
            decimal Logrado = LogradoTextBox.Text.ToDecimal();
            evaluaciones.AgregarDetalle(0, evaluaciones.EvaluacionID, CategoriaDropDownList.SelectedValue.ToInt(),
                                        Valor, Logrado, Valor - Logrado);
            ViewState[KeyViewState] = evaluaciones;
            ValorTextBox.Text = string.Empty;
            LogradoTextBox.Text = string.Empty;
            this.BindGrid();
        }

        protected void RemoverDetalleClick_Click(object sender, EventArgs e)
        {
            Evaluaciones evaluaciones = ViewStateEvaluaciones();
            GridViewRow row = (sender as Button).NamingContainer as GridViewRow;
            evaluaciones.RemoverDetalle(row.RowIndex);
            ViewState[KeyViewState] = evaluaciones;
            this.BindGrid();
        }
        #endregion

    }
}