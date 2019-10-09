
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    [Serializable]
    public class Evaluaciones
    {
        [Key]
        public int EvaluacionID { get; set; }
        public DateTime Fecha { get; set; }
        public int EstudianteId { get; set; }
        [ForeignKey("EstudianteId")]
        public virtual Estudiantes Estudiantes { get; set; }
        [NotMapped]   
        public string NombreEstudiante { get; set; }
        public decimal TotalPerdido { get; set; }
        public virtual List<DetalleEvaluaciones> DetalleEvaluaciones { get; set; }

        public Evaluaciones(int evaluacionID, DateTime fecha, int estudianteId, decimal totalPerdido)
        {
            EvaluacionID = evaluacionID;
            Fecha = fecha;
            EstudianteId = estudianteId;
            Estudiantes = new Estudiantes();
            NombreEstudiante = string.Empty;
            TotalPerdido = totalPerdido;
            DetalleEvaluaciones = new List<DetalleEvaluaciones>();
        }
        public Evaluaciones()
        {
            EvaluacionID = 0;
            Fecha = DateTime.Now;
            EstudianteId = 0;
            NombreEstudiante = string.Empty;
            TotalPerdido = 0;
            DetalleEvaluaciones = new List<DetalleEvaluaciones>();
        }
        public void AgregarDetalle(int detalleID, int evaluacionID, int categoriaId, decimal valor, decimal logrado, decimal perdido)
        {
            DetalleEvaluaciones.Add(new DetalleEvaluaciones(detalleID, evaluacionID, categoriaId, valor, logrado, perdido));
        }
        public void RemoverDetalle(int index)
        {
            this.DetalleEvaluaciones.RemoveAt(index);
        }
    }

}
