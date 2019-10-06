using Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Contexto : DbContext
    {
        public virtual DbSet<Evaluaciones> Evaluaciones { get; set; }
        public virtual DbSet<Estudiantes> Estudiantes { get; set; }
        public virtual DbSet<Categorias> Categorias { get; set; }

        public Contexto() : base("ConStr")
        {

        }
    }
}
