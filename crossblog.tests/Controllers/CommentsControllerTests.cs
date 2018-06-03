using crossblog.Controllers;
using crossblog.Domain;
using crossblog.Model;
using crossblog.Repositories;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace crossblog.tests.Controllers
{
    public class CommentsControllerTests
    {
        private CommentsController _commentsController;

        private Mock<ICommentRepository> _commentRepositoryMock = new Mock<ICommentRepository>();
        private Mock<IArticleRepository> _articleRepositoryMock = new Mock<IArticleRepository>();


        public CommentsControllerTests()
        {
            _commentsController = new CommentsController(_articleRepositoryMock.Object, _commentRepositoryMock.Object);
        }

        [Fact]
        public async Task Get_NotFound()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Article>(null));


            var commentDbSetMock = Builder<Comment>.CreateListOfSize(5).Build().ToAsyncDbSetMock();
            _commentRepositoryMock.Setup(m => m.Query()).Returns(commentDbSetMock.Object);

            // Act
            var result = await _commentsController.Get(1, 1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
        }
       

        [Fact]
        public async Task Get_ReturnsItem()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult(Builder<Article>.CreateNew().Build()));

            var commentDbSetMock = Builder<Comment>.CreateListOfSize(2).Build().ToAsyncDbSetMock();
            _commentRepositoryMock.Setup(m => m.Query()).Returns(commentDbSetMock.Object);

            // Act
            var result = await _commentsController.Get(1, 1);
    

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            var content = objectResult.Value as CommentModel;
            Assert.NotNull(content);

            Assert.Equal("Title1", content.Title);
        }

        [Fact]
        public async Task Post_ReturnsCreated()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult(Builder<Article>.CreateNew().Build()));
            var comment = Builder<CommentModel>.CreateNew()
                .Build();


            // Act
            var result = await _commentsController.Post(1, comment);

            // Assert
            Assert.NotNull(result);
            var objectResult = result as CreatedResult;
            Assert.NotNull(objectResult);
        }

        [Fact]
        public async Task Post_ReturnsNotFound()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(3)).Returns(Task.FromResult(Builder<Article>.CreateNew().Build()));
            var comment = Builder<CommentModel>.CreateNew()
                 .Build();


            // Act
            var result = await _commentsController.Post(1, comment);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
        }
    }
}
