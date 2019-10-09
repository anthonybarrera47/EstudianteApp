using DAL;
using Entidades;
using Extensores;
using Herramientas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class RepositorioEvaluacion : RepositorioBase<Evaluaciones>
    {
        public override bool Guardar(Evaluaciones entity)
        {
            bool paso = false;
            RepositorioBase<Estudiantes> repositorio = new RepositorioBase<Estudiantes>();
            Estudiantes estudiantes = repositorio.Buscar(entity.EstudianteId);
            estudiantes.PuntosPerdidos += entity.TotalPerdido;
            if (repositorio.Modificar(estudiantes))
            {
                repositorio.Dispose();
                paso = base.Guardar(entity);
            }
            CalcularPromedio();
            return paso;
        }
        public override Evaluaciones Buscar(int id)
        {
            Evaluaciones Evaluaciones = new Evaluaciones();
            Contexto db = new Contexto();
            try
            {

                Evaluaciones = db.Evaluaciones.Include(x => x.DetalleEvaluaciones)
                    .Where(x => x.EvaluacionID == id).FirstOrDefault();
            }
            catch (Exception)
            { throw; }
            finally
            { db.Dispose(); }
            return Evaluaciones;
        }
        public override bool Modificar(Evaluaciones entity)
        {
            bool paso = false;
            decimal PuntosPerdidos = 0;
            Evaluaciones Anterior = Buscar(entity.EvaluacionID);
            RepositorioBase<Estudiantes> repositorio = new RepositorioBase<Estudiantes>();
            Estudiantes estudiantes = repositorio.Buscar(entity.EstudianteId);
            Contexto db = new Contexto();

            try
            {
                using (Contexto contexto = new Contexto())
                {
                    foreach (var item in Anterior.DetalleEvaluaciones.ToList())
                    {
                        if (!entity.DetalleEvaluaciones.Exists(x => x.DetalleID == item.DetalleID))
                        {
                            contexto.Entry(item).State = EntityState.Deleted;
                            estudiantes.PuntosPerdidos -= item.Perdido;
                        }
                    }
                    contexto.SaveChanges();
                }
                foreach (var item in entity.DetalleEvaluaciones)
                {
                    var estado = EntityState.Unchanged;
                    if (item.DetalleID == 0)
                    {
                        estado = EntityState.Added;
                        estudiantes.PuntosPerdidos += item.Perdido;
                    }
                    db.Entry(item).State = estado;
                }
                estudiantes.PuntosPerdidos += PuntosPerdidos;
                repositorio.Modificar(estudiantes);
                db.Entry(entity).State = EntityState.Modified;
                paso = db.SaveChanges() > 0;
                CalcularPromedio();
            }
            catch (Exception)
            { throw; }
            finally
            { db.Dispose(); }
            return paso;
        }
        public override bool Eliminar(int id)
        {
            bool paso = false;
            RepositorioBase<Estudiantes> repositorio = new RepositorioBase<Estudiantes>();
            Evaluaciones evaluaciones = Buscar(id);
            Estudiantes estudiantes = repositorio.Buscar(evaluaciones.EstudianteId);
            estudiantes.PuntosPerdidos -= evaluaciones.TotalPerdido;
            if (repositorio.Modificar(estudiantes))
            {
                repositorio.Dispose();
                paso = base.Eliminar(id);
            }
            CalcularPromedio();
            return paso;
        }
        public override List<Evaluaciones> GetList(Expression<Func<Evaluaciones, bool>> expression)
        {
            List<Evaluaciones> ListaEvaluaciones = new List<Evaluaciones>();
            Contexto db = new Contexto();
            try
            {
                foreach (var item in base.GetList(expression))
                {
                    ListaEvaluaciones.Add(Buscar(item.EvaluacionID));
                }
            }
            catch (Exception)
            { throw; }
            finally
            { db.Dispose(); }
            return ListaEvaluaciones;
        }
        public void CalcularPromedio()
        {
            RepositorioBase<Categorias> repositorio = new RepositorioBase<Categorias>();
            List<Categorias> ListaCategorias = repositorio.GetList(x => true);
            List<Evaluaciones> ListaEvaluaciones = GetList(x => true);
            Dictionary<int, decimal> Dic = new Dictionary<int, decimal>();
            decimal TotalPuntosPerdidos = 0;
            foreach(var item in ListaCategorias.ToList())
            {
                Dic.Add(item.CategoriaId, 0);
            }
            foreach (var item in ListaEvaluaciones.ToList())
            {
                TotalPuntosPerdidos += item.TotalPerdido;
                item.DetalleEvaluaciones.ForEach(x => Dic[x.CategoriaId] += x.Perdido);
            }
            foreach(var item in Dic)
            {
                int Repeticiones = 0;
                decimal Promedio = 0;
                ListaEvaluaciones.ForEach(x => x.DetalleEvaluaciones.ForEach(t =>
                                                                                {
                                                                                    if (t.CategoriaId == item.Key)
                                                                                        Repeticiones++;
                                                                                }));
                Categorias categorias = repositorio.Buscar(item.Key);
                if (Repeticiones > 0)
                    Promedio = item.Value / Repeticiones;
                else
                    Promedio = 0;
                categorias.PromedioPerdida = Promedio;
                repositorio.Modificar(categorias);
            }

        }
    }
}
