using Microsoft.Data.Sqlite;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly string _connectionString = "Data Source=todos.db";

        public TodoService()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Todos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    IsCompleted INTEGER NOT NULL,
                    CreatedAt TEXT NOT NULL
                );
            ";
            command.ExecuteNonQuery();
        }

        public Todo CreateTodo(Todo todo)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Todos (Title, Description, IsCompleted, CreatedAt)
                VALUES (@Title, @Description, @IsCompleted, @CreatedAt);
                SELECT last_insert_rowid();
            ";
            
            command.Parameters.AddWithValue("@Title", todo.Title);
            command.Parameters.AddWithValue("@Description", todo.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsCompleted", todo.IsCompleted ? 1 : 0);
            
            var createdAt = DateTime.UtcNow;
            command.Parameters.AddWithValue("@CreatedAt", createdAt.ToString("o"));

            var id = Convert.ToInt32(command.ExecuteScalar());
            todo.Id = id;
            todo.CreatedAt = createdAt;
            return todo;
        }

        public List<Todo> GetAllTodos()
        {
            var todos = new List<Todo>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Todos";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                todos.Add(new Todo
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = !reader.IsDBNull(2) ? reader.GetString(2) : null,
                    IsCompleted = reader.GetInt32(3) == 1,
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                });
            }

            return todos;
        }

        public Todo? GetTodoById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Todos WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Todo
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = !reader.IsDBNull(2) ? reader.GetString(2) : null,
                    IsCompleted = reader.GetInt32(3) == 1,
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                };
            }

            return null;
        }

        public Todo UpdateTodo(int id, Todo todo)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Todos
                SET Title = @Title, Description = @Description, IsCompleted = @IsCompleted
                WHERE Id = @Id
            ";
            
            command.Parameters.AddWithValue("@Title", todo.Title);
            command.Parameters.AddWithValue("@Description", todo.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsCompleted", todo.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();

            todo.Id = id;
            return todo;
        }

        public bool DeleteTodo(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Todos WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}
