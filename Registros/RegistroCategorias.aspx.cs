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
    public partial class RegistroCategorias : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                FechaTextBox.Text = DateTime.Now.ToFormatDate();
            }
        }
        public void Limpiar()
        {
            CategoriaIdTextBox.Text = 0.ToString();
            FechaTextBox.Text = DateTime.Now.ToFormatDate();
            DescripcionTextBox.Text = string.Empty;
        }
        public Categorias LlenarClase()
        {
            return new Categorias(CategoriaIdTextBox.Text.ToInt(),
                DescripcionTextBox.Text,
                0,
                FechaTextBox.Text.ToDatetime());
        }
        public void LlenaCampos(Categorias categorias)
        {
            CategoriaIdTextBox.Text = categorias.CategoriaId.ToString();
            FechaTextBox.Text = categorias.Fecha.ToFormatDate();
            DescripcionTextBox.Text = categorias.Descripcion;
        }
        public bool Validar()
        {
            bool paso = true;
            if (string.IsNullOrWhiteSpace(DescripcionTextBox.Text))
                paso = false;
            return paso;
            
        }
        public bool ExisteEnLaBaseDeDatos()
        {
            RepositorioBase<Categorias> repositorio = new RepositorioBase<Categorias>();
            return !(repositorio.Buscar(CategoriaIdTextBox.Text.ToInt()).EsNulo());
        }
        protected void NuevoButton_Click(object sender, EventArgs e)
        {
            Limpiar();
        }
        protected void GuadarButton_Click(object sender, EventArgs e)
        {
            if (!Validar())
                return;
            RepositorioBase<Categorias> repositorio = new RepositorioBase<Categorias>();
            Categorias categorias = LlenarClase();
            bool paso = false;
            TipoTitulo tipoTitulo = TipoTitulo.OperacionFallida;
            TiposMensajes tiposMensajes = TiposMensajes.RegistroNoGuardado;
            IconType iconType = IconType.error;

            if (categorias.CategoriaId == 0)
                paso = repositorio.Guardar(categorias);
            else
            {
                if (!ExisteEnLaBaseDeDatos())
                {
                    Utils.ToastSweet(this, IconType.info, TiposMensajes.RegistroNoEncontrado);
                }
                else
                    paso = repositorio.Modificar(categorias);
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
            RepositorioBase<Categorias> repositorio = new RepositorioBase<Categorias>();

            if (!ExisteEnLaBaseDeDatos())
            {
                Utils.ToastSweet(this, IconType.info, TiposMensajes.RegistroNoEncontrado);
            }
            else
            {
                if(repositorio.Eliminar(CategoriaIdTextBox.Text.ToInt()))
                {
                    Utils.ToastSweet(this, IconType.success, TiposMensajes.RegistroEliminado);
                }
            }
            repositorio.Dispose();
        }

        protected void BuscarButton_ServerClick(object sender, EventArgs e)
        {
            RepositorioBase<Categorias> repositorio = new RepositorioBase<Categorias>();
            if (!ExisteEnLaBaseDeDatos())
            {
                Utils.ToastSweet(this, IconType.info, TiposMensajes.RegistroNoEncontrado);
            }
            else
            {
                Categorias categorias = repositorio.Buscar(CategoriaIdTextBox.Text.ToInt());
                if (!categorias.EsNulo())
                    LlenaCampos(categorias);
                else
                    Utils.ToastSweet(this, IconType.info, TiposMensajes.RegistroNoEncontrado);
            }
            repositorio.Dispose();
        }
    }
}