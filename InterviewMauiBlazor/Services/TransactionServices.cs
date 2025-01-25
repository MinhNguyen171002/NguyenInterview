using InterviewMauiBlazor.Database;
using InterviewMauiBlazor.Database.Entities;
using InterviewMauiBlazor.Database.Repositories;
using InterviewMauiBlazor.DTO;
using Syncfusion.Blazor.Data;
using AutoMapper;

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
        void Delete(int proid, int orid);
        List<ProductDTO> GetProducts();
        List<OrderDTO> GetOrders();

    }
    public class TransactionServices : ITransactionServices
    {
        private ApplicationDbContext dBContext;
        private ITransactionRepositories transactionRespostory;
        private IProductRepositories ProductRespostory;
        private IOrderRepositories OrderRespostory;
        private IMapper _mapper;
        public TransactionServices(ApplicationDbContext dBContext, IMapper mapper, 
            ITransactionRepositories transactionRespostory,
            IProductRepositories productRepository,
            IOrderRepositories orderRepository)
        {
            this.dBContext = dBContext;
            this.transactionRespostory = transactionRespostory;
            this._mapper = mapper;
            this.ProductRespostory = productRepository;
            OrderRespostory = orderRepository;
        }
        public void Save()
        {
            dBContext.SaveChanges();
        }
        public void Insert(TransactionDTO TransactionDTO)
        {

            var existingTransaction = transactionRespostory.Get(t => t.OrderId == TransactionDTO.orderId);
            if (existingTransaction == null)
            {
                existingTransaction = _mapper.Map<Transaction>(TransactionDTO);
                existingTransaction.Product = ProductRespostory.GetById(TransactionDTO.ProductId);
                existingTransaction.Order = OrderRespostory.GetById(TransactionDTO.orderId);
                transactionRespostory.Add(existingTransaction);
                Save();
            }
        }
        public void Update(TransactionDTO TransactionDTO)
        {
            var existingTransaction = transactionRespostory.Get(t => t.OrderId == TransactionDTO.orderId);
            if (existingTransaction != null)
            {
                transactionRespostory.Delete(existingTransaction);
                Save();

                 var newTransaction = _mapper.Map<Transaction>(TransactionDTO);

                newTransaction.Product = ProductRespostory.GetById(TransactionDTO.ProductId);
                newTransaction.Order = OrderRespostory.GetById(TransactionDTO.orderId);
                transactionRespostory.Add(newTransaction);
                Save();
            }
        }
        public void Delete(int proid,int orid)
        {
            var existingTransaction = transactionRespostory.getByKeys(proid,orid);
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
                    Time = t.Time,
                    TotalPrice = t.TotalPrice,
                    Buyer = t.Buyer,
                    Seller = t.Seller,
                    Status = t.Status,
                    Order = _mapper.Map<OrderDTO>(t.Order),
                    Product=_mapper.Map<ProductDTO>(t.Product),
                    Quantity = t.Quantity,
                }).ToList();

            return transactions;
        }

        public List<ProductDTO> GetProducts()
        {
            return _mapper.Map<List<ProductDTO>>(ProductRespostory.GetAll());
        }
        public List<OrderDTO> GetOrders()
        {
            return OrderRespostory.GetList(null, null, "Customer", 0, 0).Select(
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
