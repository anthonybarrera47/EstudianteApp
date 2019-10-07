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
                LlenarCombo();
            }
        }
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
            LlenarCombo();
            this.BindGrid();
        }
        private void LlenaCampo(Evaluaciones evaluaciones)
        {
            Limpiar();
            EvaluacionIdTextBox.Text = evaluaciones.EvaluacionID.ToString();
            FechaTextBox.Text = evaluaciones.Fecha.ToFormatDate();
            EstudianteDropdownList.SelectedValue = evaluaciones.EstudianteId.ToString();
            ViewState[KeyViewState] = evaluaciones;
            this.BindGrid();
        }
        private Evaluaciones LlenaClase()
        {
            Evaluaciones evaluaciones = ViewStateEvaluaciones();
            evaluaciones.EvaluacionID = EvaluacionIdTextBox.Text.ToInt();
            evaluaciones.EstudianteId = EstudianteDropdownList.SelectedValue.ToInt();
            evaluaciones.Fecha = FechaTextBox.Text.ToDatetime();
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
        private Evaluaciones ViewStateEvaluaciones()
        {
            return (Evaluaciones)ViewState[KeyViewState];
        }
        private void BindGrid()
        {
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
                    LlenaCampo(evaluaciones);
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
            Evaluaciones evaluaciones= LlenaClase();
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
                    Utils.ToastSweet(this, IconType.success, TiposMensajes.RegistroEliminado);
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
                                        Valor, Logrado,Valor-Logrado);
            ViewState[KeyViewState] = evaluaciones;
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
    }
}