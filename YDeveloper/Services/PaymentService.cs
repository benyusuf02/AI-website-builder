using Microsoft.Extensions.Configuration;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using YDeveloper.Models;
using Microsoft.AspNetCore.Http;
using YDeveloper.Data;

namespace YDeveloper.Services
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(string userId, decimal amount, string holderName, string cardNumber, string expireMonth, string expireYear, string cvv);
        Task<CheckoutFormInitialize?> InitializeCheckoutFormAsync(ApplicationUser user, decimal amount, string callbackUrl);
        Task<CheckoutForm?> RetrieveCheckoutFormResult(string token);
        Task<bool> ChargeStoredCardAsync(string userToken, decimal amount);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly Options _options;
        private readonly YDeveloperContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentService(IConfiguration configuration, YDeveloperContext context, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _options = new Options
            {
                ApiKey = _configuration["Payment:ApiKey"],
                SecretKey = _configuration["Payment:SecretKey"],
                BaseUrl = _configuration["Payment:BaseUrl"]
            };
        }

        public async Task<bool> ProcessPaymentAsync(string userId, decimal amount, string holderName, string cardNumber, string expireMonth, string expireYear, string cvv)
        {
            try
            {
                // Create Request
                var request = new CreatePaymentRequest
                {
                    Locale = Locale.TR.ToString(),
                    ConversationId = Guid.NewGuid().ToString(),
                    Price = amount.ToString("F2").Replace(',', '.'),
                    PaidPrice = amount.ToString("F2").Replace(',', '.'),
                    Currency = Currency.TRY.ToString(),
                    Installment = 1,
                    BasketId = "B" + Guid.NewGuid().ToString().Substring(0, 6),
                    PaymentChannel = PaymentChannel.WEB.ToString(),
                    PaymentGroup = PaymentGroup.PRODUCT.ToString(),

                    PaymentCard = new PaymentCard
                    {
                        CardHolderName = holderName,
                        CardNumber = cardNumber,
                        ExpireMonth = expireMonth,
                        ExpireYear = expireYear,
                        Cvc = cvv,
                        RegisterCard = 0
                    },

                    Buyer = new Buyer
                    {
                        Id = userId,
                        Name = "John", // In real app, get from User
                        Surname = "Doe",
                        GsmNumber = "+905350000000",
                        Email = "email@email.com",
                        IdentityNumber = "74300864791",
                        LastLoginDate = "2015-10-05 12:43:35",
                        RegistrationDate = "2013-04-21 15:12:09",
                        RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1",
                        Ip = "85.34.78.112",
                        City = "Istanbul",
                        Country = "Turkey",
                        ZipCode = "34732"
                    },

                    BillingAddress = new Address
                    {
                        ContactName = "Jane Doe",
                        City = "Istanbul",
                        Country = "Turkey",
                        Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1",
                        ZipCode = "34742"
                    },

                    ShippingAddress = new Address
                    {
                        ContactName = "Jane Doe",
                        City = "Istanbul",
                        Country = "Turkey",
                        Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1",
                        ZipCode = "34742"
                    },

                    BasketItems = new List<BasketItem>
                    {
                        new BasketItem
                        {
                            Id = "BI101",
                            Name = "Website Package",
                            Category1 = "Software",
                            ItemType = BasketItemType.VIRTUAL.ToString(),
                            Price = amount.ToString("F2").Replace(',', '.')
                        }
                    }
                };

                var clientIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                var paymentTransaction = new PaymentTransaction
                {
                    UserId = userId,
                    Amount = amount,
                    Currency = Currency.TRY.ToString(),
                    IpAddress = clientIp,
                    UserAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString(),
                    IsThreeDSecure = false, // Direct API call implies non-3D unless specified otherwise
                    Timestamp = DateTime.UtcNow
                };

                // Call API
                var payment = await Task.Run(() => Payment.Create(request, _options));

                if (payment.Status == "success")
                {
                    paymentTransaction.Status = "Success";
                    paymentTransaction.TransactionId = payment.PaymentId;
                    _context.PaymentTransactions.Add(paymentTransaction);
                    await _context.SaveChangesAsync();

                    return true;
                }
                else
                {
                    paymentTransaction.Status = "Failure";
                    paymentTransaction.ErrorMessage = payment.ErrorMessage;
                    _context.PaymentTransactions.Add(paymentTransaction);
                    await _context.SaveChangesAsync();

                    Console.WriteLine($"Payment Error: {payment.ErrorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Payment Exception: {ex.Message}");
                return false;
            }
        }
        public async Task<CheckoutForm?> RetrieveCheckoutFormResult(string token)
        {
            try
            {
                var request = new RetrieveCheckoutFormRequest
                {
                    Locale = Locale.TR.ToString(),
                    ConversationId = Guid.NewGuid().ToString(),
                    Token = token
                };

                return await Task.Run(() => CheckoutForm.Retrieve(request, _options));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RetrieveCheckoutForm Error: {ex.Message}");
                return null;
            }
        }

        public async Task<CheckoutFormInitialize?> InitializeCheckoutFormAsync(ApplicationUser user, decimal amount, string callbackUrl)
        {
            try
            {
                // Save Pending Transaction
                var conversationId = Guid.NewGuid().ToString();

                var pendingTransaction = new PaymentTransaction
                {
                    UserId = user.Id,
                    Amount = amount,
                    Currency = Currency.TRY.ToString(),
                    ConversationId = conversationId,
                    Status = "Pending",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = "85.34.78.112" // Or get from context if available
                };

                _context.PaymentTransactions.Add(pendingTransaction);
                await _context.SaveChangesAsync();

                var request = new CreateCheckoutFormInitializeRequest
                {
                    Locale = Locale.TR.ToString(),
                    ConversationId = conversationId,
                    Price = amount.ToString("F2").Replace(',', '.'),
                    PaidPrice = amount.ToString("F2").Replace(',', '.'),
                    Currency = Currency.TRY.ToString(),
                    BasketId = "B" + Guid.NewGuid().ToString().Substring(0, 6),
                    PaymentGroup = PaymentGroup.SUBSCRIPTION.ToString(),
                    CallbackUrl = callbackUrl,
                    EnabledInstallments = new List<int> { 1 },

                    Buyer = new Buyer
                    {
                        Id = user.Id,
                        Name = !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName.Split(' ').FirstOrDefault() : "Degerli",
                        Surname = (!string.IsNullOrWhiteSpace(user.FullName) && user.FullName.Contains(" ")) ? user.FullName.Substring(user.FullName.IndexOf(" ") + 1) : "Musteri",
                        GsmNumber = user.PhoneNumber ?? "+905000000000",
                        Email = user.Email ?? "email@email.com",
                        IdentityNumber = user.IdentityNumber ?? "11111111111",
                        LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        RegistrationDate = user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                        RegistrationAddress = user.Address ?? "Adres Yok",
                        Ip = "85.34.78.112",
                        City = user.City ?? "Istanbul",
                        Country = user.Country ?? "Turkey",
                        ZipCode = user.ZipCode ?? "34000"
                    },

                    BillingAddress = new Address
                    {
                        ContactName = !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : "Degerli Musteri",
                        City = user.City ?? "Istanbul",
                        Country = user.Country ?? "Turkey",
                        Description = user.Address ?? "Adres Yok",
                        ZipCode = user.ZipCode ?? "34000"
                    },

                    ShippingAddress = new Address
                    {
                        ContactName = !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : "Degerli Musteri",
                        City = user.City ?? "Istanbul",
                        Country = user.Country ?? "Turkey",
                        Description = user.Address ?? "Adres Yok",
                        ZipCode = user.ZipCode ?? "34000"
                    },

                    BasketItems = new List<BasketItem>
                    {
                        new BasketItem
                        {
                            Id = "SUB_MONTHLY_01",
                            Name = "Aylık Web Sitesi Kiralama",
                            Category1 = "Abonelik",
                            ItemType = BasketItemType.VIRTUAL.ToString(),
                            Price = amount.ToString("F2").Replace(',', '.')
                        }
                    }
                };

                return await Task.Run(() => CheckoutFormInitialize.Create(request, _options));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InitCheckoutForm Error: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> ChargeStoredCardAsync(string userToken, decimal amount)
        {
            try
            {
                // MOCK Implementation for Recurring Payment
                // Real implementation requires 'Payment.Create' with 'PaymentCard' object having 'CardUserKey' or 'CardToken'.

                await Task.Delay(500); // Simulate API
                Console.WriteLine($"[Recurring Payment] Charged {amount} TRY from Token: {userToken}");

                /* 
                   // Real World Example (Simplified):
                   var request = new CreatePaymentRequest { ... };
                   request.PaymentCard = new PaymentCard { CardUserKey = userToken };
                   var payment = Payment.Create(request, _options);
                   return payment.Status == "success";
                */

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ChargeStoredCard Error: {ex.Message}");
                return false;
            }
        }
    }
}
