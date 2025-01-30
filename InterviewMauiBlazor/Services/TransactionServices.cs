using InterviewMauiBlazor.Database;
using InterviewMauiBlazor.Database.Entities;
using InterviewMauiBlazor.Database.Repositories;
using InterviewMauiBlazor.DTO;
using Syncfusion.Blazor.Data;
using AutoMapper;
using System;
using Microsoft.EntityFrameworkCore;

namespace InterviewMauiBlazor.Services
{
    public interface ITransactionServices
    {
        List<TotalTransactionPerDaysDTO> GetByDate(DateTime? startDate, DateTime? endDate);
        List<TransactionDTO> GetAll();
        TotalTransactionDTO GetTotal(DateTime startDate, DateTime endDate, string Status);
        List<ProductStatsDTO> GetTopProduct();
        void Insert(TransactionDTO TransactionDTO);
        void Update(TransactionDTO TransactionDTO);
        void Delete(int id);
        List<ProductDTO> GetProducts();
        List<OrderDTO> GetOrders();

    }
    public class TransactionServices : ITransactionServices
    {
        private ApplicationDbContext dBContext;
        private ITransactionRepositories transactionRespostory;
        private IProductRepositories productRespostory;
        private IOrderRepositories orderRespostory;
        private IMapper _mapper;
        public TransactionServices(ApplicationDbContext dBContext, IMapper mapper, 
            ITransactionRepositories transactionRespostory,
            IProductRepositories productRepository,
            IOrderRepositories orderRepository)
        {
            this.dBContext = dBContext;
            this.transactionRespostory = transactionRespostory;
            this._mapper = mapper;
            this.productRespostory = productRepository;
            orderRespostory = orderRepository;
        }
        public void Save()
        {
            dBContext.SaveChanges();
        }
        public void Insert(TransactionDTO TransactionDTO)
        {
            var Transaction = _mapper.Map<Transaction>(TransactionDTO);
            Transaction.Order = orderRespostory.GetById(TransactionDTO.orderId);
            Transaction.Product = productRespostory.GetById(TransactionDTO.ProductId);

            dBContext.Attach(Transaction.Order);
            dBContext.Attach(Transaction.Product);

            transactionRespostory.Add(Transaction);
            Save();
        }
        public void Update(TransactionDTO TransactionDTO)
        {
            var existingTransaction = transactionRespostory.Get(t=>t.TransactionId == TransactionDTO.TransactionId, "Product,Order",true);
            if (existingTransaction != null)
            {
                existingTransaction.OrderId = TransactionDTO.orderId;
                existingTransaction.ProductId = TransactionDTO.ProductId;
                existingTransaction.Quantity = TransactionDTO.Quantity;
                existingTransaction.TotalPrice = TransactionDTO.TotalPrice;
                existingTransaction.Buyer = TransactionDTO.Buyer;
                existingTransaction.Seller = TransactionDTO.Seller;
                existingTransaction.Time = TransactionDTO.Time;
                existingTransaction.Status = TransactionDTO.Status;
                existingTransaction.Order = orderRespostory.GetById(TransactionDTO.orderId);
                existingTransaction.Product = productRespostory.GetById(TransactionDTO.ProductId);
                transactionRespostory.Update(existingTransaction);

                Save();

            }

        }
        public void Delete(int id)
        {
            var existingTransaction = transactionRespostory.GetById(id);
            if (existingTransaction != null)
            {
                transactionRespostory.Delete(existingTransaction);
                Save();
            }
        }
        public List<TotalTransactionPerDaysDTO> GetByDate(DateTime? startDate, DateTime? endDate)
        {
            var transactions = transactionRespostory.GetMany(t => t.Time >= startDate && t.Time <= endDate);
            var transactionPerDaysDTO = transactions.GroupBy(t => t.Time).Select(g => new TotalTransactionPerDaysDTO { 
                date = g.First().Time,
                TotalTransaction = g.Count(),
                TotalValue = g.Sum(t=>t.TotalPrice)
            }).ToList();

            return transactionPerDaysDTO;
        }
        public TotalTransactionDTO GetTotal(DateTime startDate, DateTime endDate,string Status )
        {
            var transactions = transactionRespostory.GetMany(t => t.Time >= startDate && t.Time <= endDate);
            var transactionDTO = new TotalTransactionDTO {
                TotalTransaction = string.IsNullOrEmpty(Status) ? transactions.Count()
                : transactions.Count(t => t.Status.Equals(Status, StringComparison.OrdinalIgnoreCase)),

                TotalValue = string.IsNullOrEmpty(Status) ? transactions.Sum(t=>t.TotalPrice) 
                : transactions.Where(t => t.Status.Equals(Status, StringComparison.OrdinalIgnoreCase)).Sum(t => t.TotalPrice),
            };
            return transactionDTO;
        }
        public List<ProductStatsDTO> GetTopProduct()
        {
            var TopProduct = transactionRespostory.GetList(null,null,"Product",0,0)
                .Where(t => t.Product != null).GroupBy(t => t.ProductId).Select(t => new ProductStatsDTO
            {
                ProductId = t.First().ProductId,
                ProductName = t.First().Product.Name,
                TotalQuantity = t.Sum(t => t.Quantity),               
            }).OrderByDescending(t => t.TotalQuantity).Take(10).ToList();
            return TopProduct;
        }
        public List<TransactionDTO> GetAll()
        {
            List<TransactionDTO> transactions = transactionRespostory.GetList(null, null, "Product,Order", 0, 0)
                .Select(t => new TransactionDTO
                {
                    TransactionId = t.TransactionId,
                    Time = t.Time,
                    TotalPrice = t.TotalPrice,
                    Buyer = t.Buyer,
                    Seller = t.Seller,
                    Status = t.Status,
                    orderId = t.OrderId,
                    ProductId = t.ProductId,
                    Order = _mapper.Map<OrderDTO>(t.Order),
                    Product=_mapper.Map<ProductDTO>(t.Product),
                    Quantity = t.Quantity,
                }).ToList();

            return transactions;
        }

        public List<ProductDTO> GetProducts()
        {
            return _mapper.Map<List<ProductDTO>>(productRespostory.GetAll());
        }
        public List<OrderDTO> GetOrders()
        {
            return orderRespostory.GetList(null, null, "Customer", 0, 0).Select(
                 o => new OrderDTO
                 {
                     CustomerName = o.Customer.Name,
                     OrderDate = o.OrderDate,
                     Id = o.Id,
                 }

            ).ToList();
        }
    }
}
