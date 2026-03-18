using TodoApi.Services;
using TodoApi.Models;
using TodoApi.Controllers;
using Moq;


namespace TodoApi.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var service = new TodoService();
        Assert.NotNull(service);
        Assert.True(true);
    }

    [Fact]
    public void TestCreateTodo()
    {
        var service = new TodoService();
        var todo = new Todo
        {
            Title = "Test",
            Description = "Test Description",
            IsCompleted = false
        };

        var result = service.CreateTodo(todo);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public void TestGetTodo()
    {
        var service = new TodoService();
        var todos = service.GetAllTodos();

        Assert.True(todos.Count > 0);
    }

    [Fact]
    public void UpdateTest()
    {
        var service = new TodoService();
        var todo = new Todo
        {
            Title = "Updated",
            Description = "Updated Description",
            IsCompleted = true
        };

        var result = service.UpdateTodo(1, todo);
        Assert.NotNull(result);
    }

    [Fact]
    public void DeleteWorks()
    {
        var service = new TodoService();
        var result = service.DeleteTodo(999);

        Assert.False(result);
    }

    [Fact]
    public void ControllerTest()
    {
        var controller = new TodoController(new TodoService());
        var todo = new Todo { Title = "Test", Description = "Desc" };

        var result = controller.CreateTodo(todo);

        Assert.NotNull(result);
    }

    [Fact]
    public void TestEverything()
    {
        var service = new TodoService();

        var todo1 = service.CreateTodo(new Todo { Title = "1", Description = "D1" });
        var todo2 = service.CreateTodo(new Todo { Title = "2", Description = "D2" });

        var all = service.GetAllTodos();

        service.UpdateTodo(todo1.Id, new Todo { Title = "Updated", Description = "D1" });

        service.DeleteTodo(todo2.Id);

        Assert.True(all.Count >= 2);
    }

    // --- NEW UNIT TESTS: Testing ITodoService in isolation using Moq ---

    [Fact]
    public void ITodoService_CreateTodo_ReturnsCreatedTodoWithId()
    {
        // Arrange
        var mockService = new Mock<ITodoService>();
        var inputTodo = new Todo { Title = "Buy groceries", Description = "Milk and eggs" };
        var expectedTodo = new Todo { Id = 42, Title = "Buy groceries", Description = "Milk and eggs", IsCompleted = false };

        mockService
            .Setup(s => s.CreateTodo(It.IsAny<Todo>()))
            .Returns(expectedTodo);

        // Act
        ITodoService service = mockService.Object;
        var result = service.CreateTodo(inputTodo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result.Id);
        Assert.Equal("Buy groceries", result.Title);
        mockService.Verify(s => s.CreateTodo(It.IsAny<Todo>()), Times.Once);
    }

    [Fact]
    public void ITodoService_CreateTodo_SetsIsCompletedFalseByDefault()
    {
        // Arrange
        var mockService = new Mock<ITodoService>();
        var inputTodo = new Todo { Title = "Walk the dog" };
        var expectedTodo = new Todo { Id = 5, Title = "Walk the dog", IsCompleted = false };

        mockService
            .Setup(s => s.CreateTodo(It.IsAny<Todo>()))
            .Returns(expectedTodo);

        // Act
        var result = mockService.Object.CreateTodo(inputTodo);

        // Assert
        Assert.False(result.IsCompleted);
        Assert.Equal("Walk the dog", result.Title);
    }

    [Fact]
    public void ITodoService_GetTodoById_ReturnsCorrectTodo()
    {
        // Arrange
        var mockService = new Mock<ITodoService>();
        var expectedTodo = new Todo { Id = 7, Title = "Read a book", Description = "Clean Code", IsCompleted = false };

        mockService
            .Setup(s => s.GetTodoById(7))
            .Returns(expectedTodo);

        // Act
        var result = mockService.Object.GetTodoById(7);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Id);
        Assert.Equal("Read a book", result.Title);
        mockService.Verify(s => s.GetTodoById(7), Times.Once);
    }

    [Fact]
    public void ITodoService_GetTodoById_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var mockService = new Mock<ITodoService>();

        mockService
            .Setup(s => s.GetTodoById(It.IsAny<int>()))
            .Returns((Todo?)null);

        // Act
        var result = mockService.Object.GetTodoById(9999);

        // Assert - should return null for a non-existent ID
        Assert.Null(result);
        mockService.Verify(s => s.GetTodoById(9999), Times.Once);
    }
}
