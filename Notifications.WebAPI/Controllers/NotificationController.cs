using AlfaBank.Logger;
using Notifications.Common.Utils;
using Notifications.DataAccess.DTO;
using Notifications.DataAccess.Interfaces;
using Notifications.WebAPI.Models.Binders;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ModelBinding;
using System.Web.UI;
using Notifications.WebAPI.SignalR;
using Notifications.BusinessLayer.Services.Interfaces;
using Notifications.WebAPI.Models;

namespace Notifications.WebAPI.Controllers
{
    /// <summary>
    /// Class TaskController.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    //[ExceptionFilter]
    //[ApiAuthorizeFilter]
    //[ExecutionDiagnosingFilter]
    [RoutePrefix("api/Notifications")]
    [EnableCors(origins: "*", headers: "*", methods: "GET, POST, PUT, DELETE, OPTIONS")]
    public class NotificationController : ApiController
    {
        private readonly ILogger _logger;
        private readonly ISmtpService _smtpService;
        private readonly INotificationData _notificationDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationController"/> class.
        /// </summary>
        /// <param name="logger">The logger service.</param>
        /// <param name="notificationDataService">The notification data service.</param>
        public NotificationController(ILogger logger, INotificationData notificationDataService, ISmtpService smtpService)
        {
            _logger = logger;
            _notificationDataService = notificationDataService;
            _smtpService = smtpService;
        }

