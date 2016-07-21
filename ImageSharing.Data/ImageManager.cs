using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSharing.Data
{
    public class ImageManager
    {
        private string _constr;
        public ImageManager(string constr)
        {
            _constr = constr;
        }

        public IEnumerable<Image> GetMostViewedImages()
        {
            using (SqlConnection connection = new SqlConnection(_constr))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT TOP 5 i.* FROM Image i ORDER BY Views DESC";
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Image> images = new List<Image>();
                while (reader.Read())
                {
                    images.Add(new Image
                    {
                        Id = (int)reader["Id"],
                        FileName = (string)reader["FileName"],
                        Description = (string)reader["Description"],
                        Views = (int)reader["Views"],
                        Date = (DateTime)reader["Date"],
                    });
                }
                return images;
            }
        }
        public IEnumerable<Image> GetRecentImages()
        {
            using (SqlConnection connection = new SqlConnection(_constr))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT TOP 5 i.* FROM Image i ORDER BY Date DESC";
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Image> images = new List<Image>();
                while (reader.Read())
                {
                    images.Add(new Image
                    {
                        Id = (int)reader["Id"],
                        FileName = (string)reader["FileName"],
                        Description = (string)reader["Description"],
                        Views = (int)reader["Views"],
                        Date = (DateTime)reader["Date"],
                    });
                }
                return images;
            }
        }
        public IEnumerable<ViewsCount> GetViews()
        {
            using (SqlConnection connection = new SqlConnection(_constr))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT i.Id, i.Views FROM Image i";
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<ViewsCount> views = new List<ViewsCount>();
                while (reader.Read())
                {
                    views.Add(new ViewsCount
                    {
                        Id = (int)reader["Id"],
                        Views = (int)reader["Views"],
                    });
                }
                return views;
            }
        }
        public Image GetImage(int id)
        {
            using (SqlConnection connection = new SqlConnection(_constr))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Image WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                Image image = new Image();
                image.Id = (int)reader["Id"];
                image.FileName = (string)reader["FileName"];
                image.Description = (string)reader["Description"];
                image.Views = (int)reader["Views"];
                image.Date = (DateTime)reader["Date"];
                return image;
            }
        }
        public int AddImage(string fileName, string description)
        {
            using (SqlConnection connection = new SqlConnection(_constr))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Image(FileName, Description, Views, Date)VALUES(@file, @description, @views, @date) SELECT @@Identity";
                command.Parameters.AddWithValue("@file", fileName);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@views", 0);
                command.Parameters.AddWithValue("@date", DateTime.Now);
                connection.Open();
                return (int)(decimal)command.ExecuteScalar();
            }
        }
        public void AddView(int id)
        {
            using (SqlConnection connection = new SqlConnection(_constr))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE Image SET Views = Views + 1 WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteScalar();
            }
        }
        public void AddLike(int userId, int imageId)
        {
            using (SqlConnection connection = new SqlConnection(_constr))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO UsersLikedImages VALUES(@userId, @imageId)";
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@imageId", imageId);
                connection.Open();
                command.ExecuteScalar();
            }
        }
        public int GetLikesCount(int imageId)
        {
            using (SqlConnection connection = new SqlConnection(_constr))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM UsersLikedImages WHERE imageId = @imageId";
                command.Parameters.AddWithValue("@imageId", imageId);
                connection.Open();
                return (int)command.ExecuteScalar();

            }
        }
        public bool CheckIfUserLikedImage(int userId, int imageId)
        {
            using (SqlConnection connection = new SqlConnection(_constr))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "select * from UsersLikedImages where userId = @userId and ImageId = @imageId ";
                command.Parameters.AddWithValue("@imageId", imageId);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();
                return command.ExecuteReader().Read();
            }
        }
        public IEnumerable<ImageWithLikes> TopMostLiked()
        {
            using (SqlConnection connection = new SqlConnection(_constr))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT TOP 5 i.*, COUNT(il.ImageId) as 'LikesCount'
                                     FROM Image i LEFT JOIN UsersLikedImages il ON i.Id = il.ImageId 
                                        GROUP BY i.Id, i.FileName, i.Date, i.Description, i.Views ORDER BY Count(il.ImageId) DESC";
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<ImageWithLikes> images = new List<ImageWithLikes>();
                while (reader.Read())
                {
                    images.Add(new ImageWithLikes
                    {
                        Image = (new Image
                         {
                             Id = (int)reader["Id"],
                             FileName = (string)reader["FileName"],
                             Description = (string)reader["Description"],
                             Views = (int)reader["Views"],
                             Date = (DateTime)reader["Date"],
                         }),
                        LikesCount = (int)reader["LikesCount"]
                    });
                }
                return images;
            }
        }
        public IEnumerable<Image> GetUsersLikedImages(int id)
        {
            using (SqlConnection connection = new SqlConnection(_constr))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT i.* FROM Image i LEFT JOIN UsersLikedImages il ON i.Id = il.ImageId
                        WHERE il.UserId = @id GROUP BY i.Id, i.FileName, i.Date, i.Description, i.Views ORDER BY i.Date DESC";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Image> images = new List<Image>();
                while (reader.Read())
                {
                    images.Add(new Image
                    {
                        Id = (int)reader["Id"],
                        FileName = (string)reader["FileName"],
                        Description = (string)reader["Description"],
                        Views = (int)reader["Views"],
                        Date = (DateTime)reader["Date"],
                    });
                }
                return images;
            }
        }
    }
}
