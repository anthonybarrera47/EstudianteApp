namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categorias",
                c => new
                    {
                        CategoriaId = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                        PromedioPerdida = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Fecha = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CategoriaId);
            
            CreateTable(
                "dbo.Estudiantes",
                c => new
                    {
                        EstudianteId = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Apellido = c.String(),
                        PuntosPerdidos = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.EstudianteId);
            
            CreateTable(
                "dbo.Evaluaciones",
                c => new
                    {
                        EvaluacionID = c.Int(nullable: false, identity: true),
                        Fecha = c.DateTime(nullable: false),
                        EstudianteId = c.Int(nullable: false),
                        TotalPerdido = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.EvaluacionID)
                .ForeignKey("dbo.Estudiantes", t => t.EstudianteId, cascadeDelete: true)
                .Index(t => t.EstudianteId);
            
            CreateTable(
                "dbo.DetalleEvaluaciones",
                c => new
                    {
                        DetalleID = c.Int(nullable: false, identity: true),
                        EvaluacionID = c.Int(nullable: false),
                        CategoriaId = c.Int(nullable: false),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Logrado = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Perdido = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DetalleID)
                .ForeignKey("dbo.Categorias", t => t.CategoriaId, cascadeDelete: true)
                .ForeignKey("dbo.Evaluaciones", t => t.EvaluacionID, cascadeDelete: true)
                .Index(t => t.EvaluacionID)
                .Index(t => t.CategoriaId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Evaluaciones", "EstudianteId", "dbo.Estudiantes");
            DropForeignKey("dbo.DetalleEvaluaciones", "EvaluacionID", "dbo.Evaluaciones");
            DropForeignKey("dbo.DetalleEvaluaciones", "CategoriaId", "dbo.Categorias");
            DropIndex("dbo.DetalleEvaluaciones", new[] { "CategoriaId" });
            DropIndex("dbo.DetalleEvaluaciones", new[] { "EvaluacionID" });
            DropIndex("dbo.Evaluaciones", new[] { "EstudianteId" });
            DropTable("dbo.DetalleEvaluaciones");
            DropTable("dbo.Evaluaciones");
            DropTable("dbo.Estudiantes");
            DropTable("dbo.Categorias");
        }
    }
}
