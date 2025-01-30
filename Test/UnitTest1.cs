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
                    new Order { Id = 8, OrderDate = now, CustomerId = 2 },
                    new Order { Id = 9, OrderDate = now.AddDays(1), CustomerId = 2 });
                }
                    
                if (!context.Transactions.Any())
                {
                    context.Transactions.AddRange(
                    new Transaction
                    {
                        TransactionId = 1,
                        OrderId = 1,
                        ProductId = 2,
                        Quantity = 2,
                        TotalPrice = 1199.98m,
                        Buyer = "John Doe",
                        Seller = "TechStore",
                        Time = now.AddDays(-6).Date,
                        Status = "Completed"
                    },
                new Transaction
                {
                    TransactionId = 2,
                    OrderId = 2,
                    ProductId = 3,
                    Quantity = 2,
                    TotalPrice = 1399.98m,
                    Buyer = "Jane Smith",
                    Seller = "TechStore",
                    Time = now.AddDays(-5).Date,
                    Status = "Pending"
                },
                new Transaction
                {
                    TransactionId = 3,
                    OrderId = 3,
                    ProductId = 1,
                    Quantity = 1,
                    TotalPrice = 999.99m,
                    Buyer = "John Doe",
                    Seller = "TechStore",
                    Time = now.AddDays(-4).Date,
                    Status = "Completed"
                },
                new Transaction
                {
                    TransactionId = 4,
                    OrderId = 4,
                    ProductId = 2,
                    Quantity = 2,
                    TotalPrice = 1199.98m,
                    Buyer = "Jane Smith",
                    Seller = "TechStore",
                    Time = now.AddDays(-3).Date,
                    Status = "Pending"
                },
                new Transaction
                {
                    TransactionId = 5,
                    OrderId = 7,
                    ProductId = 2,
                    Quantity = 2,
                    TotalPrice = 1199.98m,
                    Buyer = "John Doe",
                    Seller = "TechStore",
                    Time = now,
                    Status = "Pending"
                },
                new Transaction
                {
                    TransactionId = 6,
                    OrderId = 7,
                    ProductId = 3,
                    Quantity = 2,
                    TotalPrice = 1399.98m,
                    Buyer = "John Doe",
                    Seller = "TechStore",
                    Time = now,
                    Status = "Pending"
                },
                new Transaction
                {
                    TransactionId = 7,
                    OrderId = 8,
                    ProductId = 2,
                    Quantity = 2,
                    TotalPrice = 1199.98m,
                    Buyer = "Jane Smith",
                    Seller = "TechStore",
                    Time = now,
                    Status = "Completed"
                },
                new Transaction
                {
                    TransactionId = 8,
                    OrderId = 9,
                    ProductId = 2,
                    Quantity = 2,
                    TotalPrice = 1199.98m,
                    Buyer = "Jane Smith",
                    Seller = "TechStore",
                    Time = now.AddDays(1),
                    Status = "PreOrder"
                });
                }

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
        public void Insert_Should_Add_New_Transaction()
        {
            // Arrange
            var transactionDTO = new TransactionDTO
            {
                orderId = 1,
                ProductId = 2,
                Quantity = 3,
                TotalPrice = 100.5m,
                Buyer = "John Doe",
                Seller = "TechStore",
                Time = DateTime.Now,
                Status = "Pending"
            };
            var transaction = new Transaction();

            _mockMapper.Setup(m => m.Map<Transaction>(transactionDTO)).Returns(transaction);
            _mockOrderRepository.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns(new Order { Id = transactionDTO.orderId });
            _mockProductRepository.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns(new Product { Id = transactionDTO.ProductId });

            // Act
            _transactionServices.Insert(transactionDTO);

            // Assert
            _mockTransactionRepository.Verify(r => r.Add(It.IsAny<Transaction>()), Times.Once);
            _mockOrderRepository.Verify(r => r.GetById(transactionDTO.orderId), Times.Once);
            _mockProductRepository.Verify(r => r.GetById(transactionDTO.ProductId), Times.Once);
        }

        [Test]
        public void Update_Should_Update_Existing_Transaction()
        {
            // Arrange
            var transactionDTO = new TransactionDTO
            {
                Quantity = 5,
                TotalPrice = 200m
            };
            var existingTransaction = new Transaction
            {
                Quantity = 3,
                TotalPrice = 100m
            };

            _mockTransactionRepository
                .Setup(r => r.Get(It.IsAny<Expression<Func<Transaction, bool>>>(), "Product,Order", true))
                .Returns(existingTransaction);

            // Act
            _transactionServices.Update(transactionDTO);

            // Assert
            Assert.AreEqual(5, existingTransaction.Quantity);
            Assert.AreEqual(200m, existingTransaction.TotalPrice);
            _mockTransactionRepository.Verify(r => r.Update(It.IsAny<Transaction>()), Times.Once);
        }

        [Test]
        public void Delete_Should_Remove_Transaction()
        {
            // Arrange
            var transactionId = 1;
            var existingTransaction = new Transaction { TransactionId = transactionId };

            _mockTransactionRepository.Setup(r => r.GetById(transactionId)).Returns(existingTransaction);

            // Act
            _transactionServices.Delete(transactionId);

            // Assert
            _mockTransactionRepository.Verify(r => r.Delete(It.IsAny<Transaction>()), Times.Once);
        }

        [Test]
        public void GetAll_Should_Return_All_Transactions()
        {
            // Arrange
            var transactions = new List<Transaction>
        {
            new Transaction { TransactionId = 1 },
            new Transaction { TransactionId = 2 }
        };

            _mockTransactionRepository.Setup(r => r.GetList(null, null, "Product,Order", 0, 0)).Returns(transactions);

            // Act
            var result = _transactionServices.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void GetByDate_Should_Filter_Transactions_By_Date_Range()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(-5);
            var endDate = DateTime.Now;

            var transactions = new List<Transaction>
        {
            new Transaction { TransactionId = 1, Time = DateTime.Now.AddDays(-4) },
            new Transaction { TransactionId = 2, Time = DateTime.Now.AddDays(-3) }
        };

            _mockTransactionRepository
                .Setup(r => r.GetMany(It.IsAny<Expression<Func<Transaction, bool>>>()))
                .Returns(transactions);

            // Act
            var result = _transactionServices.GetByDate(startDate, endDate);

            // Assert
            Assert.AreEqual(2, result.Count);
        }
    }

}
