
//import signalr from 'signalr-client';

let _config = {};
let _hubConection;

class NotificationsService{
    constructor(config){
        _config = config;

        var ajaxSettings = {
            cache: false,
            xhrFields: {
                withCredentials: true
            }
        }
        //$.ajaxSetup(ajaxSettings);
    }

    getCount(){
        return $.when($.ajax({
            url: _config.resources.notificationUrls.count,
            type: 'GET',
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true
        }));
    }

    getList(isShowReaded){
        return $.when($.ajax({
            url: _config.resources.notificationUrls.list + '?onlyUnreaded=' + !isShowReaded,
            type: 'GET',
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true
        }));
    }

    markAsRead(id){
        let url = _config.resources.notificationUrls.markAsRead + '/';
        if (typeof id === 'number'){
            id = [id];
        }
        return $.when($.ajax({
            url: url,
            //dataType: 'json',
            data: JSON.stringify(id),
            type: 'POST',
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true
        }));
    }

    getHubConnection(){
        if (!_hubConection){
            //let sr =  new signalR();
            _hubConection =  $.hubConnection(
                _config.resources.notificationUrls.hubConnection,
                {
                    useDefaultPath: false,
                    EnableCrossDomain: true,
                    withCredentials: true,
                    xhrFields: {
                        withCredentials: true
                    }
                }
            );

        }
        return _hubConection;
    }
}

export default NotificationsService;