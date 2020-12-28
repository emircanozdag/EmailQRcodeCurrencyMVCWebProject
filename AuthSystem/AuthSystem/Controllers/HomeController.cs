
using AuthSystem.Areas.Identity.Data;
using AuthSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public  IActionResult Mail()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult Mail(Email model)
        {
            MailMessage mailim = new MailMessage();
            mailim.To.Add("emircanozdag@gmail.com");
            mailim.From = new MailAddress("emircanozdag@gmail.com");
            mailim.Subject = "Bize Ulaşın Sayfasından Mesajınız Var. " + model.Subject;
            mailim.Body = "Sayın yetkili, " + model.To + " kişisinden gelen mesajın içeriği aşağıdaki gibidir. <br>" + model.Body;
            mailim.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient
            {
                Credentials = new NetworkCredential("emircanozdag@gmail.com", "password"),
                Port = 587,
                Host = "smtp.gmail.com",
                EnableSsl = true
            };
            try
            {
                smtp.Send(mailim);
                TempData["Message"] = "Mesajınız iletilmiştir. En kısa zamanda size geri dönüş sağlanacaktır.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Mesaj gönderilemedi.Hata nedeni:" + ex.Message;
            }

            return View();
        }
        [HttpGet]
        public IActionResult QrCode()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult QrCode(string txtQRCode)
        {
            if (!string.IsNullOrEmpty(txtQRCode))
            {
                QRCodeGenerator _qrCode = new QRCodeGenerator();
                QRCodeData _qrCodeData = _qrCode.CreateQrCode(txtQRCode, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(_qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                return View(BitmapToBytesCode(qrCodeImage));
            }
            return NotFound();
        }
        [NonAction]
        private static Byte[] BitmapToBytesCode(Bitmap image)
        {
            using MemoryStream stream = new MemoryStream();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
        
       
        public async Task<IActionResult> Currency()
        {
            var client = new HttpClient();
            string response = await client.GetStringAsync("https://api.exchangeratesapi.io/latest");
            var currency = JsonConvert.DeserializeObject<Currency>(response);
            return View(currency);
        }
       
     
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        }
    }
}
