using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.BusinessLayer.Services.Interfaces
{
    public interface ISmtpService
    {
      //Task SendAsync(string[] emailsTo, string body);
        Task SendAsync(string emailTo, string body, string chanel);
    }
}
