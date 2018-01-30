import  './bootstrap-notify';

let notify = function (title, text, type, settings) {
    let popup = $('#notifyPopup');
    if (popup.length === 0) {
        popup = $('<div />', { id: 'notifyPopup', zindex: 99999 });
        popup.append('body');
    }
    let options = {
        title: title,
        message: text
    };

    let defaultTypes = ['info', 'error', 'danger', 'success', 'warning'];
    if (defaultTypes.indexOf(type) < 0){
        type = 'info';
    }
    settings = $.extend({
        type: (type === 'error' ? 'danger' : type) || 'info',
        z_index: 999999
    }, settings);

    return $.notify(options, settings);
};

function checkBrowserNotifications(callback) {
    if (window.Notification) {
        if (window.Notification.permission !== 'granted') {
            Notification.requestPermission().then(function(result) {
                if (result !== 'denied' && callback) {
                    callback.call();
                }
            });
        } else if (callback){
            callback.call();
        }
    }
}

let browserNotify = function(title, body, onclick) {
    checkBrowserNotifications(
        function(){
            var notification = new Notification(title, {
                body: body,
            });
            notification.onclick = onclick;
        }
    );
};

document.addEventListener('DOMContentLoaded',function(){checkBrowserNotifications();});

export {notify, browserNotify} ;