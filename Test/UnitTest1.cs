using NUnit.Framework;
using Moq;
using AutoMapper;
using InterviewMauiBlazor.Database;
using InterviewMauiBlazor.Database.Repositories;
using InterviewMauiBlazor.Services;
using InterviewMauiBlazor.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using InterviewMauiBlazor.Database.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace InterviewMauiBlazor.Tests
{
    [TestFixture]
    public class TransactionServicesTests
    {
        private TransactionServices _transactionServices;
        private ApplicationDbContext _dbContext;
        private Mock<ITransactionRepositories> _mockTransactionRepository = new Mock<ITransactionRepositories>();
        private Mock<IProductRepositories> _mockProductRepository = new Mock<IProductRepositories>();
        private Mock<IOrderRepositories> _mockOrderRepository = new Mock<IOrderRepositories>();
        private Mock<IMapper> _mockMapper = new Mock<IMapper>();

        [SetUp]
        public void Setup()
        {


            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                if (!context.Products.Any())
                {
                    context.Products.AddRange(
                        new Product { Id = 1, Name = "Laptop", Price = 999.99m },
                        new Product { Id = 2, Name = "Smartphone", Price = 599.99m },
                        new Product { Id = 3, Name = "Ipad", Price = 699.99m }
                    );
                }
                    context.Customers.AddRange(
                    new Customer { Id = 1, Name = "John Doe", Email = "john@example.com" },
                    new Customer { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
                );


                var now = DateTime.Now.Date;
                if (!context.Orders.Any())
                {
                    context.Orders.AddRange(
                    new Order { Id = 1, OrderDate = now.AddDays(-6).Date, CustomerId = 1 },
                    new Order { Id = 2, OrderDate = now.AddDays(-5).Date, CustomerId = 2 },
                    new Order { Id = 3, OrderDate = now.AddDays(-4).Date, CustomerId = 1 },
                    new Order { Id = 4, OrderDate = now.AddDays(-3).Date, CustomerId = 2 },
                    new Order { Id = 5, OrderDate = now.AddDays(-2).Date, CustomerId = 2 },
                    new Order { Id = 6, OrderDate = now.AddDays(-1).Date, CustomerId = 1 },
                    new Order { Id = 7, OrderDate = now, CustomerId = 1 },
                    new Order { Id = 8, OrderDate = now.AddDays(1), CustomerId = 2 });
                }
                    
                if (!context.Transactions.Any())
                {
                    context.Transactions.AddRange(
                    new Transaction { OrderId = 1, ProductId = 2, Quantity = 2, TotalPrice = 1199.98m, Buyer = "Jane Smith", Seller = "TechStore", Time = DateTime.Now.AddDays(-2).Date, Status = "Completed" },
                    new Transaction { OrderId = 2, ProductId = 3, Quantity = 2, TotalPrice = 1399.98m, Buyer = "Jane Smith", Seller = "TechStore", Time = DateTime.Now.AddDays(-1).Date, Status = "Pending" },
                    new Transaction { OrderId = 3, ProductId = 1, Quantity = 1, TotalPrice = 999.99m, Buyer = "John Doe", Seller = "TechStore", Time = DateTime.Now.AddDays(-1).Date, Status = "Completed" },
                    new Transaction { OrderId = 4, ProductId = 2, Quantity = 2, TotalPrice = 1199.98m, Buyer = "Jane Smith", Seller = "TechStore", Time = now, Status = "Pending" }
                );
                }

                // Save to the context
                context.SaveChanges();
            }

            // Initialize the services
            _transactionServices = new TransactionServices(
                new ApplicationDbContext(options),
                _mockMapper.Object,
                _mockTransactionRepository.Object,
                _mockProductRepository.Object,
                _mockOrderRepository.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            // Reset the mocks to ensure state does not carry over
            _mockTransactionRepository.Reset();
            _mockProductRepository.Reset();
            _mockOrderRepository.Reset();
            if (_dbContext != null)
            {
                _dbContext.Dispose();
            }
        }

        [Test]
        public void Insert_WhenTransactionDoesNotExist_ShouldAddNewTransaction()
        {
            // Arrange
            var transactionDTO = new TransactionDTO { ProductId = 1, orderId = 1, Quantity = 2, Time = DateTime.Now.Date };
            var transaction = new Transaction { ProductId = 1, OrderId = 1, Quantity = 2 };
            _mockTransactionRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Transaction, bool>>>())).Returns((Transaction)null);
            _mockMapper.Setup(m => m.Map<Transaction>(transactionDTO)).Returns(transaction);
            _mockProductRepository.Setup(r => r.GetById(1)).Returns(new Product { Id = 1 });
            _mockOrderRepository.Setup(r => r.GetById(1)).Returns(new Order { Id = 1 });

            // Act
            _transactionServices.Insert(transactionDTO);

            // Assert
            _mockTransactionRepository.Verify(r => r.Add(transaction), Times.Once);
        }

        [Test]
        public void Update_WhenTransactionExists_ShouldUpdateTransaction()
        {
            // Arrange
            var transactionDTO = new TransactionDTO { ProductId = 1, orderId = 1, Quantity = 3 };
            var existingTransaction = new Transaction { ProductId = 1, OrderId = 1, Quantity = 2 };
            var updatedTransaction = new Transaction { ProductId = 1, OrderId = 1, Quantity = 3 };
            _mockTransactionRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Transaction, bool>>>())).Returns(existingTransaction);
            _mockMapper.Setup(m => m.Map<Transaction>(transactionDTO)).Returns(updatedTransaction);
            _mockProductRepository.Setup(r => r.GetById(1)).Returns(new Product { Id = 1 });
            _mockOrderRepository.Setup(r => r.GetById(1)).Returns(new Order { Id = 1 });

            // Act
            _transactionServices.Update(transactionDTO);

            // Assert
            _mockTransactionRepository.Verify(r => r.Delete(existingTransaction), Times.Once);
            _mockTransactionRepository.Verify(r => r.Add(updatedTransaction), Times.Once);
        }

        [Test]
        public void Delete_WhenTransactionExists_ShouldDeleteTransaction()
        {
            // Arrange
            var existingTransaction = new Transaction { ProductId = 1, OrderId = 1 };
            _mockTransactionRepository.Setup(r => r.getByKeys(1, 1)).Returns(existingTransaction);

            // Act
            _transactionServices.Delete(1, 1);

            // Assert
            _mockTransactionRepository.Verify(r => r.Delete(existingTransaction), Times.Once);
        }

        [Test]
        public void GetTotal_ShouldReturnCorrectTotal_WhenCalled()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction { Time = DateTime.Now.AddDays(-1), TotalPrice = 1999.98m, Status = "Completed" },
                new Transaction { Time = DateTime.Now.AddDays(-2), TotalPrice = 2999.97m, Status = "Completed" }
            };

            // Mock GetMany để trả về danh sách giao dịch
            _mockTransactionRepository.Setup(r => r.GetMany(It.IsAny<Expression<Func<Transaction, bool>>>()))
                .Returns((Expression<Func<Transaction, bool>> predicate) =>
                    transactions.AsQueryable().Where(predicate).ToList());

            // Act
            var result = _transactionServices.GetTotal(DateTime.Now.AddDays(-7), DateTime.Now, "Completed");

            // Assert
            Assert.AreEqual(2, result.TotalTransaction);
            Assert.AreEqual(4999.95m, result.TotalValue);
        }

        [Test]
        public void GetTopProduct_ShouldReturnTopProducts_WhenCalled()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction { ProductId = 1, Quantity = 10, Product = new Product { Id = 1, Name = "Laptop" } },
                new Transaction { ProductId = 2, Quantity = 20, Product = new Product { Id = 2, Name = "Smartphone" } },
                new Transaction { ProductId = 1, Quantity = 5, Product = new Product { Id = 1, Name = "Laptop" } }
            };

            // Mock GetList để trả về danh sách giao dịch
            _mockTransactionRepository.Setup(r => r.GetList(null, null, "Product", 0, 0))
                .Returns(transactions);

            // Act
            var result = _transactionServices.GetTopProduct();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(20, result.First().TotalQuantity); // Product with ID 2 should have the highest quantity
        }

        
    }
}
