using Backend.Database;
using Backend.Models;
using Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.UnitTests.Repositories
{
    [TestClass]
    public class ArticleRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> _options;
        private ApplicationDbContext _context;
        private ArticleRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            // Set up In-Memory Database
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(_options);
            _repository = new ArticleRepository(_context);
        }

        [TestCleanup]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetMyArticlesAsync_Test()
        {
            // Arrange
            const string myId = "testId";
            var article1 = new Article { UserId = myId, CreatedAt = DateTime.UtcNow.AddDays(-3) };
            var article2 = new Article { UserId = myId, CreatedAt = DateTime.UtcNow.AddDays(-1) };
            var article3 = new Article { UserId = "otherId", CreatedAt = DateTime.UtcNow.AddDays(-5) };

            var articles = new List<Article>()
            {
                article1,
                article2,
                article3,
            };

            _context.Articles.AddRange(articles);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetMyArticlesAsync(myId);

            // Assert
            Assert.AreEqual(2, result.Count); // Csak két ilyen cikk van UserId = "testId"
            Assert.AreEqual(myId, result[0].UserId);
            Assert.AreEqual(myId, result[1].UserId);
            Assert.AreEqual(article2.ArticleId, result[0].ArticleId);
            Assert.AreEqual(article1.ArticleId, result[1].ArticleId);
        }

        [TestMethod]
        public async Task SaveArticleAsync_Test()
        {
            // Arrange
            var testArticle = new Article
            {
                ArticleId = "123",
                Title = "Test Article",
                UserId = "456",
                Text = "Test content",
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDeleted = false,
                ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-1"
                        }
                    }

            };

            // Act
            await _repository.SaveArticleAsync(testArticle);
            var resultArticle = await _context.Articles.SingleOrDefaultAsync(a => a.ArticleId == "123");

            // Assert
            Assert.IsNotNull(resultArticle);
            Assert.AreEqual(testArticle.ArticleId, resultArticle.ArticleId);
        }

        [TestMethod]
        public async Task DeleteArticleAsync_Test()
        {
            // Arrange
            var article1 = new Article
            {
                ArticleId = "123",
                Title = "Test Article 1",
                UserId = "456",
                Text = "Test content 1",
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDeleted = false,
                ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-1"
                        }
                    }
            };

            var article2 = new Article
            {
                ArticleId = "456",
                Title = "Test Article 2",
                UserId = "789",
                Text = "Test content 2",
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDeleted = false,
                ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-1"
                        }
                    }
            };

            _context.Articles.AddRange(article1, article2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteArticleAsync("123");
            var deletedArticle = await _context.Articles.SingleOrDefaultAsync(a => a.ArticleId == "123");
            var otherArticle = await _context.Articles.SingleOrDefaultAsync(a => a.ArticleId == "456");

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(deletedArticle.IsDeleted);
            Assert.AreEqual(DateTime.UtcNow.Date, deletedArticle.LastModified.Date);
            Assert.IsFalse(otherArticle.IsDeleted);
        }

        [TestMethod]
        public async Task RestoreArticleAsync_Test()
        {
            // Arrange
            var article = new Article
            {
                ArticleId = "123",
                Title = "Test Article",
                UserId = "456",
                Text = "Test content",
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDeleted = true,
                ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-1"
                        }
                    }
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.RestoreArticleAsync("123");
            var restoredArticle = await _context.Articles.SingleOrDefaultAsync(a => a.ArticleId == "123");

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(restoredArticle.IsDeleted);
        }

        [TestMethod]
        public async Task HardDeleteArticleAsync_Test()
        {
            // Arrange
            var article1 = new Article
            {
                ArticleId = "123",
                Title = "Test Article 1",
                UserId = "456",
                Text = "Test content 1",
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDeleted = false,
                ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-1"
                        }
                    }
            };

            var article2 = new Article
            {
                ArticleId = "456",
                Title = "Test Article 2",
                UserId = "789",
                Text = "Test content 2",
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDeleted = false,
                ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-2"
                        }
                    }
            };

            _context.Articles.AddRange(article1, article2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.HardDeleteArticleAsync("123");
            var deletedArticle = await _context.Articles.SingleOrDefaultAsync(a => a.ArticleId == "123");
            var otherArticle = await _context.Articles.SingleOrDefaultAsync(a => a.ArticleId == "456");

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(deletedArticle);
            Assert.IsNotNull(otherArticle);
        }

        [TestMethod]
        public async Task GetAllArticlesAsync_ReturnsAllNotDeletedArticlesInDescendingOrder_Test()
        {
            // Arrange
            var article1 = new Article
            {
                ArticleId = "123",
                Title = "Test Article 1",
                UserId = "456",
                Text = "Test content 1",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                LastModified = DateTime.UtcNow.AddDays(-1),
                IsDeleted = false,
                ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-11"
                        }
                    }
            };

            var article2 = new Article
            {
                ArticleId = "124",
                Title = "Test Article 2",
                UserId = "457",
                Text = "Test content 2",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                LastModified = DateTime.UtcNow.AddDays(-2),
                IsDeleted = true,
                ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-22"
                        }
                    }
            };

            var article3 = new Article
            {
                ArticleId = "125",
                Title = "Test Article 3",
                UserId = "458",
                Text = "Test content 3",
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDeleted = false,
                ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-33"
                        }
                    }
            };

            _context.Articles.AddRange(new List<Article> { article1, article2, article3 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllArticlesAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsFalse(result.Any(a => a.IsDeleted));
            Assert.AreEqual(article3.ArticleId, result[0].ArticleId);
            Assert.AreEqual(article1.ArticleId, result[1].ArticleId);
        }


        [TestMethod]
        public async Task GetArticleByIdAsync_ReturnsCorrectArticle_Test()
        {
            // Arrange
            var testArticle = new Article
            {
                ArticleId = "123",
                Title = "Test Article",
                UserId = "456",
                Text = "Test content",
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDeleted = false,
                ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-1"
                        }
                    }
            };

            _context.Articles.Add(testArticle);
            await _context.SaveChangesAsync();

            // Act
            var resultArticle = await _repository.GetArticleByIdAsync("123");

            // Assert
            Assert.IsNotNull(resultArticle);
            Assert.AreEqual(testArticle.ArticleId, resultArticle.ArticleId);
            Assert.AreEqual(testArticle.Title, resultArticle.Title);
            Assert.AreEqual(testArticle.UserId, resultArticle.UserId);
            Assert.AreEqual(testArticle.Text, resultArticle.Text);
        }
        [TestMethod]
        public async Task GetArticleByIdAsync_ThrowsExceptionWhenArticleIdDoesNotExist_Test()
        {
            // Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _repository.GetArticleByIdAsync("noExistId"));
        }

        [TestMethod]
        public async Task GetLatest3ArticlesAsync_ReturnsLatest3Articles_Test()
        {
            // Arrange
            var testArticles = new List<Article>
            {
                new Article
                {
                    ArticleId = "1",
                    Title = "Test Article 1",
                    UserId = "456",
                    Text = "Test content 1",
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    LastModified = DateTime.UtcNow,
                    IsDeleted = false,
                    ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-1" 
                        },
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-2" 
                        }
                    }

                },
                new Article
                {
                    ArticleId = "2",
                    Title = "Test Article 2",
                    UserId = "456",
                    Text = "Test content 2",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    LastModified = DateTime.UtcNow,
                    IsDeleted = false,
                    ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-3"
                        }                  
                    }
                },
                new Article
                {
                    ArticleId = "3",
                    Title = "Test Article 3",
                    UserId = "456",
                    Text = "Test content 3",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    LastModified = DateTime.UtcNow,
                    IsDeleted = false,
                    ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-4"
                        }
                    }
                },
                new Article
                {
                    ArticleId = "4",
                    Title = "Test Article 4",
                    UserId = "456",
                    Text = "Test content 4",
                    CreatedAt = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    IsDeleted = false,
                    ArticleImages = new List<ArticleImage>
                    {
                        new ArticleImage
                        {
                            ImageUrl = "https://example.com/image-url-5"
                        }
                    }
                },
            };

            _context.Articles.AddRange(testArticles);
            await _context.SaveChangesAsync();

            // Act
            var resultArticles = await _repository.GetLatest3ArticlesAsync();

            // Assert
            Assert.IsNotNull(resultArticles);
            Assert.AreEqual(3, resultArticles.Count);
            Assert.AreEqual("4", resultArticles[0].ArticleId);
            Assert.AreEqual("3", resultArticles[1].ArticleId);
            Assert.AreEqual("2", resultArticles[2].ArticleId);
        }
    }
}
