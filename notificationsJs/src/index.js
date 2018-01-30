//global dependencies
import 'signalr';
import 'jquery-slimscroll';

import config from './config';
import MainButtonComponent from './components/mainButtonComponent';
import NotificationService from './services/notificationsService'

if (!String.prototype.startsWith) {
    String.prototype.startsWith = function (searchString, position) {
        return this.substr(position || 0, searchString.length) === searchString;
    };
}

if (!String.prototype.endsWith)
    String.prototype.endsWith = function(searchStr, Position) {
        // This works much better than >= because
        // it compensates for NaN:
        if (!(Position < this.length))
            Position = this.length;
        else
            Position |= 0; // round position
        return this.substr(Position - searchStr.length,
                searchStr.length) === searchStr;
    };

let getCurrentScript = function(){
    let result = document.currentScript;
    if(!result){
        let scripts = document.getElementsByTagName('script');
        result = scripts[scripts.length - 1];
    }
    return result;
};

let bindUrl = function(base, url){
    if(base.endsWith('/') && url.startsWith('/')){
        base = base.substr(0, base.length - 1);
    } else if (!base.endsWith('/') && !url.startsWith('/')){
        base += '/';
    }
    return base + url;
};

let mapResourceUrls = function(settings){
    settings.resources.notificationUrls = {};
    for(var prop in settings.resources.notifications){
        settings.resources.notificationUrls[prop]
            = bindUrl(settings.serviceUrl, settings.resources.notifications[prop]);
    }
    return settings;
};

let currentScript = $(getCurrentScript());

let userSettings = currentScript.data('settings');
if (typeof userSettings === 'string' && userSettings.length > 0){
    userSettings = eval('(' + userSettings + ')');
}
let _config = $.extend(true, {}, config, userSettings);
_config.serviceUrl = _config.serviceUrl || currentScript.data('service-url') || '';
_config = mapResourceUrls(_config);

let mainComponent = new MainButtonComponent(null, config, new NotificationService(_config));
mainComponent.init(currentScript.parent());

export default mainComponent;