        /// <summary>
        /// Create the notification.
        /// </summary>
        /// <param name="notification">Notification.</param>
        /// <returns>long?</returns>
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> CreateNotification(NotificationDTO notification)
        {
            HttpResponseMessage responce;
            try
            {
                notification.Receiver = notification.Receiver?.FormatUserName();
                notification = await _notificationDataService.CreateNotification(notification);
                _logger.Info($"NotificationController.CreateNotification [notification.Id: {notification.Id}]");

                await NotificationsHub.SendNotification(notification.Receiver, notification);

                responce = Request.CreateResponse(HttpStatusCode.OK, notification);
            }
            catch (Exception ex)
            {
                _logger.Error($"NotificationController.CreateNotification [notification.Id: {notification.Id}]", ex);
                responce = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return responce;
        }

        /// <summary>
        /// Create the notifications with Email.
        /// </summary>
        /// <param name="notify">Notification.</param>
        /// <returns>long?</returns>
        [HttpPost]
        [Route("send")]
        public async Task<HttpResponseMessage> CreateNotifications(NotificationDTO notify)
        {
            HttpResponseMessage responce;
            try
            {
                notify = await _notificationDataService.CreateNotification(notify);
                _logger.Info($"SendMailController.SendMail [notification.Id: {notify.Id} notification.Protocol: {notify.Protocol}]");

                if (Enum.IsDefined(typeof(Protocol), notify.Protocol))
                {
                    switch ((Protocol)Enum.Parse(typeof(Protocol), notify.Protocol, true))
                    {
                        case Protocol.Email:
                            await _smtpService.SendAsync(notify.Receiver, notify.Body, notify.Channel);
                            break;
                        case Protocol.SignalR:
                            await NotificationsHub.SendNotification(notify.Receiver, notify);
                            break;
                    }
                }
                responce = Request.CreateResponse(HttpStatusCode.OK, notify);
            }
            catch (Exception ex)
            {
                _logger.Error($"SendMailController.SendMail [mail.Id: {notify.Id}]", ex);
                responce = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return responce;
        }

        /// <summary>
        /// Create the notifications with Email.
        /// </summary>
        /// <param name="channel, receiver,  type,  title,  body,  protocol">Notification.</param>
        /// <returns>long?</returns>
        //[HttpPost]
        //[Route("send")]
        //public async Task<HttpResponseMessage> CreateNotifications(string channel, string receiver, string type, string title, string body, string protocol)
        //{
        //    HttpResponseMessage responce;
        //    NotificationDTO notification = new NotificationDTO()
        //    {
        //        Body = body,
        //        Receiver = receiver,
        //        Type = type,
        //        Title = title,
        //        Channel = channel,
        //        Protocol = protocol
        //    };
        //    try
        //    {
        //        notification = await _notificationDataService.CreateNotification(notification);
        //        _logger.Info($"SendMailController.SendMail [notification.Id: {notification.Id} notification.Protocol: {notification.Protocol}]");

        //        if (Enum.IsDefined(typeof(Protocol), protocol))
        //        {
        //            switch ((Protocol)Enum.Parse(typeof(Protocol), protocol, true))
        //            {
        //                case Protocol.Email:
        //                    await _smtpService.SendAsync(notification.Receiver, notification.Body, notification.Channel);
        //                    break;
        //                case Protocol.SignalR:
        //                    await NotificationsHub.SendNotification(notification.Receiver, notification);
        //                    break;
        //            }
        //        }
        //        responce = Request.CreateResponse(HttpStatusCode.OK, notification);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error($"SendMailController.SendMail [mail.Id: {notification.Id}]", ex);
        //        responce = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //    return responce;
        //}

        /// <summary>
        /// Get notifications by user login.
        /// </summary>
        /// <param name="onlyUnreaded">Take only unreaded notifications.</param>
        /// <returns>IEnumerable&lt;NotificationDTO&gt;</returns>
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetNotifications(bool onlyUnreaded = true)
        {
            HttpResponseMessage responce;

            try
            {
                string receiverLogin = Thread.CurrentPrincipal.Identity.Name.FormatUserName();

                IEnumerable<NotificationDTO> notifications =
                    await _notificationDataService.GetNotificationsByReceiver(receiverLogin, onlyUnreaded);

                responce = Request.CreateResponse(HttpStatusCode.OK, notifications);
            }
            catch (Exception ex)
            {
                _logger.Error($"NotificationController.GetNotifications", ex);
                responce = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return responce;
        }

        /// <summary>
        /// Get the count of unreaded notifications by user login.
        /// </summary>
        /// <returns>int</returns>
        [HttpGet]
        [Route("Count")]
        public async Task<HttpResponseMessage> GetCountOfUnreadedNotifications()
        {
            HttpResponseMessage responce;

            try
            {
                string receiverLogin = Thread.CurrentPrincipal.Identity.Name.FormatUserName();

                int countOfUnreadedNotifications =
                    await _notificationDataService.GetCountOfUnreadedNotifications(receiverLogin);

                responce = Request.CreateResponse(HttpStatusCode.OK, countOfUnreadedNotifications);
            }
            catch (Exception ex)
            {
                _logger.Error($"NotificationController.GetCountOfUnreadedNotifications", ex);
                responce = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return responce;
        }

        /// <summary>
        /// Set notification as Readed.
        /// </summary>
        /// <param name="notificationId">The notification Id.</param>
        /// <returns>void</returns>
        [HttpPost]
        [Route("{notificationId}")]
        public async Task<HttpResponseMessage> SetNotificationAsReaded(int notificationId)
        {
            HttpResponseMessage responce;

            try
            {
                await _notificationDataService.SetNotificationAsReaded(notificationId);

                string receiverLogin = Thread.CurrentPrincipal.Identity.Name.FormatUserName();

                await NotificationsHub.SetNotificationAsReaded(receiverLogin, notificationId);

                responce = Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.Error($"NotificationController.SetNotificationAsReaded", ex);
                responce = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return responce;
        }

        /// <summary>
        /// Set notification list as Readed.
        /// </summary>
        /// <param name="notificationId">The notification Id.</param>
        /// <returns>void</returns>
        [HttpPost]
        [Route("markAsRead")]
        public async Task<HttpResponseMessage> SetNotificationListAsReaded(
            [ModelBinder(typeof(ArrayModelBinder))]int[] notificationId)
        {
            HttpResponseMessage responce;

            try
            {
                string receiverLogin = Thread.CurrentPrincipal.Identity.Name.FormatUserName();

                foreach (var id in notificationId)
                {
                    await _notificationDataService.SetNotificationAsReaded(id);
                }

                await NotificationsHub.SetNotificationAsReaded(receiverLogin, notificationId);

                responce = Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.Error($"NotificationController.SetNotificationListAsReaded", ex);
                responce = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return responce;
        }

    }
}
