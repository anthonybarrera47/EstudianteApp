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
    public class DetalleEvaluaciones
    {
        [Key]
        public int DetalleID { get; set; }
        public int EvaluacionID { get; set; }
        [ForeignKey("EvaluacionID")]
        public virtual Evaluaciones Evaluaciones { get; set; }
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public virtual Categorias Categorias { get; set; }
        public decimal Valor { get; set; }
        public decimal Logrado { get; set; }
        public decimal Perdido { get; set; }

        public DetalleEvaluaciones(int detalleID, int evaluacionID, int categoriaId, decimal valor, decimal logrado, decimal perdido)
        {
            DetalleID = detalleID;
            EvaluacionID = evaluacionID;
            Evaluaciones = new Evaluaciones();
            CategoriaId = categoriaId;
            Categorias = new Categorias();
            Valor = valor;
            Logrado = logrado;
            Perdido = perdido;
        }

        public DetalleEvaluaciones()
        {
            DetalleID = 0;
            EvaluacionID = 0;
            Evaluaciones = new Evaluaciones();
            CategoriaId = 0;
            Categorias = new Categorias();
            Valor = 0;
            Logrado = 0;
            Perdido = 0;
        }
    }
}
