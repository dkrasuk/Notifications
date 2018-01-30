let config = {
    serviceUrl: '',
    resources:{
        notifications: {
            list: '/api/notifications',
            count: 'api/notifications/count',
            markAsRead: '/api/notifications/markAsRead',
            hubConnection: 'signalr',
            hubProxy: 'notificationsHub'
        }
    },
    messages:{
        badgeMessage: 'У вас {0} непрочитаных сообщений',
        commonHttpError: 'Ошибка обработки запроса',
        newNotifyTitle: 'Новое уведомление!'
    }
};

export default config;
