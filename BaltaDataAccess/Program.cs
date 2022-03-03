using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BaltaDataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BaltaDataAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "Server=CAIO-PC\\SQLEXPRESS;Database=balta;user id=sa;password=123456; Encrypt=False;";

            //Abrir conexão com o BD
            using (var connection = new SqlConnection(connectionString))
            {
                //CreateCategory(connection);
                //CreateManyCategorys(connection);
                //UpdateCategory(connection);
                // DeleteCategory(connection);
                // ListCategories(connection);
                //GetCategory(connection);
                //ExecuteProcedure(connection);
                //ExecuteReadProcedure(connection);
                //ExecuteScalar(connection);
                //ReadView(connection);
                //OneToOne(connection);
                OneToMany(connection);
            }
        }


        static void ListCategories(SqlConnection connection)
        {
            var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");
            foreach (var item in categories)
            {
                Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }
        static void CreateCategory(SqlConnection connection)
        {
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            var insertSql = @"INSERT INTO 
                    [Category] 
                VALUES(
                    @Id, 
                    @Title, 
                    @Url, 
                    @Summary, 
                    @Order, 
                    @Description, 
                    @Featured)";

            var rows = connection.Execute(insertSql, new
            {
                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            });
            Console.WriteLine($"{rows} linhas inseridas");
        }

        static void CreateManyCategorys(SqlConnection connection)
        {
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços do AWS";
            category.Summary = "AWS Cloud";
            category.Order = 8;
            category.Featured = false;

            var category2 = new Category();
            category2.Id = Guid.NewGuid();
            category2.Title = "Categoria Nova";
            category2.Url = "categoria-nova";
            category2.Description = "Categoria nova";
            category2.Summary = "categoria";
            category2.Order = 9;
            category2.Featured = true;

            // NUNCA deve-se cocatenar string no INSERT, UPDATE etc. (Por conta do -> SQL INJECTION). 
            // Devemos optar por utilizar parâmetros (SqlParameter), que são definidos pelo '@'

            var insertSql = @"INSERT INTO 
                    [Category] 
                VALUES(
                    @Id,
                    @Title,
                    @Url,
                    @Summary, 
                    @Order,  
                    @Description, 
                    @Featured)";

            var rows = connection.Execute(insertSql, new[] {
                new
                {
                    category.Id,
                    category.Title,
                    category.Url,
                    category.Summary,
                    category.Order,
                    category.Description,
                    category.Featured
                },
                new
                {
                    category2.Id,
                    category2.Title,
                    category2.Url,
                    category2.Summary,
                    category2.Order,
                    category2.Description,
                    category2.Featured
                }
            });

            Console.WriteLine($"{rows} linhas inseridas");
        }

        static void UpdateCategory(SqlConnection connection)
        {
            var updateQuery = "UPDATE [Category] SET [Title]=@title WHERE [Id]=@id";
            var rows = connection.Execute(updateQuery, new
            {
                id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),  //Poderia colocar sem o 'new guid', apenas com aspas.
                title = "Frontend 2022"
            });

            Console.WriteLine($"{rows} registros atualizados");
        }

        static void DeleteCategory(SqlConnection connection)
        {
            var deleteQuery = "DELETE [Category] WHERE [Id]=@id";
            var rows = connection.Execute(deleteQuery, new
            {
                id = new Guid("89eb6419-1d5e-49ab-97e7-f995cb1cd01b"),
            });

            Console.WriteLine($"{rows} registros excluídos");
        }

        static void GetCategory(SqlConnection connection)
        {
            var category = connection
                .QueryFirstOrDefault<Category>(
                    "SELECT TOP 1 [Id], [Title] FROM [Category] WHERE [Id]=@id",
                    new
                    {
                        id = "af3407aa-11ae-4621-a2ef-2028b85507c4"
                    });
            Console.WriteLine($"{category.Id} - {category.Title}");

        }

        static void ExecuteProcedure(SqlConnection connection)
        {
            var procedure = "spDeleteStudent";
            var parametro = new { StudentId = "1DD13D45-7AE1-4353-907F-1538857D47B9" };
            var affectedRows = connection.Execute(procedure, parametro, commandType: CommandType.StoredProcedure);

            Console.WriteLine($"{affectedRows} linhas afetadas");
        }

        static void ExecuteReadProcedure(SqlConnection connection)
        {
            var procedure = "[spGetCoursesByCategory]";
            var parametro = new { CategoryId = "09CE0B7B-CFCA-497B-92C0-3290AD9D5142" };
            var courses = connection.Query(procedure, parametro, commandType: CommandType.StoredProcedure);

            foreach (var item in courses)
            {
                Console.WriteLine(item.Title);
            }
        }

        static void ExecuteScalar(SqlConnection connection)
        {
            var category = new Category();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            var insertSql = @"
                INSERT INTO 
                    [Category] 
                OUTPUT inserted.[Id]
                VALUES(
                    NEWID(), 
                    @Title, 
                    @Url, 
                    @Summary, 
                    @Order, 
                    @Description, 
                    @Featured) 
                SELECT SCOPE_IDENTITY()";

            var id = connection.ExecuteScalar<Guid>(insertSql, new
            {
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            });
            Console.WriteLine($"A categoria inserida foi: {id}");
        }

        static void ReadView(SqlConnection connection)
        {
            var sql = "SELECT * FROM [vwCourses]";

            var courses = connection.Query(sql);
            foreach (var item in courses)
            {
                Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }

        static void OneToOne(SqlConnection connection)
        {
            var sql = @"
                SELECT 
                    * 
                FROM 
                    [CareerItem] 
                INNER JOIN [Course] ON [CareerItem].[CourseId] = [Course].[Id]";

            var items = connection.Query<CareerItem, Course, CareerItem>(
                sql,
                (careerItem, course) =>
                {
                    careerItem.Course = course;
                    return careerItem;
                }, splitOn: "Id");

            foreach (var item in items)
            {
                Console.WriteLine($"{item.Title} - Curso: {item.Course.Title}");
            }
        }

        static void OneToMany(SqlConnection connection)
        {
            var sql = @"
                SELECT 
	                [Career].[Id],
	                [Career].[Title],
	                [CareerItem].[CareerId],
	                [CareerItem].[Title]
                FROM
	                [Career]
                INNER JOIN
	                [CareerItem] ON [CareerItem].[CareerId] = [Career].[Id] 
                ORDER BY
	                [Career].[Title]";

            var careers = new List<Career>();
            var items = connection.Query<Career, CareerItem, Career>(
                sql,
                (career, item) =>
                {
                    var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
                    if (car == null)
                    {
                        car = career;
                        car.Items.Add(item);
                        careers.Add(car);
                    }
                    else
                    {
                        car.Items.Add(item);
                    }
                    
                    return career;
                }, splitOn: "CareerId");

            foreach (var career in careers)
            {
                Console.WriteLine($"{career.Title}:");

                foreach (var item in career.Items)
                {
                    Console.WriteLine($" - {item.Title}");

                }
            }
        }
    }
}






//using (var connection = new SqlConnection(connectionString))
//{
//    Console.WriteLine("Conectado");
//    connection.Open();

//    using (var command = new SqlCommand())
//    {
//        command.Connection = connection;
//        command.CommandType = System.Data.CommandType.Text;
//        command.CommandText = "SELECT [Id], [Title] FROM [Category]";

//        var reader = command.ExecuteReader();
//        while (reader.Read())
//        {
//            Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)}");
//        }
//    }
//}